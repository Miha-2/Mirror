using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Mathematics;
using Unity.RemoteConfig;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDisplay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private HueSlider _hueSlider;
    [SerializeField] private Transform modelTransform;
    private List<Material> _materials = new List<Material>();
    private ColorInfo[] _colorInfos;
    private Image _image;
    private float s, v, a;
    private PlayerInput _playerInput;
    private void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.UI.Enable();
        _image = GetComponent<Image>();
        Color.RGBToHSV(_image.color, out _, out this.s, out this.v);
        a = _image.color.a;
        
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            _materials.Add(renderer.material);
        
        _colorInfos = new ColorInfo[_materials.Count];

        Color.RGBToHSV(_materials[0].color, out float defaultHue, out float s, out float v);
        _colorInfos[0] = new ColorInfo(){hueOffset = 0f, saturation = s, value = v};
        
        for (int i = 1; i < _materials.Count; i++)
        {
            Color.RGBToHSV(_materials[i].color, out float thisHue, out float S, out float V);

            _colorInfos[i] = new ColorInfo(){hueOffset = thisHue - defaultHue, saturation = S, value = V};
        }
        
        _hueSlider.OnHueChanged.AddListener(HueChanged);
    }


    private void HueChanged(float hue)
    {
        Color c = Color.HSVToRGB(hue, s, v);
        _image.color = new Color(c.r, c.g, c.b, a);
        
        for (int i = 0; i < _materials.Count; i++)
        {
            ColorInfo info = _colorInfos[i];
            _materials[i].color = Color.HSVToRGB((hue + info.hueOffset) % 1, info.saturation, info.value);
        }
    }

    [SerializeField] private float dragSensitivity = 10f;
    [SerializeField] private float resetTime = .35f;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> lastTween;
    private float _dragLenght;
    private bool _isDragging;
    private float _startRotation;
    public void OnPointerDown(PointerEventData eventData)
    {
        lastTween?.Kill();
        
        _startRotation = modelTransform.rotation.eulerAngles.y;
        _isDragging = true;
    }

    private void Update()
    {
        if (!_isDragging) return;
        _dragLenght += _playerInput.UI.MouseDelta.ReadValue<Vector2>().x;

        modelTransform.rotation = Quaternion.Euler(0f, _startRotation - _dragLenght / 100 * dragSensitivity, 0f);
    }
    
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        _dragLenght = 0f;
        lastTween = modelTransform.DORotate(new Vector3(0f, 180f, 0f), resetTime).SetEase(Ease.OutBack);
    }

    private void OnDestroy() => _playerInput.UI.Disable();
}

public struct ColorInfo
{
    public float hueOffset;
    public float saturation;
    public float value;
}
