using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHittable
{
    ScriptableMaterial ScriptableMaterial { get; }
    bool Hit(HitInfo hitInfo);
    public UnityEvent OnDestroy { get; }
}
