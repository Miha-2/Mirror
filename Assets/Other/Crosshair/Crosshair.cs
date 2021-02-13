using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class Crosshair : MonoBehaviour
{
    private RectTransform sideTransform;
    // [SerializeField] private float sideOffset;
    private float maxOffset = 100f;
    //
    [SerializeField] private float smoothingSpeed = 5f;
    private bool isSmoothing;
    private float delta;
    private float finalDelta = 0.1f;

    private void Start()
    {
        sideTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isSmoothing)
        {
            if (finalDelta - delta > 0)
            {
                delta += Time.deltaTime * smoothingSpeed;
                if (delta >= finalDelta)
                {
                    delta = finalDelta;
                    isSmoothing = false;
                }
            }
            else
            {
                delta -= Time.deltaTime * smoothingSpeed;
                if (delta <= finalDelta)
                {
                    delta = finalDelta;
                    isSmoothing = false;
                }
            }

            float offset = delta * maxOffset;
            sideTransform.sizeDelta = new Vector2(12f + offset, 12f + offset);
        }
        // if (Application.isPlaying || true)
        // {
        //     
        // }
        // else
        // {
        //     sideOffset = Mathf.Clamp(sideOffset, 0f, maxOffset);
        //     sideTransform.sizeDelta = new Vector2(12f + sideOffset, 12f + sideOffset);
        // }
    }

    public float Delta
    {
        set
        {
            isSmoothing = true;
            finalDelta = Mathf.Clamp(value, 0f, 2.5f);
        }
        get => finalDelta;
    }
}
