using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    protected float sliderScale;

    [SerializeField] protected GameObject barObject = null;
    [SerializeField] private bool startOn = true;
    [SerializeField] private float smoothingTime = .05f;
    protected bool isSmoothing = false;
    protected float deltaTime;

    [SerializeField] private float startDelta;
    [SerializeField] private float endDelta;
    [SerializeField] protected float healthDelta;

    protected virtual void Awake()
    {
        barObject.SetActive(startOn);
    }

    public virtual void UpdateHealthbar(float maxHealth, float oldHealth, float newHealth)
    {
        isSmoothing = true;
        deltaTime = smoothingTime;
        startDelta = Mathf.Max(oldHealth / maxHealth, healthDelta);
        endDelta = newHealth / maxHealth;
    }

    protected virtual void Update()
    {
        if (isSmoothing)
        {
            deltaTime -= Time.deltaTime;
            if (deltaTime <= 0f)
            {
                deltaTime = 0f;
                isSmoothing = false;
            }

            healthDelta = Mathf.Lerp(endDelta, startDelta, deltaTime / smoothingTime);
        }
    }
}
