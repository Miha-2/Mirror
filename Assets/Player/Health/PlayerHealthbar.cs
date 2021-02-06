using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthbar : HealthBar
{
    [SerializeField] private RectTransform healthRect = null;
    [SerializeField] private TextMeshProUGUI maxHealthText = null, healthText = null;
    protected override void Awake()
    {
        base.Awake();
        sliderScale = healthRect.rect.width;
    }
    public override void UpdateHealthbar(float maxHealth, float oldHealth, float newHealth)
    {
        base.UpdateHealthbar(maxHealth, oldHealth, newHealth);
        if(sliderScale == 0f)
            sliderScale = healthRect.rect.width;
        maxHealthText.text = maxHealth.ToString("F0");
        healthText.text = newHealth.ToString("F0");
    }

    protected override void Update()
    {
        base.Update();

        if (isSmoothing)
        {
            healthRect.sizeDelta = new Vector2(sliderScale * healthDelta, healthRect.rect.height);
        }
    }
}