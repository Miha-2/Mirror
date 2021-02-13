using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = null;

    private void Start()
    {
        inputField.text = PlayerPrefs.GetString(PlayerInfo.Pref_Name);
    }

    public void UpdatedName()
    {
        PlayerPrefs.SetString(PlayerInfo.Pref_Name, inputField.text);
    }
}
