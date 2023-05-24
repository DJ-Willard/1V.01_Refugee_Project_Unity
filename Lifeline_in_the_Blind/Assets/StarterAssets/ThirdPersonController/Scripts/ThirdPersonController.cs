using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        // Need to decide which inventory system is being used.
        [Header("Objectives")]
        public ObjectiveHandler objectiveHandler;
        public TMP_Text objectivePromptTMP;

        [Header("Inventory")]
        [Tooltip("Manual implementation of inventory.")] // added
        public InventoryObject inventory;
        public GameObject inventory_canvas;
        public bool fill_example_inventory;
        public List<ItemObject> example_items = new List<ItemObject>();

        [HideInInspector] public bool inventoryOpen;
        [HideInInspector] public bool radioOpen;

        [Tooltip("Sound to play for radio.")]
        public AudioSource radio_static;

        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Crouch speed of the character in m/s")]
        public float CrouchSpeed = 1.0f;

        [Tooltip("How low the character will go when crouching")]
        public float CrouchHeight = 1.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        [Range(0, 1)] public float LandingAudioVolume = 0.5f;   // added
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // PW: added (global variable implementation see Scripts > ScriptableObjectTemplates
            // Access via pickedUpRadio.value
            // Initial value set in Awake() in this script.
            // Can remove from inspector with [HideInInspector]
        [Header("Testing global variable implemented via ScriptableObject")]
        [Tooltip("Global boolean accessable between scripts without object reference.")]
        public GlobalBoolVariable pickedUpRadio;

        // PW: added public sfx audio source to play pickup sound for radio
        public AudioSource GotRadio;

        // SD: added music track for when player is walking
        public AudioSource WalkingMusic;
        public AudioSource AmbientMusic;
        [Range(0, 1)] public float MinAmbientVolume = 0.0f;
        private bool walkingMusicPlaying = false;
        private float originalVolume;
        private float originalHeight;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
            originalHeight = _controller.height;
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // reset other variables
            pickedUpRadio.value = false; // review rework this?
            radioOpen = false;

            inventoryOpen = true;
            if (fill_example_inventory) FillExampleInventory();

            originalVolume  = AmbientMusic.volume;

            // OBJECTIVES
            objectiveHandler.Init();
            objectiveHandler.DisplayCurrObjectiveByRef(ref objectivePromptTMP);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            InventoryToggle();
            RadioToggle();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed;
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            // float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.crouch)
            {
                targetSpeed = CrouchSpeed;
            }
            else if (_input.sprint)
            {
                targetSpeed = SprintSpeed;
            }
            else
            {
                targetSpeed = MoveSpeed;
            }

            if (_input.crouch)
            {
                // Animation changes need to go here. Note this moves the center of the whole hitbox, 
                // either work around this in the animations or find a good way to move the hitbox without changing position
                // Something maybe like _animator.SetBool("IsCrouching", true);
                _controller.height = CrouchHeight;
                _controller.center.Set(_controller.center.x, _controller.center.y - (CrouchHeight / 2), _controller.center.z);
            }
            else
            {
                _controller.height = originalHeight;
                _controller.center.Set(_controller.center.x, _controller.center.y + (CrouchHeight / 2), _controller.center.z);
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) 
            {
                targetSpeed = 0.0f;

                // SD: Stop walking music if it's playing
                if (walkingMusicPlaying)
                {
                    WalkingMusic.Stop();
                    walkingMusicPlaying = false;
                    //AmbientMusic.volume = originalVolume;
                }
                if (AmbientMusic.volume < originalVolume)
                {
                    AmbientMusic.volume += 0.0025f;
                    if (AmbientMusic.volume > originalVolume)
                    {
                        AmbientMusic.volume = originalVolume;
                    }
                }
            }

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                // SD: Play walking music if it isn't playing
                if (!walkingMusicPlaying)
                {
                    WalkingMusic.Play();
                    walkingMusicPlaying = true;
                }
                else
                {
                    if (AmbientMusic.volume > MinAmbientVolume)
                    {
                        AmbientMusic.volume -= 0.005f;
                        if (AmbientMusic.volume < MinAmbientVolume)
                        {
                            AmbientMusic.volume = MinAmbientVolume;
                        }
                    }
                }
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

                private void InventoryToggle()
        {
            if (_input.inventoryToggle)
            {
                if (inventoryOpen)
                {
                    inventoryOpen = false;
                    inventory_canvas.SetActive(false);
                    Debug.Log("Inventory was closed.");
                }
                else 
                {
                    inventoryOpen = true;
                    inventory_canvas.SetActive(true);
                    Debug.Log("Inventory was opened.");
                }
                _input.inventoryToggle = false;     // you'd think a button wouldn't need this but it does
            }   
        }

        private void RadioToggle()
        {
            if (_input.radioToggle)
            {
                if (radioOpen)
                {
                    radioOpen = false;
                    radio_static.Stop();
                    Debug.Log("Radio was closed.");
                }
                else 
                {
                    radioOpen = true;
                    radio_static.Play();
                    Debug.Log("Radio was opened.");
                }
                _input.radioToggle = false;     // you'd think a button wouldn't need this but it does
            }   
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                // changed to new LandingAudioVolume
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), LandingAudioVolume);
            }
        }

        // PW: added function and logic for radio pickup
        // PW: added Inventory behavior
        // is there any argument that the radio object should perform this logic instead?
        private void OnTriggerEnter(Collider other)
        {
            // Begin inventory behavior
            // var infers object type. If 'other' has the item script / class add this item to
            // to the inventory. 
            var item = other.GetComponent<Item>();
            if (item)
            {
                inventory.AddItem(item.item, 1);
                Destroy(other.gameObject);
            }
            // End inventory behavior

            // Review: consider rewriting this to implement radio as inventory item
            if (other.gameObject.name == "walkie_pickup")
            {
                Debug.Log("Player collision with walkie pickup.");
                
                other.gameObject.SetActive(false);
                pickedUpRadio.value = true;
                GotRadio.Play();
                // above line is global flag see Scripts > ScriptableObjectTemplates
                // could also destroy object, probably no need
            }
        }

        private void FillExampleInventory()
        {
            Debug.Log("FillExampleInventory() called.");
            for (int i = 0; i < example_items.Count; i++)
            {
                inventory.AddItem(example_items[i], 10);
            }
        }

        public void OnUse()
        {
            // PW: well this works. very cool. The Input System automatically messages this function wherever it's at.
            // Review This means that my radio and inventory toggles are probably needlessly complex.
            // Review Annoyingly these messages are only sent within the object. Csharp or Unity events and an event manager
            //  would be a helpful change of pace for handling things like doors and pickups.
            // And they are called Update() which is even dumber. 
            Debug.Log("TPC called OnUse()");
        }

        // PW: added. Since ScriptableObjects are persistent, need to
        // the inventory to empty on game quit.
        // Could have a similar function to fill inventory, at least for debug.
        // Or use a different inventory ScriptableObject, such as tutorialInventory.
        private void OnApplicationQuit()
        {
            // Review If not using PW inventory system, this does not need to be here
            inventory.InventoryList.Clear();
            objectiveHandler.Quit();
        }
    }
}