using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public bool IsTeamOne = true;
    private void OnDrawGizmos()
    {
        Gizmos.color = IsTeamOne ? Color.blue : Color.red;
        Gizmos.DrawSphere(transform.position, 1.5f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 8.46f);
    }
}
