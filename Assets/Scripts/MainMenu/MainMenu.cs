using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.RemoteConfig;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager manager;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private CustomButton joinServer;
    private string _serverIP = string.Empty;
    private readonly string PLAYER_NAME = "PlayerName";
    
    private struct UserAttributes
    {
        
    }
    private struct AppAttributes
    {
        
    }

    private NetworkManager Manager
    {
        get
        {
            if (!manager)
                manager = FindObjectOfType<NetworkManager>();
            return manager;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(PLAYER_NAME))
        {
            string playerName = PlayerPrefs.GetString(PLAYER_NAME);
            nameInput.text = playerName;
            MenuInfo.PlayerName = playerName;
        }
        
        joinServer.IsEnabled = _serverIP != string.Empty;
        ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
        ConfigManager.FetchCompleted += OnFetchCompleted;
        FindObjectOfType<HueSlider>().OnHueChanged.AddListener(delegate(float hue) { MenuInfo.Hue = hue; });
    }

    private void OnFetchCompleted(ConfigResponse fetchData)
    {
        switch (fetchData.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("Cannot access remote configs");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("Remote configs already cached");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("Remote configs successfully cached");
                _serverIP = ConfigManager.appConfig.GetString("server_ip");
                joinServer.IsEnabled = true;
                break;
        }
    }

    public void JoinServer()
    {
        Manager.StartClient();
        Manager.networkAddress = _serverIP;
    }
    
    public void JoinIP()
    {
        Manager.StartClient();
        Manager.networkAddress = ipInput.text;
    }

    public void LocalHost()
    {
        Manager.StartHost();
    }

    public void ServerOnly()
    {
        Manager.StartServer();
    }

    public void UpdateName()
    {
        PlayerPrefs.SetString(PLAYER_NAME, nameInput.text);
        MenuInfo.PlayerName = nameInput.text;
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= OnFetchCompleted;
    }
}
