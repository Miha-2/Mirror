using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamemodeManager : NetworkBehaviour
{
    [Tooltip("Game lenght in seconds")] [SerializeField] private float gameLenght = 5 * 60;
    [SerializeField] private int killGoal = 20;
    [SerializeField] private float respawnDelay;

    #region Spawn Points

    private Spawnpoint[] _spawnPoints;
    private Spawnpoint[] SpawnPoints => _spawnPoints ??= FindObjectsOfType<Spawnpoint>();

    private List<Spawnpoint> _avalibleSpawnPoints = new List<Spawnpoint>();
    private List<Spawnpoint> AvalibleSpawnPoints
    {
        get
        {
            if (_avalibleSpawnPoints.Count == 0)
                _avalibleSpawnPoints = new List<Spawnpoint>(SpawnPoints);
            return _avalibleSpawnPoints;
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
    private void Start() => Timer = gameLenght;
    
    private void OnTimerChanged(float oldTime, float newTime) => TimerText.text = $"{Mathf.Floor(newTime / 60f):0}:{Mathf.Floor(newTime % 60):00}";
    
    
    private readonly Dictionary<NetworkConnection, ResultInfo> _resultInfos = new Dictionary<NetworkConnection, ResultInfo>();

    public void StartGame(MainNetworkManager manager)
    {
        // NetworkServer.Spawn(gameObject);
        _manager = manager;
        
        Timer = gameLenght;
        _gameStarted = true;

        foreach (KeyValuePair<NetworkConnection,ServerPlayer> pair in ServerInfo.PlayerData)
        {
            //Spawn Player
            int randomPosition = Random.Range(0, AvalibleSpawnPoints.Count);
            PlayerState player = Instantiate(manager.mapPlayerPrefab,
                AvalibleSpawnPoints[randomPosition].transform.position,
                Quaternion.identity);
            AvalibleSpawnPoints.RemoveAt(randomPosition);
            
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

        Timer -= Time.deltaTime;
        
        if(Timer <= 0f)
            EndGame();
    }

    private static int KILLS = 0;
    private static int DEATHS = 1;
    private static int ASSISTS = 2;
    
    private void OnPlayerDeath(NetworkConnection dead, NetworkConnection killer)
    {
        ServerInfo.PlayerData[dead].PlayerStats[DEATHS] += 1;
        _resultInfos[dead].Deaths += 1;
        
        if (dead != killer)
        {
            ServerInfo.PlayerData[killer].PlayerStats[KILLS] += 1;
            _resultInfos[killer].Kills += 1;
            if (ServerInfo.PlayerData[killer].PlayerStats[KILLS] == killGoal) 
                EndGame();
        }

        Debug.Log(ServerInfo.PlayerData[killer].PlayerName +" has " + ServerInfo.PlayerData[killer].PlayerStats[KILLS] + " kills");
        StartCoroutine(RespawnPlayer(dead));
    }

    private IEnumerator RespawnPlayer(NetworkConnection conn)
    {
        yield return new WaitForSeconds(respawnDelay);
        int randomPosition = Random.Range(0, AvalibleSpawnPoints.Count);
        PlayerState player = Instantiate(_manager.mapPlayerPrefab,
            AvalibleSpawnPoints[randomPosition].transform.position,
            Quaternion.identity);
        AvalibleSpawnPoints.RemoveAt(randomPosition);
            
        player.OnPlayerDeath.AddListener(OnPlayerDeath);
            
        NetworkServer.Spawn(player.gameObject, conn);
    }

    private void EndGame()
    {
        RpcOnEndGame();
        _manager.Invoke(nameof(_manager.StopGame), 7f);
    }

    [ClientRpc]
    private void RpcOnEndGame()
    {
        ResultList.OnDisable();
        ResultList.Activate(true);
    }
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(GamemodeManager))]
public class GamemodeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(14f);
        foreach (KeyValuePair<NetworkConnection,ServerPlayer> pair in ServerInfo.PlayerData)
        {
            EditorGUILayout.LabelField(pair.Value.PlayerName + ":");
            EditorGUILayout.LabelField("    Kills: " + pair.Value.PlayerStats[0]);
            EditorGUILayout.LabelField("    Deaths: " + pair.Value.PlayerStats[1]);
            EditorGUILayout.Space(10f);
        }
    }
}
#endif
