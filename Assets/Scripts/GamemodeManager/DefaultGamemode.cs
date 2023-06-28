using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MirrorProject.TestSceneTwo;
using Telepathy;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DefaultGamemode : NetworkBehaviour, IPlayerJoinable
{
    // [Tooltip("Game lenght in seconds")] [SerializeField] private float gameLenght = 5 * 60;
    [SerializeField] private int killGoal = 20;
    [SerializeField] private float respawnDelay;
    [SerializeField] private float spawnPointThreshold = 8f;
    [SerializeField] private GameObject prefabSpectator;

    #region Spawn Points

    private Spawnpoint[] _spawnPoints;
    private Spawnpoint[] SpawnPoints => _spawnPoints ??= FindObjectsOfType<Spawnpoint>();

    private List<Spawnpoint> _availableSpawnPoints = new List<Spawnpoint>();
    private List<Spawnpoint> AvailableSpawnPoints
    {
        get
        {
            if (_availableSpawnPoints.Count == 0)
                _availableSpawnPoints = new List<Spawnpoint>(SpawnPoints);
            return _availableSpawnPoints;
        }
    }

    #endregion

    private TextMeshProUGUI _timerText;
    [SyncVar(hook = nameof(OnTimerChanged))]
    private float _timer;
    private bool _gameStarted;
    private TextMeshProUGUI TimerText
    {
        get
        {
            if (_timerText == null)
                _timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            return _timerText;
        }
    }
    
    private float Timer
    {
        get => _timer;
        set
        {
            _timer = Mathf.Max(0f, value);
            TimerText.text = $"{Mathf.Floor(_timer / 60f):0}:{Mathf.Floor(_timer % 60):00}";
        }
    }

    private ResultList _resultList;
    private ResultList ResultList
    {
        get
        {
            if (!_resultList)
                _resultList = FindObjectOfType<ResultList>();
            return _resultList;
        }
    }

    private MainNetworkManager _manager;

    [ServerCallback]
    private void Start() => Timer = ServerInfo.Gamemode.time;
    
    private void OnTimerChanged(float oldTime, float newTime) => TimerText.text = $"{Mathf.Floor(newTime / 60f):0}:{Mathf.Floor(newTime % 60):00}";
    
    
    private readonly Dictionary<NetworkConnection, ResultInfo> _resultInfos = new Dictionary<NetworkConnection, ResultInfo>();

    public void StartGame(MainNetworkManager manager)
    { 
        _manager = manager;
        
        Timer = ServerInfo.Gamemode.time;
        _gameStarted = true;

        foreach (KeyValuePair<NetworkConnection,PlayerData> pair in ServerInfo.PlayerData)
        {
            //Spawn Player
            int randomPosition = Random.Range(0, AvailableSpawnPoints.Count);
            PlayerState player = Instantiate(manager.mapPlayerPrefab,
                AvailableSpawnPoints[randomPosition].transform.position,
                Quaternion.identity);
            AvailableSpawnPoints.RemoveAt(randomPosition);
            
            player.OnPlayerDeath.AddListener(OnPlayerDeath);
            
            NetworkServer.Spawn(player.gameObject, pair.Key);
            
            //Add player stats
            _resultInfos.Add(pair.Key, ResultList.AddPlayer(pair.Key, pair.Value));
        }
    }

    [ServerCallback]
    private void Update()
    {
        if(!_gameStarted) return;

        if (ServerInfo.PlayerData.Count == 0)
            _manager.StopGame();
        
        Timer -= Time.deltaTime;
        
        if(Timer <= 0f)
            EndGame();
    }

    private static int KILLS = 0;
    private static int DEATHS = 1;
    private static int ASSISTS = 2;
    
    private void OnPlayerDeath(NetworkConnection dead, NetworkConnection killer, NetworkConnection assister)
    {
        ServerInfo.PlayerData[dead].pStats.Deaths += 1;
        _resultInfos[dead].Deaths += 1;

        if (assister != null)
        {
            ServerInfo.PlayerData[assister].pStats.Assists += 1;
            _resultInfos[assister].Assists += 1;
        }
        
        if (dead != killer)
        {
            ServerInfo.PlayerData[killer].pStats.Kills += 1;
            _resultInfos[killer].Kills += 1;
            if (ServerInfo.PlayerData[killer].pStats.Kills == killGoal) 
                EndGame();
        }

        Debug.Log(ServerInfo.PlayerData[killer].pName +" has " + ServerInfo.PlayerData[killer].pStats.Kills + " kills");
        StartCoroutine(RespawnPlayer(dead, respawnDelay));
    }

    private IEnumerator RespawnPlayer(NetworkConnection conn, float delay)
    {
        yield return new WaitForSeconds(delay);

        IEnumerable<PlayerState> alivePlayers = FindObjectsOfType<PlayerState>().Where(x => !x.IsDead);

        List<Spawnpoint> outThresholdSpawnPoints = new List<Spawnpoint>();
        
        foreach (Spawnpoint spawnPoint in SpawnPoints)
        {
            bool outThreshold = alivePlayers.All(alivePlayer =>
                Vector3.Distance(spawnPoint.transform.position, alivePlayer.transform.position) >= spawnPointThreshold);

            if(outThreshold)
                outThresholdSpawnPoints.Add(spawnPoint);
        }

        Spawnpoint playerSpawnPoint = outThresholdSpawnPoints.Count > 0
            ? outThresholdSpawnPoints[Random.Range(0, outThresholdSpawnPoints.Count)]
            : SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        
        
        PlayerState player = Instantiate(_manager.mapPlayerPrefab,
            playerSpawnPoint.transform.position,
            Quaternion.identity);
            
        player.OnPlayerDeath.AddListener(OnPlayerDeath);
            
        NetworkServer.Spawn(player.gameObject, conn);
    }

    private void EndGame()
    {
        RpcOnEndGame();
        _manager.Invoke(nameof(_manager.StopGame), 3f);
    }

    [ClientRpc]
    private void RpcOnEndGame()
    {
        GameSystem.InputManager.PlayerInput.Disable();
        ResultList.Activate(true);
    }

    public void PlayerJoined(NetworkConnection conn)
    {
        //Spawn spectator
        if (ServerInfo.Gamemode.joinAsSpectator)
        {
            prefabSpectator = Resources.Load<GameObject>("Spectator Player"); //Temporary solution
            Debug.Log("Is it really null?? " + (prefabSpectator == null));
            GameObject go = Instantiate(prefabSpectator);
            NetworkServer.Spawn(go, conn);
        }
        //Spawn player
        else
        {
            int randomPosition = Random.Range(0, AvailableSpawnPoints.Count);
            PlayerState player = Instantiate(_manager.mapPlayerPrefab,
                AvailableSpawnPoints[randomPosition].transform.position,
                Quaternion.identity);
            AvailableSpawnPoints.RemoveAt(randomPosition);

            player.OnPlayerDeath.AddListener(OnPlayerDeath);

            NetworkServer.Spawn(player.gameObject, conn);

            //Add player stats
            _resultInfos.Add(conn, ResultList.AddPlayer(conn, ServerInfo.PlayerData[conn]));

            if (ServerInfo.Gamemode.isTeam)
                ServerInfo.PlayerData[conn].team = ServerInfo.Team1.Length <= ServerInfo.Team2.Length;
        }
    }
}
