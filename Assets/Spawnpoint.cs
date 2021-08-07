using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 1f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, FindObjectOfType<GamemodeManager>().SpawnPointTreshold);
    }
}
