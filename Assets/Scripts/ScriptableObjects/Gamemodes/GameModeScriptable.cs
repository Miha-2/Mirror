using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gamemode", menuName = "Scriptable/Gamemode", order = 0)]
public class GameModeScriptable : ScriptableObject
{
    public string title;
    [TextArea] public string description;
}
