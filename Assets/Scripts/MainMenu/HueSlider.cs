using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HueSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float _hue = 0f;
    [SerializeField] private float sliderLimit = 144.5f;
    [SerializeField] private RectTransform slider;
    private PlayerInput _playerInput;
    private bool doSlide = false;
    private RectTransform canvasRect;
    private float oldPosition;

    private readonly string HUE = "Hue";

    [HideInInspector] public UnityEvent<float> OnHueChanged = new UnityEvent<float>();

    private float Hue
    {
        get => _hue;
        set
        {
            OnHueChanged.Invoke(value);
            PlayerPrefs.SetFloat(HUE, value);
            _hue = value;
        }
    }

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        SetHue(PlayerPrefs.HasKey(HUE) ? PlayerPrefs.GetFloat(HUE) : Random.Range(0f, 1f));

        _playerInput = new PlayerInput();
        _playerInput.UI.Enable();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (!eventData.hovered.Contains(slider.gameObject)) return;
        oldPosition = GetCanvasXPos();
        doSlide = true;
    }

    public void OnPointerUp(PointerEventData eventData) => doSlide = false;

    private void Update()
    {
        if (!doSlide) return;
        float newPos = GetCanvasXPos();
        slider.anchoredPosition = new Vector2(Mathf.Clamp(slider.anchoredPosition.x + newPos-oldPosition, -sliderLimit, sliderLimit), 0f);
        Hue = slider.anchoredPosition.x / (sliderLimit * 2) + .5f;
        oldPosition = newPos;
    }

    private void SetHue(float hue)
    {
        Hue = hue;
        slider.anchoredPosition = new Vector2((hue -.5f) * 2 * sliderLimit, 0f);
    }
    

    private float GetCanvasXPos()
    {
        Vector2 mousePos = _playerInput.UI.MousePosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToViewportPoint(mousePos);
        return (mousePos.x - .5f) * canvasRect.rect.width;
    }
    
    private void OnDestroy() => OnHueChanged.RemoveAllListeners();
}
