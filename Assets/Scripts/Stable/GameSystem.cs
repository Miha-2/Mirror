using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

public static class GameSystem
{
    public static Queue<BulletHole> PreSpawnedBulletHoles = new Queue<BulletHole>();
    

    #region Weapons Prefabs

    private static bool _isLoaded = false;
    private static readonly List<Item> _items = new List<Item>();
    public static List<Item> Items
    {
        get
        {
            if(!_isLoaded)
                LoadPrefabs();
            return _items;
        }
    }

    private static void LoadPrefabs()
    {
        _isLoaded = true;
        Object[] os = Resources.LoadAll("Items", typeof(GameObject));
        foreach (Object o in os)
        {
            GameObject go = (GameObject) o;
            if(go.TryGetComponent(out Item w))
                _items.Add(w);
        }
        Resources.UnloadUnusedAssets();
    }

    #endregion

    public static bool OnPause = false;
    
    
    private static InputManager _inputManager;
    public static InputManager InputManager
    {
        get
        {
            if (_inputManager == null)
                _inputManager = Object.FindObjectOfType<InputManager>();
            return _inputManager;
        }
    }

}