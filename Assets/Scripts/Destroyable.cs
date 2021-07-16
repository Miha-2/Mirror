using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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


    protected virtual float Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0f, maxHealth);
    }
    
    private Renderer _objectRenderer = null;
    private Collider _objectCollider = null;
    private Rigidbody _objectRigidbody = null;
    protected void Awake()
    {
        _objectRenderer = GetComponent<Renderer>();
        _objectCollider = GetComponent<Collider>();
        _objectRigidbody = GetComponent<Rigidbody>();
        Health = maxHealth;
    }
    private void OnValidate()
    {
        if(!ScriptableMaterial)
            Debug.LogError($"Hittable object: {name} doesn't have ScriptableMaterial assigned!");
    }
    
    public UnityEvent OnDestroy { get; } = new UnityEvent();

    public virtual bool Hit(HitInfo hitInfo)
    {
        print($"{name} was hit with damage of: {hitInfo.Damage}");
        Health -= hitInfo.Damage;

        if (!(Health <= 0f)) return false;
        
        OnDestroy.Invoke();
        NetworkServer.UnSpawn(gameObject);
        
        if(respawn)
        {
            Invoke(nameof(Respawn), respawnTime);
            _objectRenderer.enabled = false;
            _objectCollider.enabled = false;
            _objectRigidbody.isKinematic = true;
        }
        else
            gameObject.SetActive(false);

        return true;
    }
    
    private void Respawn()
    {
        Health = maxHealth;
        _objectRenderer.enabled = true;
        _objectCollider.enabled = true;
        _objectRigidbody.isKinematic = false;
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
        _objectRenderer.material.SetColor("_BaseColor", new Color(gradient, gradient, gradient));
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
