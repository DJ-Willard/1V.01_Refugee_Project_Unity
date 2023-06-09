using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{

    // Third-person Controller variables
    public GameObject PlayerArmature;
    [Range(1, 15)]
    public float miniMultiplyer = 5.3f;
    [Range(1, 15)]
    public float fullMultiplyer = 7f;
    private VisualElement _root;
    private VisualElement _playerRepresentation;
    private VisualElement _mapContainer;
    private VisualElement _mapImage;
    private StarterAssets.StarterAssetsInputs _input;
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
        _input = GetComponent<StarterAssets.StarterAssetsInputs>();
        _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>
            ("Container");
        _playerRepresentation = _root.Q<VisualElement>("Player");
        _root.Q<VisualElement>("Image");
        _mapContainer = _root.Q<VisualElement>("Map");

        ToggleMap(false); // Set the map to the mini-map mode by default
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
        var multiplier = IsMapOpen ? fullMultiplyer : miniMultiplyer;
        _playerRepresentation.style.translate =
            new Translate(PlayerArmature.transform.position.x * multiplier,
            PlayerArmature.transform.position.z * -multiplier, 0);
        _playerRepresentation.style.rotate = new Rotate(
            new Angle(PlayerArmature.transform.rotation.eulerAngles.y));
        if (!IsMapOpen)
        {
            var clampWidth = _mapImage.worldBound.width / 2 -
                _mapContainer.worldBound.width / 2;
            var clampHeight = _mapImage.worldBound.height / 2 -
                _mapContainer.worldBound.height / 2;
            var xPos = Mathf.Clamp(PlayerArmature.transform.position.x * -multiplier,
                -clampWidth, clampWidth);
            var yPos = Mathf.Clamp(PlayerArmature.transform.position.z * multiplier,
                -clampHeight, clampHeight);
            _mapImage.style.translate = new Translate(xPos, yPos, 0);
        }
        else
        {
            _mapImage.style.translate = new Translate(0, 0, 0);
        }
        MapFaded = IsMapOpen && (_input.move != Vector2.zero);
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