using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float destructionTime;
    public float additionalRandomDelay;
    protected virtual void Start()
    {
        Destroy(gameObject, destructionTime + Random.Range(0f, additionalRandomDelay));
    }
}