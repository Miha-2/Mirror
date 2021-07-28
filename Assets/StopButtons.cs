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

        switch (manager.networkStatus)
        {
            case NetworkStatus.NotConnected:
                break;
            case NetworkStatus.Client:
                clientButton.SetActive(true);
                break;
            case NetworkStatus.Host:
                hostButton.SetActive(true);
                break;
            case NetworkStatus.Server:
                serverButton.SetActive(true);
                break;
        }
    }

    public void StopClient() => manager.StopClient();

    public void StopServer() => manager.StopServer();

    public void StopHost() => manager.StopHost();
}
