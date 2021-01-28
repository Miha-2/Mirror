using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Client Settings", menuName = "Player Settings/Client Settings")]
public class ClientSettings2 : ScriptableObject
{
    public float sensitivityX = .1f;
    public float sensitivityY = .1f;
}