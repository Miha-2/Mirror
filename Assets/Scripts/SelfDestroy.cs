using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SelfDestroy : NetworkBehaviour
{
    [SerializeField] protected float destructionTime;
    [SerializeField] protected float additionalRandomDelay;
    protected virtual void Start()
    {
        Destroy(gameObject, destructionTime + Random.Range(0f, additionalRandomDelay));
    }
}