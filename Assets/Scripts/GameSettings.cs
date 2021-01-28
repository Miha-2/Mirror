using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private GlobalEventSingleton globalEventSingleton = null;
    [SerializeField] private Image showHits_toggle = null;

    private void Start()
    {
        showHits_toggle.color = globalEventSingleton.clientSettings.showHits ? Color.white : Color.black;
    }

    public void ToggleHitObject(Image image)
    {
        globalEventSingleton.clientSettings.showHits = !globalEventSingleton.clientSettings.showHits;

        image.color = globalEventSingleton.clientSettings.showHits ? Color.white : Color.black;
    }
}
