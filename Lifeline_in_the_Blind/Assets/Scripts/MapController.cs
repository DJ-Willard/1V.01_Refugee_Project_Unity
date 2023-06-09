using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{

    // Third-person Controller variables
    private ThirdPersonController thirdPersonController;
    public GameObject PlayerArmature;
    [Range(1, 15)]
    public float miniMultiplyer = 5.3f;
    [Range(1, 15)]
    public float fullMultiplyer = 7f;
    private VisualElement _root;
    private VisualElement _playerRepresentation;
    private VisualElement _mapContainer;
    private VisualElement _mapImage;
    private bool IsMapOpen => _root.ClassListContains("root-container-full");
    private bool _mapFaded;
   
    public bool MapFaded
    {
        get => _mapFaded;

        set
        {
            if (_mapFaded == value)
            {
                return;
            }
            Color end = !_mapFaded ? new Color(1f, 1f, 1f, 0.5f) : Color.white;

            _mapImage.experimental.animation.Start(
                _mapImage.style.unityBackgroundImageTintColor.value, end, 500,
                (elm, val) => { elm.style.unityBackgroundImageTintColor = val; });
            _mapFaded = value;
        }
    }

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>
            ("Container");
        _playerRepresentation = _root.Q<VisualElement>("Player");
        _mapImage.style.translate = new Translate(Player.transform.position.x * -Multiplyer, Player.transform.position.z * Multiplyer, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap(!IsMapOpen);
        }
    }

    private void LateUpdate()
    {
        var multiplyer = IsMapOpen ? fullMultiplyer : miniMultiplyer;
        _playerRepresentation.style.translate =
            new Translate(PlayerArmature.transform.position.x * multiplyer,
            PlayerArmature.transform.position.z * -multiplyer, 0);
        _playerRepresentation.style.rotate = new Rotate(
            new Angle(PlayerArmature.transform.rotation.eulerAngles.y));
        if (!IsMapOpen)
        {
            var clampWidth = _mapImage.worldBound.width / 2 -
                _mapContainer.worldBound.width / 2;
            var clampHeight = _mapImage.worldBound.height / 2 -
                _mapContainer.worldBound.height / 2;
            var xPos = Mathf.Clamp(PlayerArmature.transform.position.x * -Multiplyer,
                -clampWidth, clampWidth);
            var yPos = Mathf.Clamp(PlayerArmature.transform.position.z * Multiplyer,
                -clampHeight, clampHeight);
            _mapImage.style.translate = new Translate(xPos, yPos, 0);
        }
        else
        {
            _mapImage.style.translate = new Translate(0, 0, 0);
        }
        MapFaded = IsMapOpen && thirdPersonController.IsMoving();
    }

    private void ToggleMap(bool on)
    {
        _root.EnableInClassList("root-container-mini", !on);
        _root.EnableInClassList("root-container-full", on);
    }

    private bool IsMoving()
    {
        // Check if the player is currently moving based on their input
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMoving = moveInput.magnitude > 0.1f;

        // Return the result
        return isMoving;
    }

}