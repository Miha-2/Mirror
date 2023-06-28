using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using UnityEngine;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MainNetworkManager : NetworkManager
{
    [Scene] [SerializeField] private string mapScene;
    public PlayerObject playerObject;
    public PlayerState mapPlayerPrefab = null;
    [Space]
    [SerializeField] private int preSpawnedBullets = 200;
    [SerializeField] private BulletHole holePrefab;
    
    public override void Start()
    {
        base.Start();
        RegisterWeaponPrefabs();

        // InvokeRepeating(nameof(Beat), 0f,5f);
    }

    [ContextMenu(nameof(RegisterWeaponPrefabs))]
    private void RegisterWeaponPrefabs()
    {
        foreach (Item w in GameSystem.Items)
        {
            NetworkClient.RegisterPrefab(w.gameObject);
        }
    }

    /// <summary>
    /// Pre-spawns bullet holes on a server to improve performance
    /// </summary>
    private void PreSpawnHoles()
    {
        Debug.Log(nameof(PreSpawnHoles).ToUpper());
        Vector3 pos = new Vector3(0f, -10000f, 0f);
        
        GameSystem.PreSpawnedBulletHoles = new Queue<BulletHole>();
        
        for (int i = 0; i < preSpawnedBullets; i++)
        {
            BulletHole bulletHole = Instantiate(holePrefab, pos, Quaternion.identity);
            // NetworkServer.Spawn(bulletHole.gameObject);
            bulletHole.gameObject.SetActive(false);
            GameSystem.PreSpawnedBulletHoles.Enqueue(bulletHole);
        }
    }

    
    /// <summary>
    /// Function logs the server status
    /// </summary>
    private void Beat() => Debug.Log("Server status: " + (isNetworkActive ? "active" : "inactive"));

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        ServerInfo.PlayerData.Remove(conn);

        foreach (IPlayerDisconnect playerDisconnect in FindObjectsOfType<MonoBehaviour>().OfType<IPlayerDisconnect>())
            playerDisconnect.PlayerDisconnected(conn);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        RegisterWeaponPrefabs();
    }

    /// <summary>
    /// Stops network manager regardless of it's use mode (client, server, local host)
    /// </summary>
    public void StopAny()
    {
        switch (mode)
        {
            case NetworkManagerMode.ClientOnly:
                StopClient();
                break;
            case NetworkManagerMode.Host:
                StopHost();
                break;
            case NetworkManagerMode.ServerOnly:
                StopServer();
                break;
            // StopClient();
            case NetworkManagerMode.Offline:
                Debug.LogError("Trying to stop network that is already closed!");
                break;
        }
    }

    private bool _inGame;
    
    /// <summary>
    /// Starts actual game from a game-ready lobby (on a server)
    /// </summary>
    public void StartGame()
    {
        _inGame = true;
        
        ServerChangeScene(mapScene);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        
        if(!_inGame)return;

        PreSpawnHoles();
        
        FindObjectOfType<DefaultGamemode>().StartGame(this);
    }

    public override void OnStopServer() => _inGame = false;

    
    /// <summary>
    /// Changes scene to lobby and stops game loop. Can be run on a server
    /// </summary>
    [ContextMenu("Stop game")]
    public void StopGame()
    {
        if (!_inGame) return;
        _inGame = false;
        ServerChangeScene("Lobby");
    }

    private bool tryConnecting = false;
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        tryConnecting = true;
    }

    [ClientCallback]
    private void Update()
    {
        if (tryConnecting)
        {
            if (NetworkClient.localPlayer == null)
                return;
            NetworkClient.localPlayer.GetComponent<PlayerRequests>().CmdSendData(MenuInfo.PlayerName, MenuInfo.Hue);
            tryConnecting = false;
        }
    }


    //When a server receives data from newly connected client
    public void OnServerData(NetworkConnection conn, string playerName, float playerHue)
    {
        PlayerData newPlayer = new PlayerData { pName = playerName, pHue = playerHue };
        ServerInfo.PlayerData.Add(conn, newPlayer);
        
        //Join player
        IPlayerJoinable playerJoinable = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerJoinable>().First();
        
        if (playerJoinable != null)
            playerJoinable.PlayerJoined(conn);
        else
            Debug.LogError("There was not an IPlayerJoinable script in the scene when a player joined the game!");
    }
}
