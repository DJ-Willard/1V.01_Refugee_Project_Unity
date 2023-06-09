using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    public GameObject Player;
    [Range(1, 15)]
    public float miniMultiplier = 5.3f;
    [Range(1, 15)]
    public float fullMultiplier = 7f;
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
        _root = GetComponent<UIDocument>().rootVisualElement;
        _playerRepresentation = _root.Q<VisualElement>("Player");
        _mapContainer = _root.Q<VisualElement>("MapContainer");
        _mapImage = _mapContainer.Q<VisualElement>("MapImage");

        _mapImage.style.translate = new Translate(Player.transform.position.x * -miniMultiplier, Player.transform.position.z * miniMultiplier, 0);
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
        var multiplier = IsMapOpen ? fullMultiplier : miniMultiplier;
        _playerRepresentation.style.translate =
            new Translate(Player.transform.position.x * multiplier,
            Player.transform.position.z * -multiplier, 0);
        _playerRepresentation.style.rotate = new Rotate(
            new Angle(Player.transform.rotation.eulerAngles.y));
        if (!IsMapOpen)
        {
            var clampWidth = _mapImage.worldBound.width / 2 -
                _mapContainer.worldBound.width / 2;
            var clampHeight = _mapImage.worldBound.height / 2 -
                _mapContainer.worldBound.height / 2;
            var xPos = Mathf.Clamp(Player.transform.position.x * -miniMultiplier,
                -clampWidth, clampWidth);
            var yPos = Mathf.Clamp(Player.transform.position.z * miniMultiplier,
                -clampHeight, clampHeight);
            _mapImage.style.translate = new Translate(xPos, yPos, 0);
        }
        else
        {
            _mapImage.style.translate = new Translate(0, 0, 0);
        }
        MapFaded = IsMapOpen && IsPlayerMoving;
    }

    private void ToggleMap(bool on)
    {
        _root.EnableInClassList("root-container-mini", !on);
        _root.EnableInClassList("root-container-full", on);
    }
}