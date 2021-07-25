using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Ease easeType = Ease.OutSine;
    [SerializeField] private float easeTime = .2f;
    [SerializeField] private Color hoverColor = new Color(.93f, .93f, .93f);
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private Color pressColor = new Color(.84f, .84f, .84f);
    [SerializeField] private Color disabledColor = Color.grey;
    private Image _image;
    private Vector3 _defaultScale;
    private Color _defaultColor;
    private bool isEnabled = true;
    
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (value)
            {
                transform.DOScale(_defaultScale, .1f).SetEase(easeType);
                _image.DOColor(_defaultColor, .1f).SetEase(easeType);
            }
            else
                _image.DOColor(disabledColor, .1f).SetEase(easeType);

            isEnabled = value;
        }
    }

    [Space]
    public UnityEvent onClickEvent = new UnityEvent();


    private void Awake() => _image = GetComponent<Image>();

    private void Start()
    {
        _defaultScale = transform.localScale;
        _defaultColor = _image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsEnabled) return;
        transform.DOScale(_defaultScale * hoverScale, easeTime).SetEase(easeType);
        _image.DOColor(hoverColor, easeTime).SetEase(easeType);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsEnabled) return;
        transform.DOScale(_defaultScale, easeTime).SetEase(easeType);
        _image.DOColor(Color.white, easeTime).SetEase(easeType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsEnabled) return;
        transform.DOScale(_defaultScale, easeTime / 2).SetEase(easeType);
        _image.DOColor(pressColor, easeTime).SetEase(easeType);
        
        onClickEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsEnabled) return;
        transform.DOScale(Vector3.Max(_defaultScale * hoverScale, transform.localScale), easeTime / 2).SetEase(easeType);
        if(_image.color != _defaultColor)
            _image.DOColor(hoverColor, easeTime).SetEase(easeType);
    }

    private void OnDestroy() => DOTween.CompleteAll();
}