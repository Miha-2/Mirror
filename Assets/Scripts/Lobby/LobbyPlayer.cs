using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    private LobbyInfo _lobbyInfo;
    private bool _isReady;

    public bool IsReady
    {
        get => _isReady;
        set
        {
            if(_isReady != value && isClient)
                CmdUpdateStatus(value);
            _isReady = value;
        }
    }

    public override void OnStartAuthority()
    {
        LobbyButtons lobbyButtons = FindObjectOfType<LobbyButtons>();
        if (lobbyButtons == null) return;
        lobbyButtons.LobbyPlayer = this;

        CmdSendPlayerData(MenuInfo.PlayerName, MenuInfo.Hue);
    }

    [Command]
    private void CmdSendPlayerData(string playerName, float hue)
    {
        FindObjectOfType<LobbyManager>().PlayerList.Add(this);
        ServerInfo.PlayerData[connectionToClient] = new ServerPlayer {PlayerName = playerName, Hue = hue, PlayerStats = new int[3]};
        Debug.Log("Lenght: " + ServerInfo.PlayerData[connectionToClient].PlayerStats.Length);
        _lobbyInfo = FindObjectOfType<LobbyList>().AddPlayer(hue, playerName, connectionToClient);
    }

    [Command]
    private void CmdUpdateStatus(bool status)
    {
        _isReady = status;
        _lobbyInfo.IsReady = status;
    }
}
