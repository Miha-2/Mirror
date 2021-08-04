using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public static class MenuInfo
{
    private static string _playerName = string.Empty;

    public static string PlayerName
    {
        get
        {
            if (_playerName == string.Empty)
                _playerName = "Default_" + Random.Range(1, 1000);
            return _playerName;
        }
        set => _playerName = value;
    }

    public static float Hue = 0f;

    public static string CustomIP = "localhost";
}
