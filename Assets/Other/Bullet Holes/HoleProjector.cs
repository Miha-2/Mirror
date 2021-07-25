using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class HoleProjector : NetworkBehaviour
{
    [SerializeField] private DecalProjector _decalProjector = null;
    public override void OnStartClient()
    {
        base.OnStartClient();
        // print("Started on client");
        // Sprite[] bulletHoles = GameSystem.EventSingleton.bulletHoleData.bulletHoles;
        // _decalProjector.material.SetTexture("BaseColorMap", bulletHoles[Random.Range(0, bulletHoles.Length)].texture);
    }
}
