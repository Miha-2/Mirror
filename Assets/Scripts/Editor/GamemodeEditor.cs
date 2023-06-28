using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Gamemode))]
public class GamemodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Gamemode gamemode = target as Gamemode;

        List<string> excludedProperties = new List<string>();
        
        
        if (gamemode is { } && !gamemode.respawnPlayer)
            excludedProperties.Add(nameof(gamemode.respawnTime));
        
        if(!serializedObject.FindProperty("customBehaviour").boolValue)
            excludedProperties.Add(nameof(gamemode.customGamemode));
        
        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}
