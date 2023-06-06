using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
// using System;    // conflicts with UnityEngine, just call by System.<> instead 
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
        // Need to decide which inventory system is being used.
        [Header("Debug")]
        public int CurrentObjIndexOverride;

        [Header("UI")]
        public ObjectiveHandler objectiveHandler;
        public TMP_Text objectivePromptTMP;
        public TMP_Text interactivePromptTMP;
        [Space]
        [Tooltip("Manual implementation of inventory.")] // added
        public InventoryObject inventory;
        // public GameObject inventory_canvas;
        // public bool fill_example_inventory;
        // public List<ItemObject> example_items = new List<ItemObject>();
        private Canvas CanvasStartMenu;
        private Canvas CanvasDeathMenu;
        private Canvas Canvas_UI;
        private bool StartMenuOpen = true;
        private bool DeathMenuOpen = false;

        [HideInInspector] public bool inventoryOpen;
        [HideInInspector] public bool radioOpen;

        [Header("Radio")]
        [Tooltip("Sound to play for radio.")]
        public AudioSource radio_static;
        public AudioSource radio_beep_src;
        public AudioClip radio_beep_clip;
        public float max_pitch;
        public float min_pitch;
        public float max_freq;
        public float min_freq;

        [Header("Movement")]
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
        public float CameraLookSpeed = 1f;

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
        // need radio ping rate, transform with lerp or slerp

        // SD: added music track for when player is walking
        public AudioSource WalkingMusic;
        public AudioSource AmbientMusic;
        public AudioSource EnemyMusic;
        [Range(0, 1)] public float MinAmbientVolume = 0.0f;
        [Range(0, 1)] public float MinWalkingVolume = 0.0f;
        private float maxAmbientVolume;
        private float maxWalkingVolume;
        
        private float originalHeight;

        // PW for handling interactions and tag changes
        private GameObject interactableItem = null;

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
            maxAmbientVolume = AmbientMusic.volume;
            maxWalkingVolume = WalkingMusic.volume;
            WalkingMusic.volume = MinWalkingVolume;
            EnemyMusic.volume = 0.0f;

            // OBJECTIVES AND UI
            Time.timeScale = 0f; // game is initially paused under StartMenu in the same scene
            inventoryOpen = true;
            interactivePromptTMP.gameObject.SetActive(false);
            // if (fill_example_inventory) FillExampleInventory();
            CanvasStartMenu = GameObject.Find("CanvasStartMenu").GetComponent<Canvas>();
            CanvasDeathMenu = GameObject.Find("CanvasDeathMenu").GetComponent<Canvas>();
            Canvas_UI = GameObject.Find("Canvas_UI").GetComponent<Canvas>();
            CanvasStartMenu.gameObject.SetActive(true);
            CanvasDeathMenu.gameObject.SetActive(false);
            Canvas_UI.gameObject.SetActive(false);
            
            objectiveHandler.Init(CurrentObjIndexOverride);
            objectiveHandler.DisplayCurrObjectiveByRef(ref objectivePromptTMP);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            //InventoryToggle();
            RadioToggle();
            EnemyMusic.volume -= 0.0075f;
        }

        private void LateUpdate()
        {
            CameraRotation();
            if (AmbientMusic.volume < MinAmbientVolume)
            {
                AmbientMusic.volume = MinAmbientVolume;
            }
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

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * CameraLookSpeed;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * CameraLookSpeed;
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
                /*if (walkingMusicPlaying)
                {
                    WalkingMusic.Stop();
                    walkingMusicPlaying = false;
                    //AmbientMusic.volume = originalVolume;
                }*/
                if(WalkingMusic.volume > MinWalkingVolume)
                {
                    WalkingMusic.volume -= 0.00325f;
                    if (WalkingMusic.volume < MinWalkingVolume)
                    {
                        WalkingMusic.volume = MinWalkingVolume;
                    }
                }
                if (AmbientMusic.volume < maxAmbientVolume)
                {
                    AmbientMusic.volume += 0.00125f;
                    if (AmbientMusic.volume > maxAmbientVolume)
                    {
                        AmbientMusic.volume = maxAmbientVolume;
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
                if (AmbientMusic.volume > MinAmbientVolume)
                {
                    AmbientMusic.volume -= 0.0025f;
                    if (AmbientMusic.volume < MinAmbientVolume)
                    {
                        AmbientMusic.volume = MinAmbientVolume;
                    }
                }
                if (WalkingMusic.volume < maxWalkingVolume)
                {
                    WalkingMusic.volume += 0.0025f;
                    if (WalkingMusic.volume > maxWalkingVolume)
                    {
                        WalkingMusic.volume = maxWalkingVolume;
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

/*
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
*/

        private void RadioToggle()
        {
            //if (inventory.playerHasRadio)
            if (true)
            {
                if (_input.radioToggle)
                {
                    if (radioOpen)
                    {
                        radioOpen = false;
                        // radio_static.Stop();
                        // StopCoroutine(RadioPing());  // not sure why this doesn't work. using bool radioOpen instead
                        Debug.Log("Radio was closed.");
                    }
                    else 
                    {
                        radioOpen = true;
                        Debug.Log("Radio was opened.");
                        // MAIN RADIO CODE
                        // determine whether facing current objective
                        StartCoroutine(RadioPing());
                    }
                    _input.radioToggle = false;     // you'd think a button wouldn't need this but it does
                }
            }
        }

        IEnumerator RadioPing()
        {
            GameObject objectiveGO = GameObject.Find(objectiveHandler.CurrentMainObj.GO_name);
            Debug.Log("Ping target = " + objectiveGO.name);
            GameObject camera = GameObject.Find("MainCamera");
            //Vector3 cameraLook;
            float pitch;
            float freq;
            float dot;
            float dotNormalized;
            radio_static.Play();

            // Camera direction based
            while(true)
            {
                dot = Vector3.Dot(camera.transform.forward, (objectiveGO.transform.position - camera.transform.position).normalized);
                dotNormalized = Mathf.InverseLerp(-1, 1, dot);
                
                pitch = Mathf.Lerp(min_pitch, max_pitch, dotNormalized);
                freq = Mathf.Lerp(min_freq, max_freq, dotNormalized);
                radio_beep_src.pitch = pitch;
                radio_beep_src.PlayOneShot(radio_beep_clip);
                
                // Debug.Log("camera.tf.forward" + camera.transform.forward + " ");
                // Debug.Log("Ping target = " + objectiveGO.name);
                // Debug.Log("dot, dotNormalized, pitch, freq\n" + dot + " " + dotNormalized + " " +  pitch + " " + freq);

                if (!radioOpen) break;
                yield return new WaitForSeconds(freq);
            }
            radio_static.Stop();
            yield return null;

            /* // Character direction based
            while(true)
            {
                dot = Vector3.Dot(transform.forward, (objectiveGO.transform.position - transform.position).normalized);
                dotNormalized = Mathf.InverseLerp(-1, 1, dot);
                
                pitch = Mathf.Lerp(min_pitch, max_pitch, dotNormalized);
                freq = Mathf.Lerp(min_freq, max_freq, dotNormalized);
                radio_beep_src.pitch = pitch;
                radio_beep_src.PlayOneShot(radio_beep_clip);
                
                Debug.Log("1 = facing: " + dot);
                Debug.Log("dotNormalized, pitch, freq\n" + dotNormalized + " " +  pitch + " " + freq);
                yield return new WaitForSeconds(freq);
            }
            */
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

/*
        private void FillExampleInventory()
        {
            Debug.Log("FillExampleInventory() called.");

            inventory.InventoryList.Clear();
            for (int i = 0; i < example_items.Count; i++)
            {
                inventory.AddItem(example_items[i], 10);
            }
        }
*/

        // PW: added function and logic for radio pickup
        // PW: added Inventory behavior
        // is there any argument that the radio object should perform this logic instead?
        private void OnTriggerEnter(Collider other)
        {
            // PICKUPS requiring 'E' press
            if (other.gameObject.CompareTag("InteractivePickup"))
            {
                Debug.Log("Triggered: InteractivePickup tag");
                // possible inventory behavior within
                // InteractivePickup tag should guarantee interactive prompt available
                interactivePromptTMP.text = "Press 'E' to pick up";
                interactivePromptTMP.gameObject.SetActive(true);
                interactableItem = other.gameObject;
            }
            
            // DOORS
            if (other.gameObject.CompareTag("OpenDoor") || other.gameObject.CompareTag("Door_RadioLocked"))
            {
                Debug.Log("Triggered: OpenDoor tag");

                interactivePromptTMP.text = "Press 'E' use door";
                interactivePromptTMP.gameObject.SetActive(true);
                interactableItem = other.gameObject;
            }

            // CHECKPOINTS
            if (other.gameObject.CompareTag("Checkpoint"))
            {
                Debug.Log("triggered Checkpoint tag");
                // todo logic
                // if other triggered checkpoint is next checkpoint, advance current checkpoint
                // else display locked checkpoint logic prompt?
                if (objectiveHandler.NextCheckpoint.trigger_name == other.gameObject.name)
                {
                    Debug.Log("Passed next checkpoint");
                    UpdateCheckpoint();
                    // ^^ change to coroutine for prompt display?
                }
            }

            // OBJECTIVES
            if (other.gameObject.CompareTag("Objective"))
            {
                if (other.gameObject.name == "RestaurantTrigger")
                {
                    if (objectiveHandler.CurrentMainObj.GO_name == "RestaurantTrigger")
                    {
                        StartCoroutine(UpdateObjective());
                    }
                    // todo else if here? simple walking one like this may not require it
                }
                if (other.gameObject.name == "paper_crane_pickup")
                {
                    if (objectiveHandler.CurrentMainObj.GO_name == "paper_crane_pickup")
                    {
                        StartCoroutine(UpdateObjective());
                    }
                    // todo else, display appropriate text. this could use its own coroutine
                }
            }

            /* INVENTORY REMOVED
            // Begin inventory behavior --------------------------------------
            // var infers object type. If 'other' has the item script / class add this item to
            // to the inventory. 
            var item = other.GetComponent<Item>();
            if (item)
            {
                inventory.AddItem(item.item, 1);
                Destroy(other.gameObject);
            }
            // End inventory behavior -----------------------------------------
            */ 
        }

        // mirror OnTriggerEnter behavior where necessary
        private void OnTriggerExit(Collider other)
        {
            // This might cover all interactableItem more cleanly than multiple if,
            // which are another option to extend example commented out below.
            if (interactableItem) ResetInteractablePromptAndItem();

            /*
            if (other.gameObject.CompareTag("InteractivePickup"))
            {
                ResetInteractablePromptAndItem();
            }
            */
        }

        private void ResetInteractablePromptAndItem()
        {
            interactableItem = null;
            interactivePromptTMP.gameObject.SetActive(false);
            interactivePromptTMP.text = "No interactable item in range.";   // for debug in case something goes weird
        }

        private void UpdateCheckpoint()
        {
            objectiveHandler.IncrementCheckpoint();
            // other code? should this be a coroutine?
        }

        // may want to update this to use something other than interativePromptTMP. or not. 1 might be good enough.
        IEnumerator UpdateObjective()
        {
            GotRadio.Play();
            interactivePromptTMP.text = objectiveHandler.CurrentMainObj.objectiveUnlockedText;
            interactivePromptTMP.gameObject.SetActive(true);
            objectiveHandler.IncrementMainObjective();
            objectivePromptTMP.text = objectiveHandler.GetCurrObjText();
            yield return new WaitForSeconds(4);
            interactivePromptTMP.gameObject.SetActive(false);
            interactivePromptTMP.text = "no interactive prompt set";
            yield return null;
        }

        public void OnUse()
        {
            // PW: well this works. very cool. The Input System automatically messages this function wherever it's at.
            // Review This means that my radio and inventory toggles are probably needlessly complex.
            // Review Annoyingly these messages are only sent within the object. Csharp or Unity events and an event manager
            //  would be a helpful change of pace for handling things like doors and pickups.
            // And they are called Update() which is even dumber. 
            
            Debug.Log("TPC called OnUse()");
            if (interactableItem){
                Debug.Log("used interactableItem " + interactableItem.name);
                Debug.Log("interactableItem = " + interactableItem.name);
                Debug.Log("currentMainObj.GO_name = "  + objectiveHandler.CurrentMainObj.GO_name);
                // now determine what interactableItem is and use it appropriately
                // FIRST CHECK NON-OBJECTIVE / LOCKED ITEMS
                if (interactableItem.gameObject.CompareTag("OpenDoor"))
                {
                    // BroadcastMessage calls string-named function in any mono objects and child mono objects in receiver
                    // https://docs.unity3d.com/ScriptReference/Component.BroadcastMessage.html
                    // (vs SendMessage which does not traverse hierarchy)
                    interactableItem.gameObject.BroadcastMessage("UseDoor");
                }
                
                // NOW CHECK OBJECTIVE / LOCKED ITEMS
                // Good current objective match
                else if (interactableItem.name == objectiveHandler.CurrentMainObj.GO_name)
                {
                    // todo refactor to have objhandler do more of this? could leave radio alone and finish others better
                    // if current obj match, complete objective -- no need for check
                    // else, display locked prompt text appropriately
                    // (a way to expand this would be have list of currently available objectives)
                    // DO CURRENT OBJECTIVE
                    if (interactableItem.name == "radio_pickup")
                    {
                        Destroy(interactableItem);
                        ResetInteractablePromptAndItem();
                        inventory.playerHasRadio = true;    // radio is special (may want to turn of inventory if not using)
                        GameObject[] gos = GameObject.FindGameObjectsWithTag("Door_RadioLocked");
                        foreach (GameObject go in gos)
                        {
                            go.tag = "OpenDoor";
                        }
                        GotRadio.Play(); // could be replaced by objective handling object
                        StartCoroutine(UpdateObjective()); // just for this object or any objective critical objects. See objectives in scriptableobjects folder.
                    }
                    // todo fill remaining objectives
                }
                // Bad current objective match, display appropriate text
                else 
                {
                    bool badMatchFound = false;
                    Debug.Log("bad objective match tag comparison:");
                    foreach (ObjectiveItem OI in objectiveHandler.MainObjList)
                    {
                        Debug.Log(interactableItem.tag + " ?= " + OI.lock_tag);
                        if (interactableItem.tag == OI.lock_tag)
                        {
                            interactivePromptTMP.text = OI.objectiveIncompleteText;    
                            interactivePromptTMP.gameObject.SetActive(true);

                            // error handling to debug
                            badMatchFound = true;
                            break;
                        }
                    }
                    // may want to consider other loops for non-tag locks? this scenario might arise
                    if (!badMatchFound){
                        Debug.Log("Failed to find objective match with current interactable item.");
                    }
                }
                /*
                if (interactableItem.gameObject.CompareTag("Door_RadioLocked"))
                {
                    if (inventory.playerHasRadio)
                    {
                        interactableItem.gameObject.BroadcastMessage("UseDoor");           
                    } else {
                        interactivePromptTMP.text = "Find Radio before leaving apartment";
                        interactivePromptTMP.gameObject.SetActive(true); 
                    }
                }
                */
            }
        }

        public void OnStartGame()
        {
            // time pause, see tutorial
            if (StartMenuOpen)
            {
                Time.timeScale = 1f;
                Canvas_UI.gameObject.SetActive(true);
                CanvasStartMenu.gameObject.SetActive(false);
                StartMenuOpen = false;
            }
            else if (DeathMenuOpen)
            {
                // todo logic
            }
            // else handle death menu
            // else do nothing
        }

        // Activated by 'L'
        private void OnLoadLastCheckpoint()
        {
            Vector3 newPosition = GameObject.Find(objectiveHandler.CurrentCheckpoint.transform_name).transform.position;
            Debug.Log("transform.position, newPosition: " + transform.position + ", " + newPosition);
            
            // NOTE: this line did not work until turning ON Project Settings > Physics > Auto Sync Transforms:
            // "Whether or not to automatically sync transform changes with the physics system whenever a Transform
            //  component changes."
            // Not sure if this impacts performance, or if there is a way to do it through the thirdpersoncontroller.
            // This is simplest.
            transform.position = newPosition;
        }

        public void LoadCheckpoint(string checkpoint_transform)
        {
            // do something
            // transform.position = ...
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