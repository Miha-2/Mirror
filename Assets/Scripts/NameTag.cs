using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NameTag : MonoBehaviour
{
    private Camera activeCam;
    private void Start()
    {
        activeCam = GameSystem.ActiveCamera;
        GameSystem.ActiveCameraChanged.AddListener(delegate(Camera newCam) { activeCam = newCam; });
        
    }
    private void Update()
    {
        if(!activeCam) return;
        Vector2 vector = new Vector2(transform.position.x - activeCam.transform.position.x, transform.position.z - activeCam.transform.position.z);
        float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, -angle + 90f, 0f);
    }
}