using System;
using System.Collections;
using System.Collections.Generic;
using Lobby;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gamemode", menuName = "Scriptable/Gamemode", order = 0)]
public class Gamemode : ScriptableObject, IComparable
{
    public string title;
    [TextArea] public string description;
    public bool isTeam;
    public int minPlayers = 2;
    public int time = 6 * 50;
    public int scoreCap = 20;
    public bool respawnPlayer = true;
    public float respawnTime = 3f;
    public bool joinAsSpectator;
    
    [SerializeField] private bool customBehaviour;
    public DefaultGamemode customGamemode;
    
    
    public int CompareTo(object obj) => 0;
}
