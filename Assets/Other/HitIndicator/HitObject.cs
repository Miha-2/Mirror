using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
public class HitObject : SelfDestroy
{
    protected override void Start()
    {
        if(GameSystem.ShowHitIndicator)
            base.Start();
        else
            Destroy(gameObject);
    }
}