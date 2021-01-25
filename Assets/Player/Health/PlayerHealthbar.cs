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
    public override void UpdateHealthbar(float maxHealth, float health)
    {
        if(sliderScale == 0f)
            sliderScale = healthRect.rect.width;
        healthRect.sizeDelta = new Vector2(sliderScale * health / maxHealth, healthRect.rect.height);
        maxHealthText.text = maxHealth.ToString("F0");
        healthText.text = health.ToString("F0");
    }
}
