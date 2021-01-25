using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public void ToggleHitObject(Image image)
    {
        GameSystem.ShowHitIndicator = !GameSystem.ShowHitIndicator;

        image.color = GameSystem.ShowHitIndicator ? Color.white : Color.black;
    }
}
