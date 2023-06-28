using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeManager : MonoBehaviour
{
    private void Awake()
    {
        Type type = ServerInfo.Gamemode.customGamemode == null
            ? typeof(DefaultGamemode)
            : ServerInfo.Gamemode.customGamemode.GetType();
        gameObject.AddComponent(type);
    }
}
