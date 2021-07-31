using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;

public class StopButtons : MonoBehaviour
{
    [SerializeField] private GameObject clientButton;
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject serverButton;
    private MainNetworkManager manager;
    private void Start()
    {
        manager = FindObjectOfType<MainNetworkManager>();

        if (manager == null)
            return;

        switch (manager.mode)
        {
            case NetworkManagerMode.Offline:
                break;
            case NetworkManagerMode.ClientOnly:
                clientButton.SetActive(true);
                break;
            case NetworkManagerMode.Host:
                hostButton.SetActive(true);
                break;
            case NetworkManagerMode.ServerOnly:
                serverButton.SetActive(true);
                break;
        }
    }

    public void StopAny() => manager.StopAny();
}
