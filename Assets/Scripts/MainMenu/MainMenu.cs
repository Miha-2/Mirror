using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.RemoteConfig;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private NetworkManager manager;
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
        
        ipInput.text = MenuInfo.CustomIP;
        ConfigManager.FetchConfigs(new UserAttributes(), new AppAttributes());
        ConfigManager.FetchCompleted += OnFetchCompleted;
        FindObjectOfType<HueSlider>().OnHueChanged.AddListener(delegate(float hue) { MenuInfo.Hue = hue; });
    }

    private void OnFetchCompleted(ConfigResponse fetchData)
    {
        switch (fetchData.requestOrigin)
        {
            case ConfigOrigin.Default:
                joinServer.IsEnabled = false;
                Debug.Log("Cannot access remote configs");
                break;
            case ConfigOrigin.Cached:
                // _serverIP = ConfigManager.appConfig.GetString("server_ip");
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
        if(_serverIP == String.Empty)
        {
            joinServer.IsEnabled = false;
            return;
        }

        Manager.networkAddress = _serverIP;
        Manager.StartClient();
    }
    
    public void JoinIP()
    {
        MenuInfo.CustomIP = ipInput.text;
        Manager.networkAddress = ipInput.text;
        Manager.StartClient();
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
