using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public List<LobbyPlayer> PlayerList = new List<LobbyPlayer>();

    [SerializeField] private LobbyPlayer prefabLobbyPlayer;

    [SerializeField] private TextMeshProUGUI playersInfo;
    [SerializeField] private TextMeshProUGUI startingInfo;

    [Space]
    [SerializeField] private int minPlayers = 2;

    [SerializeField] private float defaultTime = 120f;
    [SerializeField] private float readyTime = 5f;
    private bool enoughPlayers;
    private bool allReady;
    
    private int lobbyTimer;
    private float pureTimer;

    [SyncVar(hook = nameof(OnPlayerInfo))]
    private string playersInfoText;
    [SyncVar(hook = nameof(OnStartingInfo))]
    private string startingInfoText;

    private bool gameStarted;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!isServer) return;
        
#if UNITY_EDITOR
        minPlayers = 1;
#endif

        foreach (KeyValuePair<NetworkConnection, ServerPlayer> pair in ServerInfo.PlayerData)
        {
            if (PlayerList.Any(lobbyPlayer => lobbyPlayer.connectionToClient == pair.Key))
                return;
            
            LobbyPlayer player = Instantiate(prefabLobbyPlayer);
        
            PlayerList.Add(player);
        
            NetworkServer.Spawn(player.gameObject, pair.Key);
        }
    }

    [ServerCallback]
    private void Update()
    {
        for (int i = 0; i < PlayerList.Count; i++)
            if (PlayerList[i] == null)
                PlayerList.RemoveAt(i);
        
        int playerCount = PlayerList.Count;
        int readyPlayers = PlayerList.Count(lobbyPlayer => lobbyPlayer.IsReady);

        if (!enoughPlayers && playerCount >= minPlayers)
            pureTimer = defaultTime;
        enoughPlayers = playerCount >= minPlayers;

        if (!allReady && readyPlayers == playerCount)
            pureTimer = readyTime;
        else if (allReady && readyPlayers != playerCount)
            pureTimer = defaultTime;
        allReady = readyPlayers == playerCount;
        
        playersInfoText = $"Players Ready {readyPlayers}/{playerCount}";
        playersInfo.text = $"Players Ready {readyPlayers}/{playerCount}";

        if (enoughPlayers)
        {
            pureTimer -= Time.deltaTime;
            lobbyTimer = Mathf.RoundToInt(pureTimer);
            
            startingInfo.text = $"Starting in {lobbyTimer} seconds";
            startingInfoText = $"Starting in {lobbyTimer} seconds";
            
            if(pureTimer <= 0f)
                StartGame();
        }
        else
        {
            playersInfoText = "Not enough players";
            playersInfo.text = "Not enough players";

            string startingText;
            int missingPlayers = minPlayers - playerCount;
            startingText = missingPlayers == 1 ? "Waiting for 1 player" : $"Waiting for {missingPlayers} players";
            startingInfo.text = startingText;
            startingInfoText = startingText;
        }
    }

    public void PlayerJoined(NetworkConnection conn)
    {
        if (PlayerList.Any(lobbyPlayer => lobbyPlayer.connectionToClient == conn))
            return;

        LobbyPlayer player = Instantiate(prefabLobbyPlayer);
        
        PlayerList.Add(player);
        
        NetworkServer.Spawn(player.gameObject, conn);
    }

    private void OnPlayerInfo(string oldInfo, string newInfo) => playersInfo.text = newInfo;

    private void OnStartingInfo(string oldInfo, string newInfo) => startingInfo.text = newInfo;

    [ContextMenu("Force Start")]
    private void StartGame()
    {
        if(gameStarted) return;
        gameStarted = true;
        if (!isServer) return;
        Debug.Log("START GAME FROM LOBBY");
        FindObjectOfType<MainNetworkManager>().StartGame();
    }
}
