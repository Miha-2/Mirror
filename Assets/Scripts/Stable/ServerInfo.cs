using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public static class ServerInfo
{
    public static readonly Dictionary<NetworkConnection, ServerPlayer> PlayerData = new Dictionary<NetworkConnection, ServerPlayer>();
    public static readonly UnityEvent<string> AddChat = new UnityEvent<string>();
}

public struct ServerPlayer
{
    public string PlayerName;
    public float Hue;
    public int[] PlayerStats;
}