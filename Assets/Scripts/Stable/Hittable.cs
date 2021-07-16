using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hittable : MonoBehaviour, IHittable
{
    [SerializeField] private ScriptableMaterial scriptableMaterial = null;
    public ScriptableMaterial ScriptableMaterial => scriptableMaterial;

    public bool Hit(HitInfo hitInfo)
    {
        return false;}

    public UnityEvent OnDestroy { get; } = new UnityEvent();

    private void OnValidate()
    {
        if(!ScriptableMaterial)
            Debug.LogError($"Hittable object: {name} doesn't have ScriptableMaterial assigned!");
    }
}
