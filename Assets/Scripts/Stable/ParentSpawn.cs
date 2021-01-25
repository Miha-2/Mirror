using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;

public class ParentSpawn : NetworkBehaviour
{
    [SyncVar]
    [HideInInspector] public uint parentNetId;

    private bool isParented;
    private List<Renderer> objectRenderers = new List<Renderer>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (NetworkIdentity.spawned.ContainsKey(parentNetId)) 
            SelfParent();
        else
        {
            foreach (Renderer renderer in gameObject.GetComponents<Renderer>())
            {
                objectRenderers.Add(renderer);
            }
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                objectRenderers.Add(renderer);
            }

            foreach (Renderer renderer in objectRenderers)
            {
                renderer.enabled = false;
            }
        }
    }

    protected virtual void Update()
    {
        if(!isParented)
            if (NetworkIdentity.spawned.ContainsKey(parentNetId)) SelfParent();
    }

    private void SelfParent()
    {
        if(NetworkIdentity.spawned[parentNetId].TryGetComponent(out IParentSpawner parentSpawner))
            parentSpawner.ParentSpawned(transform);
        else
            Debug.LogError("The object that spawned this object is not interface: " + nameof(IParentSpawner));
        
        if(objectRenderers.Count > 0)
            foreach (Renderer renderer in objectRenderers)
            {
                renderer.enabled = true;
            }

        isParented = true;
    }
}
