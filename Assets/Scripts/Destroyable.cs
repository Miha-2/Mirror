using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

//[RequireComponent(typeof(Renderer))]
public class Destroyable : NetworkBehaviour, IHittable
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 20f;
    [SyncVar(hook = nameof(OnHealthChanged))]
    /* [HideInInspector]*/ private float health;

    [SerializeField] private ScriptableMaterial scriptableMaterial = null;
    public ScriptableMaterial ScriptableMaterial => scriptableMaterial;


    public virtual float Health
    {
        get => health;
        protected set => health = Mathf.Clamp(value, 0f, maxHealth);
    }
    
    private Renderer objectRenderer = null;


    protected void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        Health = maxHealth;

#if UNITY_EDITOR
        if(!ScriptableMaterial)
            Debug.LogError($"Hittable object: {name} doesn't have ScriptableMaterial assigned!");
#endif
    }


    public virtual bool Hit(HitInfo hitInfo)
    {
        print($"{name} was hit with damage of: {hitInfo.Damage}");
        Health -= hitInfo.Damage;
        return Health <= 0f;
    }

    protected virtual void OnHealthChanged(float oldHealth, float newHealth)
    {
        if (newHealth <= 0f)
        {
            //Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
            
        float gradient = newHealth / maxHealth;
        objectRenderer.material.SetColor("_BaseColor", new Color(gradient, gradient, gradient));
    }
}
