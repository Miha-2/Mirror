using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
    [SerializeField] private Image pointerImage;
    [HideInInspector] public bool IsOwner;
    
    
    private Color _color;
    public Color Color
    {
        get => _color;
        set
        {
            pointerImage.color = value;
            _color = value;
        }
    }

    private RectTransform _rectTransform;

    private RectTransform RectTransform
    {
        get
        {
            if(_rectTransform == null)
                RectTransform = pointerImage.transform as RectTransform;
            return _rectTransform;
        }
        set => _rectTransform = value;
    }

    private float _defaultLenght = -1f;
    private float _defaultRotation = -1f;

    /// <param name="position"> -1 ... 1 is show on a map </param>
    /// <param name="rotation"></param>
    /// <param name="zoom"></param>
    /// <param name="mapSize"> Canvas size of a minimap </param>
    public void SetPosition(Vector2 position,float rotation, float zoom, float mapSize)
    {
        if (IsOwner)
        {
            pointerImage.enabled = false;
            return;
        }

        if (_defaultLenght < 0f)
            _defaultLenght = RectTransform.anchoredPosition.x;
        if (_defaultRotation < 0f)
            _defaultRotation = RectTransform.localRotation.eulerAngles.z;
        
        //Mini Map
        if ((position * zoom).magnitude > 1)
        {
            transform.localRotation = Quaternion.Euler(0f,0f, Mathf.Atan2(position.y * zoom, position.x * zoom) * Mathf.Rad2Deg);
            RectTransform.localRotation = Quaternion.Euler(0f, 0f, _defaultRotation);
            RectTransform.anchoredPosition = new Vector2(_defaultLenght, 0f);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
            RectTransform.localRotation = Quaternion.Euler(0f, 0f, -rotation + 135f);
            RectTransform.anchoredPosition = position * (mapSize * zoom) / 2;
        }
    }
}
