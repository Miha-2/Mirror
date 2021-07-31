using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private LobbyInfo infoPrefab;
    public Transform layoutRoot;

    public LobbyInfo AddPlayer(float hue, string playerName, NetworkConnection connection)
    {
        LobbyInfo lobbyInfo = Instantiate(infoPrefab, layoutRoot);
        lobbyInfo.HueInfo = hue;
        lobbyInfo.NameInfo = playerName;
        lobbyInfo.IsReady = false;
        
        NetworkServer.Spawn(lobbyInfo.gameObject, connection);
        
        return lobbyInfo;
    }
}
