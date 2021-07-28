using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShader : MonoBehaviour
{
    [SerializeField] private Shader _shader;

    private void Start()
    {
        GetComponent<Camera>().SetReplacementShader(_shader, String.Empty);
    }
}
