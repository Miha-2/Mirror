using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(Renderer))]
public class Destroyable : NetworkBehaviour, IHittable
{
    [Header("Health")]
    [SerializeField] protected float maxHealth = 20f;
    [SyncVar(hook = nameof(OnHealthChanged))]
    /* [HideInInspector]*/ private float health;
    
    [SerializeField] private bool respawn = false;
    [SerializeField] private float respawnTime = 5f;

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

        if (!(Health <= 0f)) return false;
        Debug.Log("Unspawning: " + gameObject.name);
        NetworkServer.UnSpawn(gameObject);
        
        if(respawn)
            Invoke(nameof(Respawn), respawnTime);
        else
            gameObject.SetActive(false);

        return true;
    }
    
    private void Respawn()
    {
        Health = maxHealth;
        NetworkServer.Spawn(gameObject);
    }

    protected virtual void OnHealthChanged(float oldHealth, float newHealth)
    {
        if (newHealth <= 0f)
        {
            //Destroy(gameObject);
            // NetworkServer.UnSpawn(gameObject);
        }
            
        float gradient = newHealth / maxHealth;
        objectRenderer.material.SetColor("_BaseColor", new Color(gradient, gradient, gradient));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Destroyable), true)]
public class DestroyableEditor : NetworkBehaviourInspector
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        string exclude = String.Empty;
        if (!serializedObject.FindProperty("respawn").boolValue)
            exclude = "respawnTime";
        DrawPropertiesExcluding(serializedObject, exclude);
        serializedObject.ApplyModifiedProperties();
        DrawNetworking();
    }
}
#endif
