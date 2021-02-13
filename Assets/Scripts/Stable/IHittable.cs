using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    ScriptableMaterial ScriptableMaterial { get; }
    bool Hit(HitInfo hitInfo);
}
