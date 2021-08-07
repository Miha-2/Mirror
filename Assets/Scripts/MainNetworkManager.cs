using System;
using System.Collections;
using System.Collections.Generic;
using kcp2k;
using UnityEngine;
using Mirror;
using MirrorProject.TestSceneTwo;
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
        foreach (Item w in GameSystem.Weapons)
        {
            NetworkClient.RegisterPrefab(w.gameObject);
        }
    }

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

        Debug.Log(GameSystem.PreSpawnedBulletHoles.Count + " Bullet Holes!");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        ServerInfo.PlayerData.Add(conn, new ServerPlayer{Hue = Random.Range(0f, 1f)});

        if (!_inGame)
            FindObjectOfType<LobbyManager>().PlayerJoined(conn);
        else
            FindObjectOfType<GamemodeManager>().PlayerJoined(conn);
        //Get info data then spawn the player
    }

    private void Beat() => Debug.Log("Server status: " + (isNetworkActive ? "active" : "inactive"));

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        ServerInfo.PlayerData.Remove(conn);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        RegisterWeaponPrefabs();
    }

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
    public void StartGame()
    {
        _inGame = true;
        Debug.Log("START GAME FROM MANAGER");
        
        ServerChangeScene(mapScene);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        
        if(!_inGame)return;
        
        // foreach (KeyValuePair<NetworkConnection, ServerPlayer> conn in ServerInfo.PlayerData)
        // {
        //     Transform startPos = GetStartPosition();
        //     GameObject player = startPos != null
        //         ? Instantiate(playerObject.gameObject, startPos.position, startPos.rotation)
        //         : Instantiate(playerObject.gameObject);
        //
        //     NetworkServer.AddPlayerForConnection(conn.Key, player);
        // }
        
        PreSpawnHoles();
        
        FindObjectOfType<GamemodeManager>().StartGame(this);
    }

    public override void OnStopServer() => _inGame = false;

    [ContextMenu("Stop game")]
    public void StopGame()
    {
        if (!_inGame) return;
        _inGame = false;
        ServerChangeScene("Lobby");
    }

    [SerializeField] private NetworkManagerMode _mode;

    private void Update() => _mode = mode;
}