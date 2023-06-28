using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class SpectatorPlayer : NetworkBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private float movementSpeed = 8f; 
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private SpectatorLimit spectatorLimit;
    
    
    public override void OnStartAuthority()
    {
        playerInput = GameSystem.InputManager.PlayerInput;
        playerInput.PlayerMovement.Enable();
        spectatorLimit = FindObjectOfType<SpectatorLimit>();
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
        virtualCamera.enabled = true;
    }
    
    private void Update()
    {
        if(!hasAuthority) return;
        
        RotateCamera();
        MovePlayer();

        if (spectatorLimit != null)
            LimitPlayer();
    }

    

    private void RotateCamera()
    {
        Vector2 mouseDelta = playerInput.PlayerMovement.MouseDelta.ReadValue<Vector2>();

        float rotX = transform.rotation.eulerAngles.x + mouseDelta.y / -8;
        
        rotX = rotX < 180
            ? Mathf.Min(rotX, 90)
            : Mathf.Max(rotX, 360f + -90);
        
        transform.rotation = Quaternion.Euler( rotX, transform.rotation.eulerAngles.y + mouseDelta.x / 8, 0f);
    }
    
    private void MovePlayer()
    {
        Vector2 moveInput = playerInput.PlayerMovement.Movement.ReadValue<Vector2>();

        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 flatRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
        
        transform.position += (flatForward * (moveInput.y * Time.deltaTime) + flatRight * (moveInput.x * Time.deltaTime)) * movementSpeed;

        float fly = playerInput.PlayerMovement.Fly.ReadValue<float>();
        
        transform.position += Vector3.up * (fly * Time.deltaTime * flySpeed);
    }
    
    private void LimitPlayer()
    {
        float distance = (transform.position - spectatorLimit.transform.position).magnitude;

        if (distance <= spectatorLimit.Limit) return;

        transform.position = spectatorLimit.transform.position + (transform.position - spectatorLimit.transform.position).normalized * spectatorLimit.Limit;

    }
}
