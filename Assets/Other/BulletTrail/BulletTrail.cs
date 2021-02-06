using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BulletTrail : NetworkBehaviour
{
    [SyncVar]
    [HideInInspector] public Vector3 destination;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        StartCoroutine(MoveNextFrame());
    }

    private IEnumerator MoveNextFrame()
    {
        yield return new WaitForEndOfFrame();
        transform.position = destination;
    }
}