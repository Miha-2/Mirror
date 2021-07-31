using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode : MonoBehaviour
{
    [SerializeField] protected int minPlayers;
    [SerializeField] protected int maxPlayers;

    private void OnValidate() => maxPlayers = Mathf.Max(minPlayers, maxPlayers);
}
