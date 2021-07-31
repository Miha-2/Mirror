using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomMethods
{
    public static string HueString(string inString, float hue)
    {
        return ColorString(inString, Color.HSVToRGB(hue, 1f, 1f));
    }

    public static string ColorString(string inString, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{inString}</color>";
    }
}
