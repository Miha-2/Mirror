using System;
using System.Collections;
using System.Collections.Generic;
using Unity.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private HueSlider _hueSlider;
    [SerializeField] private Transform modelTransform;
    private List<Material> _materials = new List<Material>();
    private ColorInfo[] _colorInfos;
    [SerializeField] private float rotationSpeed = 10f;
    private Image _image;
    private float s, v, a;
    private void Start()
    {
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

    private void Update()
    {
        modelTransform.rotation = Quaternion.Euler(0f, modelTransform.eulerAngles.y + Time.deltaTime * rotationSpeed, 0f);
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
}

public struct ColorInfo
{
    public float hueOffset;
    public float saturation;
    public float value;
}
