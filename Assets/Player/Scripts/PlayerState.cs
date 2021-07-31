using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerState : Destroyable
{
    [Header("Name")]
    [SerializeField] private TextMeshPro nameDisplay = null;
    [Header("Data")]
    [SerializeField] private PlayerItem playerItem = null;
    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private CinemachineVirtualCamera playerCamera = null;
    [SerializeField] private Animator playerAnimator = null;
    [SerializeField] private CharacterController cc = null;
    [SerializeField] private Transform rightHand = null;
    [SerializeField] private Renderer[] playerRenderers = null;
    [Header("Player Health")]
    [SerializeField] private HealthBar UI_healthbar = null;
    [SerializeField] private WorldHealthBar world_healthbar = null;
    [Space]
    [SerializeField] private float regenAfter = 5f;
    [SerializeField] private float regenRate = 8f;
    private float regenTimer;

    private bool isDead;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollColliders;


    [SyncVar] private float playerHue;
    [SyncVar] private string playerName;

    protected override float Health
    {
        set
        {
            if (base.Health > value)
            {
                regenTimer = regenAfter;
                Debug.Log("Reset regen timer!");
            }
            base.Health = value;
        }
    }

    private void Start()
    {
        headshotMask = LayerMask.GetMask("Ragdoll");
        world_healthbar.owned = hasAuthority;
        nameDisplay.gameObject.SetActive(!hasAuthority);
            
        world_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        UI_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdoll(false);
    }
    public override void OnStartAuthority()
    {
        Debug.Log("AUTHORITY STARTED");
        playerCamera.gameObject.SetActive(true);
    }

    protected override void OnHealthChanged(float oldHealth, float newHealth)
    {
        newHealth = Mathf.Max(0f, newHealth);
        world_healthbar.UpdateHealthbar(maxHealth,oldHealth, newHealth);
        UI_healthbar.UpdateHealthbar(maxHealth, oldHealth, newHealth);
    }

    public override void OnStartServer()
    {
        ServerPlayer playerData = ServerInfo.PlayerData[connectionToClient];
        playerName = playerData.PlayerName;
        playerHue = playerData.Hue;
    }
    public override void OnStartClient()
    {
        nameDisplay.text = playerName;
        
        foreach (Renderer renderer in playerRenderers)
        {
            Color currentColor = renderer.material.GetColor("_BaseColor");
            float h, s, v;
            Color.RGBToHSV(currentColor, out h, out s, out v);
            h = (h + playerHue) % 1;
            currentColor = Color.HSVToRGB(h, s, v);
            renderer.material.SetColor("_BaseColor", currentColor);
        }
        
        FindObjectOfType<Minimap>().AddPointer(transform, Color.HSVToRGB(playerHue, 1f, 1f), hasAuthority);
    }
    
    
    #region Death

    [HideInInspector] public UnityEvent<NetworkConnection, NetworkConnection> OnPlayerDeath = new UnityEvent<NetworkConnection, NetworkConnection>();
    
    //Server call
    private void Death(NetworkConnection killer)
    {
        isDead = true;
        Invoke(nameof(DeathCleanup), 8f);
        OnPlayerDeath.Invoke(connectionToClient, killer);
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
        
        nameDisplay.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(false);
        if(playerItem.Item != null)
            playerItem.Item.transform.SetParent(rightHand, true);
        SetRagdoll(true);
    }

    #endregion

    
    
    private LayerMask headshotMask;
    public override void Hit(HitInfo hitInfo, Item item)
    {
        if (isDead)
            return;
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
        {
            //When a player is killed
            ServerPlayer killer = ServerInfo.PlayerData[item.connectionToClient];
            ServerPlayer victim = ServerInfo.PlayerData[connectionToClient];
            string killInfo =
                $"{CustomMethods.HueString(killer.PlayerName, killer.Hue)}" +
                $" killed {CustomMethods.HueString(victim.PlayerName, victim.Hue)}" +
                $" with {CustomMethods.ColorString(item.ItemName, Color.white)}";
                    
            ServerInfo.AddChat.Invoke(killInfo);
            
            Death(item.connectionToClient);
        }
    }

    [ServerCallback]
    private void Update()
    {
        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0f && Math.Abs(Health - maxHealth) > .01f && Health > 0f)
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

#if UNITY_EDITOR
    [ContextMenu("Commit Death")]
    [Command]
    private void CmdDie() => Death(connectionToClient);
#endif
    
    private void OnDrawGizmos()
    {
        Gizmos.color = hasAuthority ? Color.blue : Color.red;
        Gizmos.DrawLine(playerCamera.transform.position, playerCamera.transform.forward * 100f);
    }

    
    [ServerCallback]
    private void OnDestroy()
    {
        OnPlayerDeath.RemoveAllListeners();
    }
}
