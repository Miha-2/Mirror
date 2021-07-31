using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldTimer : MonoBehaviour
{
    [SerializeField] private GameObject visuals = null;
    [SerializeField] private RectTransform timerSlider = null;
    private float width;
    private float timerLenght;
    private float _time;
    private bool isActive = false;

    private void Start()
    {
        width = timerSlider.sizeDelta.x;
        visuals.SetActive(false);
    }

    private void Update()
    {
        if(!isActive)return;

        _time -= Time.deltaTime;
        if (_time <= 0f)
        {
            isActive = false;
            visuals.SetActive(false);
        }
        else
            timerSlider.sizeDelta = new Vector2(width * _time / timerLenght, timerSlider.sizeDelta.y);
    }

    public void StartTimer(float time)
    {
        if(time <= 0f) return;
        isActive = true;
        timerLenght = time;
        _time = time;
        
        visuals.SetActive(true);
    }
}
