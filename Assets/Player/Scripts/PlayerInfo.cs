using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ReSharper disable CompareOfFloatsByEqualityOperator

public static class PlayerInfo
{
    public const string Pref_Name = "Player Name";
    private static Crosshair _crosshair;

    public static Crosshair Crosshair
    {
        get
        {
            if (!_crosshair)
                _crosshair = Object.FindObjectOfType<Crosshair>();
            return _crosshair;
        } 
        set => _crosshair = value;
    }


    public static UnityEvent<MultiplierData> OnMultiplierData = new UnityEvent<MultiplierData>();
    private static MultiplierData multiplierData;
    public static  UnityEvent<bool> OnActionState = new UnityEvent<bool>();
    
    public static MultiplierData MultiplierData
    {
        get => multiplierData;
        set
        {
            if (multiplierData.Movement != value.Movement || multiplierData.Stability != value.Stability)
            {
                OnMultiplierData.Invoke(value);
            }
            multiplierData = value;
        }
    }

    public static float HueShift;
}