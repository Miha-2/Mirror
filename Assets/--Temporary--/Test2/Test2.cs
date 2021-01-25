using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public GameObject go;

    private GameObject po;

    public GameObject Po
    {
        get => po;
        set
        {
            po = value;
            print(value);
        }
    }

    private void Start()
    {
        Po = Instantiate(go);
    Invoke(nameof(Des), 4f);
    }

    private void Des()
    {
        Destroy(Po);
        Po = null;
    }
}
