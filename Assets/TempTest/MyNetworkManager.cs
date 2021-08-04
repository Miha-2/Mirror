using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [Scene, SerializeField] private string secondScene;

    private void OnGUI()
    {
        if (!NetworkServer.active) return;
        if (GUILayout.Button("New scene"))
            ServerChangeScene(secondScene);
    }
}
