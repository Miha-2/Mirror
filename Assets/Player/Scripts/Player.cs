using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

// ReSharper disable All

namespace MirrorProject.TestSceneTwo
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : Hittable
    {
        [Header("Data")]
        [SerializeField] private Camera playerCam = null;
        [SerializeField] private Transform cameraPivot = null;
        [SerializeField] private HealthBar UI_healthbar = null;
        [SerializeField] private WorldHealthBar world_healthbar = null;
        [SerializeField] protected Animator playerAnimator = null;
        private CharacterController cc = null;
        private PlayerInput PlayerInput;
        [Header("Physics")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float gravity = 10f;
        [SerializeField] private float jumpForce = 8f;
        Vector3 moveVector = Vector3.zero;
        Vector2 moveInput;
        [Header("Camera Movement")]
        [SerializeField] private float sensitivityUpDown = 1f;
        [SerializeField] private float sensitivityLeftRight = 1f;
        [SerializeField] private float maxUp = 60f;
        [SerializeField] private float maxDown = -60f;
        //private float rotationX = 0f;

        private bool jumped = false;
        private int reset = 0;

        private void Start()
        {
            world_healthbar.owned = hasAuthority;
            
            world_healthbar.UpdateHealthbar(maxHealth, health);
            UI_healthbar.UpdateHealthbar(maxHealth, health);
            playerAnimator.SetFloat("SpeedMultiplier", speed * 1.4f);
            //Later change do change layer so spectator camera can see it..
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            startPos = transform.position;
            
            GameSystem.PauseStatusChanged.AddListener(delegate(bool arg0)
            {
                if(arg0)
                    PlayerInput.Disable();
                else
                    PlayerInput.Enable();
            });
            
            if(PlayerInput == null)
                PlayerInput = new PlayerInput();
            PlayerInput.Enable();

            if(cc == null)
                cc = GetComponent<CharacterController>();
            
            playerCam.gameObject.SetActive(true);
            PlayerInput.Player.Jump.performed += context => jumped = true;
            PlayerInput.Player.ResetPosition.performed += context => reset = 3;
        }

        private void Update()
        {
            if (!hasAuthority)return;

            if (reset > 0)
            {
                transform.position = startPos;
                reset--;
                return;
            }
            
            MovePlayer();
            MoveCamera();

            // if (Input.GetMouseButtonDown(0))
            //     CmdShoot();
        }
        
        private Vector3 velocity;
        private Vector3 startPos;
        private void MovePlayer()
        {
            moveInput = PlayerInput.Player.Movement.ReadValue<Vector2>();

            // if(cc.isGrounded)
            
            
            velocity = transform.forward * (moveInput.y * speed * (moveInput.y > 0f ? 1.4f : 1f)) + transform.right * (moveInput.x * speed) / 2;

            moveVector.x = velocity.x;
            moveVector.z = velocity.z;
            if(!cc.isGrounded)
                moveVector.y -= gravity * Time.deltaTime;
            else if(jumped)
                moveVector.y = jumpForce;
            else
                moveVector.y = -cc.stepOffset / Time.deltaTime;
            
            cc.Move(moveVector * Time.deltaTime);
            jumped = false;
            
            
            playerAnimator.SetBool("IsMoving", moveInput.magnitude != 0f);
        }
        
        private void MoveCamera()
        {
            Vector2 mouseDelta = PlayerInput.Player.MouseDelta.ReadValue<Vector2>();
            
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + mouseDelta.x * sensitivityLeftRight, 0f);
            float y = cameraPivot.localRotation.eulerAngles.x - mouseDelta.y * sensitivityUpDown;
            float x = y < 180
                ? Mathf.Min(y, maxUp)
                : Mathf.Max(y, 360f + maxDown);
            cameraPivot.localRotation =
                Quaternion.Euler(x,0f, 0f);
        }

        // [Command]
        // private void CmdShoot()
        // {
        //     print(gameObject + " shot");
        //     if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, 100f))
        //     {
        //         print(gameObject + " hit: " + hit.collider.name);
        //         if(hit.collider.TryGetComponent(out Hittable hittable))
        //         {
        //             //hittable.RpcChangeColor(Random.ColorHSV(0f, 1f, .5f, 1f, 1f, 1f, 1f, 1f));
        //             if(hittable.health <= 0) {print("health too low to hit"); return; }
        //             hittable.health -= 1f;
        //         }
        //         else if (hit.collider.CompareTag("Object"))
        //         {
        //             Destroy(hit.collider.gameObject);
        //
        //             NetworkServer.Destroy(hit.collider.gameObject);
        //         }
        //     }
        // }
        private void OnDrawGizmos()
        {
            Gizmos.color = hasAuthority ? Color.blue : Color.red;
            Gizmos.DrawLine(playerCam.transform.position, playerCam.transform.forward * 100f);
        }

        protected override void OnHealthChanged(float old, float newHealth)
        {
            newHealth = Mathf.Max(0f, newHealth);
            world_healthbar.UpdateHealthbar(maxHealth, newHealth);
            UI_healthbar.UpdateHealthbar(maxHealth, newHealth);
        }
    }
}
