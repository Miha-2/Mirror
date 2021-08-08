using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorLimit : MonoBehaviour
{
    [SerializeField] private float limit;
    [SerializeField] private bool drawOpaque;

    public float Limit => limit;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.73f, 0.02f);
        if(!drawOpaque)
            Gizmos.DrawWireSphere(transform.position, limit);
        else
            Gizmos.DrawSphere(transform.position, limit);
    }
}
