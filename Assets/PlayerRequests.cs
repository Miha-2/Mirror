using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerRequests : NetworkBehaviour
{
    [Command]
    public void CmdSendData(string playerName, float playerHue)
    {
        ((MainNetworkManager)NetworkManager.singleton).OnServerData(connectionToClient, playerName, playerHue);
    }
}
