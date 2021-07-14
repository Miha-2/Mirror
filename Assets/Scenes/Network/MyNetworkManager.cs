using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    #region Server Callbacks

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"New client connected: {conn.address}");
    }
    
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"A client disconnected: {conn.address}");
    }

    private void OnServerInitialized()
    {
        Debug.Log("Server initialized!");
    }

    #endregion

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log($"Connected to server: {conn.address}");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log($"Disconnected from server: {conn.address}");
    }
}
