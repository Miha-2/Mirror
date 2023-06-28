using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;
using UnityEngine.Events;

public static class ServerInfo
{
    public static readonly Dictionary<NetworkConnection, PlayerData> PlayerData = new Dictionary<NetworkConnection, PlayerData>();
    public static readonly UnityEvent<string> AddChat = new UnityEvent<string>();
    public static Dictionary<NetworkConnection, bool> TeamLayout => PlayerData.ToDictionary(pair => pair.Key, pair => pair.Value.team);

    public static NetworkConnection[] Team1 => TeamLayout.Where(a => a.Value).Select(t => t.Key).ToArray();
    public static NetworkConnection[] Team2 => TeamLayout.Where(a => !a.Value).Select(t => t.Key).ToArray();

    public static Dictionary<bool, float> TeamHue = new Dictionary<bool, float>();

    private static Gamemode _gamemode;
    
    public static Gamemode Gamemode
    {
        get
        {
            if (_gamemode == null)
            {
                _gamemode = Resources.Load<Gamemode>("Gamemodes/Default");
                Debug.Log("Server Info: " + _gamemode.title);
            }

            return _gamemode;
        }
        set => _gamemode = value;
    }
}

// public struct ServerPlayer
// {
//     public string PlayerName;
//     public float Hue;
// }