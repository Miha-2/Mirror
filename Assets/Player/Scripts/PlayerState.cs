using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;
using UnityEngine.Events;

public class PlayerState : Hittable
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
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollColliders;

    private PlayerInput _playerInput;
    
    [HideInInspector] public UnityEvent PlayerDeath = new UnityEvent();

    private void Start()
    {
        world_healthbar.owned = hasAuthority;
            
        world_healthbar.UpdateHealthbar(maxHealth, Health);
        UI_healthbar.UpdateHealthbar(maxHealth, Health);
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        if (hasAuthority)
            _playerInput = GameSystem.PlayerGlobalInput;
        
        SetRagdoll(false);
    }

    protected override void OnHealthChanged(float old, float newHealth)
    {
        print(nameof(newHealth)+ newHealth+nameof(old)+old);
        newHealth = Mathf.Max(0f, newHealth);
        world_healthbar.UpdateHealthbar(maxHealth, newHealth);
        UI_healthbar.UpdateHealthbar(maxHealth, newHealth);

        if(!hasAuthority) return;
        if (newHealth <= 0f)
        {
            Death();
        }
    }

    [ContextMenu("Die")]
    private void Death()
    {
        Destroy(playerMovement);
        // _playerInput.PlayerMovement.Disable();
        if(playerItem.Item != null)
            playerItem.Item.enabled = false;
        
        playerCamera.gameObject.SetActive(false);
        
        CmdDeath();
    }

    [Command]
    private void CmdDeath()
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
        if(playerItem.Item != null)
            playerItem.Item.transform.SetParent(rightHand, true);
        SetRagdoll(true);
    }
    
    // private bool isRagdoll = false;
    private void SetRagdoll(bool state)
    {
        // isRagdoll = state;
            
        playerAnimator.enabled = !state;

        foreach (Rigidbody rb in ragdollRbs)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider _collider in ragdollColliders)
        {
            _collider.enabled = state;
        }

        cc.enabled = !state;
    }
}
