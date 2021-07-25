using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public static class ServerInfo
{
    public static Dictionary<int, ServerPlayer> PlayerData = new Dictionary<int, ServerPlayer>();
    public static UnityEvent<string> AddChat = new UnityEvent<string>();
}

public struct ServerPlayer
{
    public string PlayerName;
    public float Hue;
}