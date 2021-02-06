using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;
using UnityEngine.Events;

public class PlayerState : Destroyable
{
    [Header("Data")]
    [SerializeField] private PlayerItem playerItem = null;
    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private Animator playerAnimator = null;
    [SerializeField] private CharacterController cc = null;
    [SerializeField] private Transform rightHand = null;
    [Header("Health")]
    [SerializeField] private HealthBar UI_healthbar = null;
    [SerializeField] private WorldHealthBar world_healthbar = null;
    [Space]
    [SerializeField] private float regenAfter = 5f;
    [SerializeField] private float regenRate = 8f;
    private float regenTimer;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollColliders;

    private PlayerInput _playerInput;
    public override float Health
    {
        protected set
        {
            if (base.Health > value)
                regenTimer = regenAfter;
            base.Health = value;
        }
    }

    [HideInInspector] public UnityEvent PlayerDeath = new UnityEvent();

    private void Start()
    {
        headshotMask = LayerMask.GetMask("Ragdoll");
        world_healthbar.owned = hasAuthority;
            
        world_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        UI_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        if (hasAuthority)
            _playerInput = GameSystem.PlayerGlobalInput;
        
        SetRagdoll(false);
    }

    protected override void OnHealthChanged(float oldHealth, float newHealth)
    {
        newHealth = Mathf.Max(0f, newHealth);
        world_healthbar.UpdateHealthbar(maxHealth,oldHealth, newHealth);
        UI_healthbar.UpdateHealthbar(maxHealth, oldHealth, newHealth);
    }
    
    #region Death

    //Server call
    private void Death()
    {
        Invoke(nameof(DeathCleanup), 8f);
        PlayerDeath.Invoke();
        RpcDeath();
    }

    private void DeathCleanup()
    {
        if(playerItem.Item != null)
            NetworkServer.Destroy(playerItem.Item.gameObject);
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcDeath()
    {
        if (hasAuthority)
        {
            Destroy(playerMovement);
            
            if(playerItem.Item != null)
                playerItem.Item.enabled = false;
        }
        
        playerCamera.gameObject.SetActive(false);
        if(playerItem.Item != null)
            playerItem.Item.transform.SetParent(rightHand, true);
        SetRagdoll(true);
    }

    #endregion

    private LayerMask headshotMask;
    public override void Hit(HitInfo hitInfo)
    {
        float multiplier = 1f;
        Collider[] colliders = Physics.OverlapSphere(hitInfo.Point, .05f, headshotMask);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Head"))
            {
                multiplier = hitInfo.HeadshotMultiplier;
                break;
            }
        }
        Health -= hitInfo.Damage * multiplier;
        if (Health <= 0f)
            Death();
    }

    private void Update()
    {
        if(!isServer) return;
        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0f && Math.Abs(Health - maxHealth) > .01f)
            Health += regenRate * Time.deltaTime;
    }

    private void SetRagdoll(bool state)
    {
        playerAnimator.enabled = !state;

        foreach (Rigidbody rb in ragdollRbs)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider _collider in ragdollColliders)
        {
            if (_collider is CharacterController)
                continue;
            _collider.isTrigger = !state;
        }

        cc.enabled = !state;
    }
}
