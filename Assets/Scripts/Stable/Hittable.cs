using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour, IHittable
{
    [SerializeField] private ScriptableMaterial scriptableMaterial = null;
    public ScriptableMaterial ScriptableMaterial => scriptableMaterial;
    public void Hit(HitInfo hitInfo){}

    #if UNITY_EDITOR
    private void Awake()
    {
        if(!ScriptableMaterial)
            Debug.LogError($"Hittable object: {name} doesn't have ScriptableMaterial assigned!");
    }
    #endif
}
