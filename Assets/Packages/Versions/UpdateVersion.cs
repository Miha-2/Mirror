#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class UpdateVersion : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
        string oldVersion = PlayerSettings.bundleVersion;
        
        Debug.Log(oldVersion[oldVersion.Length - 1]);

        string numPart = String.Empty;
        for (int i = oldVersion.Length - 1; i >= 0; i--)
        {
            if (Char.IsNumber(oldVersion[i]))
                numPart = oldVersion[i] + numPart;
            else
                break;
        }

        if (numPart != String.Empty)
        {
            int num = int.Parse(numPart) + 1;
            string newVersion = oldVersion.Substring(0, oldVersion.Length - numPart.Length) + num;
            
            //Adds "v" in front of version if it doesn't already exist!
            if (oldVersion[0] != 'v')
                newVersion = "v" + newVersion;
            
            PlayerSettings.bundleVersion = newVersion;
            Debug.Log($"The app version was updated from {oldVersion} to {newVersion}");
        }
        else
            Debug.LogError("Invalid version type");
    }
}

#endif