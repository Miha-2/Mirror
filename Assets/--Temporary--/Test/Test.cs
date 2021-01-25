using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Test : NetworkBehaviour
{
    public GameObject go;
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if(!isServer) return;
        
        CmdSpawn();
    }

    [Command]
    private void CmdSpawn()
    {
        GameObject a = Instantiate(go); //ostane na serverju!!
        NetworkServer.Spawn(a, connectionToClient); //gre na cliente!!
    }
}
