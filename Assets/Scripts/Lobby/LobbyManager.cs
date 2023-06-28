using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Lobby
{
    public class LobbyManager : NetworkBehaviour, IPlayerJoinable
    {
        /*[HideInInspector]*/
        public List<LobbyPlayer> PlayerList = new List<LobbyPlayer>(); //???????

        public LobbyButtons lobbyButtons;
        [SerializeField] private LobbyPlayer prefabLobbyPlayer;
        public LobbyList lobbyList;

        [SyncVar]
        private bool votingStage;
        public bool VotingStage => votingStage;

        [HideInInspector] [SyncVar] public Gamemode gamemode;
        
        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void OnStartServer()
        {
            foreach (NetworkConnection conn in ServerInfo.PlayerData.Keys)
                PlayerJoined(conn);

            votingStage = true;
            lobbyVoting.SetVotes();
            gamemode = ServerInfo.Gamemode;
            Debug.Log("Selected: " + gamemode.title);
        }

        public override void OnStartClient()
        {
            lobbyButtons.gameObject.SetActive(!votingStage);
            gamemodeInfo.gameObject.SetActive(!votingStage);
            // LobbyButtons.switchTeams.gameObject.SetActive(gamemode.isTeam);

            // gamemodeInfo.Gamemode = gamemode;
        }
        public void PlayerJoined(NetworkConnection conn)
        {
            if (PlayerList.Any(lobbyPlayer => lobbyPlayer.connectionToClient == conn))
                return;

            // lobbyList.AddPlayer(conn);
            
            // return;
            Debug.Log(nameof(PlayerJoined) + " with name: " + ServerInfo.PlayerData[conn]);
            LobbyPlayer player = Instantiate(prefabLobbyPlayer);

            PlayerList.Add(player);

            NetworkServer.Spawn(player.gameObject, conn);
        }


        [SerializeField] private LobbyVoting lobbyVoting;
        [ServerCallback]
        private void Update()
        {
            if(votingStage)
                lobbyVoting.ManagerUpdate(this);
            else
                ReadyUpdate();
        }
        
        
        
        #region Ready Stage
        [SerializeField] private LobbyGamemodeInfo gamemodeInfo;

        [SerializeField] private TextMeshProUGUI playersInfo;
        [SerializeField] private TextMeshProUGUI startingInfo;
        
        [SyncVar(hook = nameof(OnPlayerInfo))] private string playersInfoText;

        [SyncVar(hook = nameof(OnStartingInfo))] private string startingInfoText;


        public void StartReadyStage()
        {
            lobbyList.lobbyInfos.RemoveAll(info => info == null);
            foreach (LobbyInfo lobbyInfo in lobbyList.lobbyInfos)
                lobbyInfo.IsVoting = false;
            votingStage = false;
            lobbyList.ServerReadyStage();
            RpcReadyStage();
        }
        
        [ClientRpc]
        private void RpcReadyStage()
        {
            lobbyVoting.gameObject.SetActive(false);
            lobbyButtons.gameObject.SetActive(true);
            gamemodeInfo.gameObject.SetActive(true);
        }

        private string PlayersInfo
        {
            set
            {
                playersInfoText = value;
                playersInfo.text = value;
            }
        }
        private string StartingInfo
        {
            set
            {
                startingInfoText = value;
                startingInfo.text = value;
            }
        }
        
        private bool gameStarted;
        private int RoundedTimer => Mathf.RoundToInt(_startGameTimer);

        [SerializeField] private float defaultTime = 120f;
        [SerializeField] private float readyTime = 5f;
        private bool enoughPlayers;
        private bool allReady;

        private float _startGameTimer;

        private void ReadyUpdate()
        {
            //TO BE IMPROVED!!
            
            for (int i = 0; i < PlayerList.Count; i++)
                if (PlayerList[i] == null)
                    PlayerList.RemoveAt(i);

            int playerCount = PlayerList.Count;
            int readyPlayers = PlayerList.Count(lobbyPlayer => lobbyPlayer.IsReady);

            int minPlayers = gamemode.minPlayers;
            
#if UNITY_EDITOR
            minPlayers = 1;
#endif
            
            //Reset timer
            if (!enoughPlayers && playerCount >= minPlayers)
                _startGameTimer = defaultTime;
            
            enoughPlayers = playerCount >= minPlayers;

            //Ready timer
            if (!allReady && readyPlayers == playerCount)
                _startGameTimer = readyTime;
            
            //Reset timer
            else if (allReady && readyPlayers != playerCount)
                _startGameTimer = defaultTime;
            allReady = readyPlayers == playerCount;

            PlayersInfo = $"Players Ready {readyPlayers}/{playerCount}";

            
            if(!gamemode.isTeam)
            {
                if (enoughPlayers)
                    RunTimer();
                else
                {
                    PlayersInfo = "Not enough players";

                    int missingPlayers = minPlayers - playerCount;
                    StartingInfo = missingPlayers == 1
                        ? "Waiting for 1 player"
                        : $"Waiting for {missingPlayers} players";
                }
            }
            else
            {
                if (ServerInfo.Team1.Length == 0)
                    StartingInfo = "Team 1 is empty!!";
#if !UNITY_EDITOR
                else if (ServerInfo.Team2.Length == 0)
                    StartingInfo = "Team 2 is empty!!";
#endif
                else
                    RunTimer();
            }
        }
        
        private void RunTimer()
        {
            _startGameTimer -= Time.deltaTime;
                
            StartingInfo = $"Starting in {RoundedTimer} seconds";

            if (_startGameTimer <= 0f)
                StartGame();
        }


        private void OnPlayerInfo(string oldInfo, string newInfo) => playersInfo.text = newInfo;

        private void OnStartingInfo(string oldInfo, string newInfo) => startingInfo.text = newInfo;
        
        [ContextMenu("Force Start")]
        private void StartGame()
        {
            if (!isServer) return;
            if (gameStarted) return;
            gameStarted = true;

            FindObjectOfType<MainNetworkManager>().StartGame();
        }

        #endregion
    }
}
