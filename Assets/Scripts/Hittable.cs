using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//[RequireComponent(typeof(Renderer))]
public class Hittable : NetworkBehaviour
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 20f;
    [SyncVar(hook = nameof(OnHealthChanged))]
    /* [HideInInspector]*/ private float health;
    
    
    public float Health
    {
        get => health;
        private set => health = value;
    }
    
    private Renderer objectRenderer = null;


    protected void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        Health = maxHealth;
    }

    public void Hit(float damage)
    {
        print($"{name} was hit with damage of: {damage}");
        Health -= damage;
    }

    protected virtual void OnHealthChanged(float old, float newHealth)
    {
        if (newHealth <= 0f)
        {
            //Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
            
        float gradient = newHealth / maxHealth;
        objectRenderer.material.color = new Color(gradient, gradient, gradient);
    }
}
