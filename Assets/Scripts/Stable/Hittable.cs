using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Hittable : MonoBehaviour, IHittable
{
    [SerializeField] private ScriptableMaterial scriptableMaterial = null;
    public ScriptableMaterial ScriptableMaterial => scriptableMaterial;

    public void Hit(HitInfo hitInfo, Item item){ }

    public UnityEvent OnDestroyed { get; } = new UnityEvent();

    private void OnValidate()
    {
        if(!ScriptableMaterial)
            Debug.LogError($"Hittable object: {name} doesn't have ScriptableMaterial assigned!");
    }
}
