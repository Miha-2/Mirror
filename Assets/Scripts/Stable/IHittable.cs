using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public interface IHittable
{
    ScriptableMaterial ScriptableMaterial { get; }
    void Hit(HitInfo hitInfo, Item item);
    public UnityEvent OnDestroyed { get; }
}
