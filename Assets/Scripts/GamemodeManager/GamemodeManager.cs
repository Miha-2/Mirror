using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamemodeManager : MonoBehaviour
{
    [Tooltip("Game lenght in seconds")] [SerializeField] private float gameLenght = 5 * 60;
    [SerializeField] private int killGoal = 20;

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
            TimerText.text = $"{Mathf.Floor(value / 60):0}:{Mathf.Floor(value % 60):00}";
            _timer = value;
        }
    }

    public void StartGame(MainNetworkManager manager)
    {
        Timer = gameLenght;
        _gameStarted = true;
        
        foreach (KeyValuePair<NetworkConnection,ServerPlayer> pair in ServerInfo.PlayerData)
        {
            int randomPosition = Random.Range(0, AvalibleSpawnPoints.Count);
            PlayerState player = Instantiate(manager.mapPlayerPrefab,
                AvalibleSpawnPoints[randomPosition].transform.position,
                Quaternion.identity);
            AvalibleSpawnPoints.RemoveAt(randomPosition);
            
            player.OnPlayerDeath.AddListener(OnPlayerDeath);
            
            NetworkServer.Spawn(player.gameObject, pair.Key);
        }
    }

    [ServerCallback]
    private void Update()
    {
        if(!_gameStarted) return;

        Timer -= Time.deltaTime;
    }

    private static int KILLS = 0;
    private static int DEATHS = 1;
    
    private static void OnPlayerDeath(NetworkConnection dead, NetworkConnection killer)
    {
        ServerInfo.PlayerData[dead].PlayerStats[DEATHS] += 1;

        Debug.Log(ServerInfo.PlayerData[dead].PlayerName +" has " + ServerInfo.PlayerData[dead].PlayerStats.Length + " deaths");       
        
        if (dead != killer)
            ServerInfo.PlayerData[killer].PlayerStats[KILLS] += 1;
        Debug.Log(ServerInfo.PlayerData[killer].PlayerName +" has " + ServerInfo.PlayerData[killer].PlayerStats[KILLS] + " kills");
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
