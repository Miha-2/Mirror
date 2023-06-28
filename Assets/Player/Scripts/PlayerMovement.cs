using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

// ReSharper disable All


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] private Transform cameraPivot = null;
    [SerializeField] protected Animator playerAnimator = null;
    private CharacterController cc = null;
    private PlayerInput PlayerInput;
    [Header("Physics")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] Vector3 moveVector = Vector3.zero;
    [SerializeField] private bool isGrounded;
    Vector2 moveInput;
    [Header("Camera Movement")]
    [SerializeField] private float sensitivityUpDown = 1f;
    [SerializeField] private float sensitivityLeftRight = 1f;
    [SerializeField] private float maxUp = 60f;
    [SerializeField] private float maxDown = -60f;

    //private float rotationX = 0f;

    private bool jumped = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private MultiplierData movementMultiplier;
    private bool inAction;
    
    private void Start()
    {
        playerAnimator.SetFloat("SpeedMultiplier", speed * 1.4f);
        //Later change do change layer so spectator camera can see it..
    }

    private bool authorityStarted = false;
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        // _globalEventSingleton = GameSystem.EventSingleton;
        authorityStarted = true;
        
        startPos = transform.position;
        movementMultiplier = new MultiplierData(){Movement = 1f, Stability = 1f};
        PlayerInfo.OnActionState.AddListener(delegate(bool state) { inAction = state;  });

        if (PlayerInput == null)
            PlayerInput = GameSystem.InputManager.PlayerInput;
        
        if(!GameSystem.OnPause)
            PlayerInput.PlayerMovement.Enable();

        if(cc == null)
            cc = GetComponent<CharacterController>();
        
        PlayerInput.PlayerMovement.Jump.performed += context => jumped = true;
        // PlayerInput.PlayerMovement.ResetPosition.performed += context => reset = 3;
        PlayerInput.PlayerMovement.Sprint.performed += context => isSprinting = true;
        PlayerInput.PlayerMovement.Sprint.canceled += context => isSprinting = false;
    }

    private void Update()
    {
        if (!hasAuthority) return;
        if(!authorityStarted)return;

        // if (reset > 0)
        // {
        //     transform.position = startPos;
        //     reset--;
        //     return;
        // }
        
        MovePlayer();
        MoveCamera();

        // if (Input.GetMouseButtonDown(0))
        //     CmdShoot();
    }
    
    private Vector3 velocity;
    private Vector3 startPos;
    private bool wasGrounded;
    private void MovePlayer()
    {
        isGrounded = cc.isGrounded;
        float forewardSpeed = isSprinting /* && cc.isGrounded */ ? sprintSpeed : speed;
        moveInput = PlayerInput.PlayerMovement.Movement.ReadValue<Vector2>();

        if (!inAction)
            velocity = transform.forward * (moveInput.y * (moveInput.y > 0f ? forewardSpeed : speed)) +
                       transform.right * (moveInput.x * (moveInput.y > 0f ? forewardSpeed : speed)) / 1.3333f;
        else
            velocity = transform.forward * moveInput.y * speed / 1.5f + transform.right * moveInput.x * speed / 3f;

        moveVector.x = velocity.x;
        moveVector.z = velocity.z;

        if (wasGrounded && !cc.isGrounded && moveVector.y < 0f)
        {
            moveVector.y = -gravity * Time.deltaTime;
            Debug.Log("DID IT");
        }

        if(!cc.isGrounded)
            moveVector.y -= gravity * Time.deltaTime;
        else if(jumped)
            moveVector.y = jumpForce;
        else
            moveVector.y = -cc.skinWidth / Time.deltaTime;
        
        wasGrounded = cc.isGrounded;

        cc.Move(moveVector * Time.deltaTime);
        jumped = false;
        #region Multipliers

        float mMultiplier = cc.isGrounded ? 1f : 1.3f;
        if (moveInput == Vector2.zero)
            mMultiplier *= 1f;
        else if (isSprinting && moveInput.y > 0f)
            mMultiplier *= 1.55f;
        else
            mMultiplier *= 1.3f;

        float sMultiplier = isCrouching ? .6f : 1f;

        PlayerInfo.MultiplierData = new MultiplierData {Movement = mMultiplier, Stability = sMultiplier};

        #endregion
            
        playerAnimator.SetBool("IsMoving", moveInput.magnitude != 0f);
    }
    
    private void MoveCamera()
    {
        Vector2 mouseDelta = PlayerInput.PlayerMovement.MouseDelta.ReadValue<Vector2>();
        
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + mouseDelta.x * sensitivityLeftRight, 0f);
        float y = cameraPivot.localRotation.eulerAngles.x - mouseDelta.y * sensitivityUpDown;
        float x = y < 180
            ? Mathf.Min(y, maxUp)
            : Mathf.Max(y, 360f + maxDown);
        cameraPivot.localRotation =
            Quaternion.Euler(x,0f, 0f);
    }
}

public struct MultiplierData
{
    public float Movement;
    public float Stability;
}