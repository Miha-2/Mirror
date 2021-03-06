﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WorldHealthBar : HealthBar
{
    [SerializeField] private Transform healthSlider = null;
    [SerializeField] private TextMeshPro fullHealthText = null, healthText = null;
    [HideInInspector] public bool owned;

    protected override void Awake()
    {
        base.Awake();
        sliderScale = healthSlider.localScale.x;
    }
    public override void UpdateHealthbar(float maxHealth, float oldHealth, float newHealth)
    {
        base.UpdateHealthbar(maxHealth, oldHealth, newHealth);
        if(owned) return; //later maybe check which camera is active (Camera.Active -> later static class)
        
        barObject.SetActive(true);
        if(sliderScale == 0f)
            sliderScale = healthSlider.localScale.x;
        fullHealthText.text = maxHealth.ToString("F0");
        healthText.text = newHealth.ToString("F0");
        if(newHealth <= 0f)
            gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (isSmoothing)
        {
            float sliderSize = sliderScale * healthDelta;
            healthSlider.transform.localScale = new Vector3(sliderSize, healthSlider.transform.localScale.y, healthSlider.transform.localScale.z);
            healthSlider.transform.localPosition = new Vector3(-(sliderScale - sliderSize) / 2, 0f, 0f);
        }
    }
}