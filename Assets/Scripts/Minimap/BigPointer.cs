using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigPointer : MonoBehaviour
{
    public Image Image;
    private RectTransform _rectTransform;
    private RectTransform RectTransform
    {
        get
        {
            if(_rectTransform == null)
                _rectTransform = transform as RectTransform;
            return _rectTransform;
        }
        set => _rectTransform = value;
    }

    public void SetPosition(Vector2 position, float rotation, float mapSize)
    {
        {
            RectTransform.anchoredPosition = position * (mapSize / 2);
            RectTransform.rotation = Quaternion.Euler(0f, 0f, -rotation + 135f);
        }
    }
}
