using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

//Item => an object a player can hold and interact with
//In this class setup inputs (and other general logic)
public class Item : ParentSpawn
{
    [SerializeField] private string itemName;
    protected string ItemName
    {
        get
        {
            if (itemName != String.Empty)
                return itemName;
            return $"[{typeof(Item)}]";
        }
    }
    [HideInInspector] public CinemachineVirtualCamera Camera;

    public AnimationClip anim_draw;
    protected PlayerInput _playerInput;
    [HideInInspector] public UnityEvent<int> amountChanged = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<float> actionStarted = new UnityEvent<float>();
    [HideInInspector] public UnityEvent actionEnded = new UnityEvent();

    private void Awake()
    {
        name += Random.Range(1, 999);
    }

    public virtual void SetupItem(PlayerInput playerInput)
    {
        _playerInput = playerInput;
    }
    
    public virtual void RemoveInput(){}
}