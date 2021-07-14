using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayVersion : MonoBehaviour
{
    private void Start() => GetComponent<TextMeshProUGUI>().text = Application.version;
}
