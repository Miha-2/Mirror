using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerState : Destroyable
{
    [Header("Name")]
    [SerializeField] private TextMeshPro nameDisplay = null;
    [SyncVar(hook = nameof(OnNameChanged))]
    private string playerName;
    [Header("Data")]
    [SerializeField] private PlayerItem playerItem = null;
    [SerializeField] private PlayerMovement playerMovement = null;
    [SerializeField] private Camera playerCamera = null;
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

    private bool isDead = false;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollColliders;


    [SyncVar]
    private float hueShift;

    private PlayerInput _playerInput;
    public override float Health
    {
        protected set
        {
            if (base.Health > value)
            {
                regenTimer = regenAfter;
                Debug.Log("Reset regen timer!");
            }
            base.Health = value;
        }
    }

    [HideInInspector] public UnityEvent PlayerDeath = new UnityEvent();

    private void Start()
    {
        headshotMask = LayerMask.GetMask("Ragdoll");
        world_healthbar.owned = hasAuthority;
        nameDisplay.gameObject.SetActive(!hasAuthority);
            
        world_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        UI_healthbar.UpdateHealthbar(maxHealth,Health ,Health);
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        if (hasAuthority)
        {
            string setName = PlayerPrefs.GetString(PlayerInfo.Pref_Name, $"Mr. {Random.Range(1, 1000)}");
            nameDisplay.text = setName;
            CmdUpdateName(setName);
            
            _playerInput = GameSystem.PlayerGlobalInput;
        }

        SetRagdoll(false);
    }

    [Command]
    private void CmdUpdateName(string newName)
    {
        playerName = newName;
        ServerInfo.PlayerData[connectionToClient.connectionId] = new ServerPlayer {HueShift =  ServerInfo.PlayerData[connectionToClient.connectionId].HueShift, PlayerName = newName};
    }

    protected override void OnHealthChanged(float oldHealth, float newHealth)
    {
        newHealth = Mathf.Max(0f, newHealth);
        world_healthbar.UpdateHealthbar(maxHealth,oldHealth, newHealth);
        UI_healthbar.UpdateHealthbar(maxHealth, oldHealth, newHealth);
    }

    private void OnNameChanged(string oldName, string newName)
    {
        if (!newName.Equals(string.Empty)) nameDisplay.text = newName;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        hueShift = ServerInfo.PlayerData[connectionToClient.connectionId].HueShift;
    }

    public override void OnStartClient()
    {
        foreach (Renderer renderer in playerRenderers)
        {
            Color currentColor = renderer.material.GetColor("_BaseColor");
            float h, s, v;
            Color.RGBToHSV(currentColor, out h, out s, out v);
            h = (h + hueShift) % 1;
            currentColor = Color.HSVToRGB(h, s, v);
            renderer.material.SetColor("_BaseColor", currentColor);
        }
    }
    #region Death

    //Server call
    private void Death()
    {
        isDead = true;
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
        
        nameDisplay.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(false);
        if(playerItem.Item != null)
            playerItem.Item.transform.SetParent(rightHand, true);
        SetRagdoll(true);
    }

    #endregion

    private LayerMask headshotMask;
    public override bool Hit(HitInfo hitInfo)
    {
        if (isDead)
            return false;
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
            Death();
            return true;
        }

        return false;
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
    private void CmdDie()
    {
        Death();
    }
#endif
}
