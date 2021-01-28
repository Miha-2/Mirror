using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MirrorProject.TestSceneTwo;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerItem : NetworkBehaviour, IParentSpawner
{
    private Item _item;
    

    [SerializeField] private TextMeshProUGUI amountText = null;

    [SerializeField] private Camera _playerCamera = null;
    [SerializeField] private Transform weaponPivot = null;
    
    [SerializeField] private Animator _animator = null;
    private NetworkAnimator _networkAnimator;
    private AnimatorOverrideController _overrideController;

    private PlayerInput PlayerInput;
    
    //Action
    [SerializeField] private Image actionIndicator = null;
    private bool inAction;
    private float actionLenght;


    // private bool isActive = false;
    
    
    public Item Item
    {
        get => _item;
        set
        {
            if (_item && value && hasAuthority)
                _networkAnimator.SetTrigger("ReDraw");

            _item = value;

            if (!_item) return;
            if(!hasAuthority) return;
            _item.SetupItem(PlayerInput);
            _item.amountChanged.AddListener(delegate(int arg0) { amountText.text = arg0.ToString(); });
            _item.actionStarted.AddListener(delegate(float arg0)
            {
                actionIndicator.fillAmount = 0f;
                inAction = true;
                actionLenght = arg0;
                actionIndicator.enabled = true;
            });
            _item.actionEnded.AddListener(delegate
            {
                inAction = false;
                actionIndicator.enabled = false;
            });
            
            
            _overrideController["draw_weapon"] = _item.anim_draw;
            _animator.SetBool("HasWeapon", true);
        }
    }

    private void Start()
    {
        amountText.enabled = false;
        _overrideController = (AnimatorOverrideController)_animator.runtimeAnimatorController;
        _networkAnimator = GetComponent<NetworkAnimator>();
        print(hasAuthority);
        if (hasAuthority)
        {
            PlayerInput = GameSystem.PlayerGlobalInput;
            if(!GameSystem.OnPause)
                PlayerInput.ItemInteractions.Enable();
        }
    }

    public void EquipItem(Item item)
    {
        /*if (Item) Destroy(Item.gameObject);
        else*/ amountText.enabled = true;

        CmdEquipItem(GameSystem.WeaponToByte(item));
        
        // IsActive = true;
    }

    [Command]
    private void CmdEquipItem(byte weaponID)
    {
        if (Item) NetworkServer.Destroy(Item.gameObject);
        
        Item prefab = GameSystem.ByteToWeapon(weaponID);
        
        Item = Instantiate(prefab, Vector3.zero, quaternion.identity, weaponPivot);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = quaternion.identity;
        Item.parentNetId = netId;
        Item.Camera = _playerCamera;

        NetworkServer.Spawn(Item.gameObject, connectionToClient);
    }

    public void RemoveItem()
    {
        if(!Item) return;
        amountText.enabled = false;
        
        Item.RemoveInput();
        Item.amountChanged.RemoveAllListeners();
        Item.actionStarted.RemoveAllListeners();
        Item.actionEnded.RemoveAllListeners();
        _animator.SetBool("HasWeapon", false);
        
        CmdRemoveItem();
        

        //Destroy(Item.gameObject);
        // IsActive = false;
        // _animator.SetLayerWeight(1, 0.00001f);
    }

    [Command]
    private void CmdRemoveItem()
    {
        if(Item)
            NetworkServer.Destroy(Item.gameObject);
    }

    private void Update()
    {
        if (inAction)
            actionIndicator.fillAmount += Time.deltaTime / actionLenght;
    }

    public void ParentSpawned(Transform t)
    {
        if (t.TryGetComponent(out Item item))
        {
            if(Item == item) return;
            t.SetParent(weaponPivot);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;

            // if(hasAuthority)
                Item = item;
        }
    }

    private void OnEnable()
    {
        // if (!hasAuthority) return;
        // PlayerInput?.ItemInteractions.Enable();
    }

    private void OnDisable()
    {
        // if (!hasAuthority) return;
        // PlayerInput?.ItemInteractions.Disable();
    }
}
