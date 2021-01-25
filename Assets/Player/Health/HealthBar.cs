using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBar : MonoBehaviour
{
    protected float sliderScale;

    [SerializeField] protected GameObject barObject = null;
    [SerializeField] private bool startOn = true;

    protected virtual void Awake()
    {
        barObject.SetActive(startOn);
    }
    public abstract void UpdateHealthbar(float maxHealth, float health);
}
