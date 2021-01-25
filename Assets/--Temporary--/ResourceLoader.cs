using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceLoader : NetworkBehaviour, IParentSpawner
{
    //Load resources..
    [ContextMenu("AssetDatabase/LoadAllAssetsAtPath")]
    private void TestLoad()
    {
        Object[] weapons = Resources.LoadAll("GunPrefabs", typeof(Item));
        foreach (var o in weapons)
        {
            var prefab = (Item) o;
            print(prefab.anim_draw.name);
        }
    }
    
    
    //Spawning parent objects
    [Space]
    [SerializeField] private ParentSpawn obj;
    public Transform rootTransform = null;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        ParentSpawn o = Instantiate(obj);
        o.parentNetId = netId;
        NetworkServer.Spawn(o.gameObject);
    }

    public void ParentSpawned(Transform t)
    {
        t.SetParent(rootTransform);
    }
}
