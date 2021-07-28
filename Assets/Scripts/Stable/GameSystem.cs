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
    private static List<Item> _weapons = new List<Item>();
    public static List<Item> Weapons
    {
        get
        {
            if(!_isLoaded)
                LoadPrefabs();
            return _weapons;
        }
    }

    private static void LoadPrefabs()
    {
        _isLoaded = true;
        Object[] os = Resources.LoadAll("Weapons", typeof(GameObject));
        foreach (Object o in os)
        {
            GameObject go = (GameObject) o;
            if(go.TryGetComponent(out Item w))
                _weapons.Add(w);
        }
    }

    #endregion

    #region Weapon indexing

    public static byte WeaponToByte(Item w)
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            if (Weapons[i] == w)
                return (byte) i;
        }

        Debug.LogError("No similar weapon found");
        return 255;
    }

    public static Item ByteToWeapon(byte b)
    {
        if (b >= Weapons.Count)
        {
            Debug.LogError("The byte was bigger than the amount of weapons in resource folder");
            return null;
        }
        return Weapons[b];
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