using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MirrorProject.TestSceneTwo;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Gun : Item

{
    #region Gun Properties
    [Header("Gun Type")]
    [SerializeField] private bool isAutomatic = true;
    [SerializeField] private bool canAim = true;

    [Header("Gun Properties")]
    [SerializeField] [Tooltip("Range at wich damage starts dropping")]
    protected float startDropoff = 100f;
    [SerializeField] protected float range = 200f;
    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float minDamage = 8f;
    [SerializeField] protected float headShotMultiplier = 1.5f;
    // [SerializeField] protected float hitForce = 8f;
    [SerializeField] private int magazineSize = 40;
    [SerializeField] private int avalibleAmmo = 160;
    [SerializeField] protected float fireRate = 0.1f; //DONE
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float bulletPenetration = 10;
    [Header("Bullet Spread")]
    [SerializeField] private float maxSpread = .5f;
    [SerializeField] private float spreadStep = .85f;
    
    private float _spread;
    private float Spread
    {
        get => _spread;
        set
        {
            PlayerInfo.Crosshair.Delta = value;
            _spread = value;
        }
    }
    
    private float _maxSpread;
    private float _minSpread;
    private float _spreadStep;

    // [SerializeField] protected float recoilAmount = 10f;
    [SerializeField] [Tooltip("Higher is more accurate, 1 is the same")] protected float aimAccuracyMultiplier = 10f; //Only in inspector if can aim..

    [Header("Gun Other")]
    [SerializeField] private Transform exitPoint = null;
    [SerializeField] private AudioClip shotEffect = null;
    [SerializeField] private AudioSource shotSourcePrefab = null;
    private Queue<AudioSource> shotSources = new Queue<AudioSource>();

    private float nextFire;
    private int ammo;
    private float reloadTimer;
    private bool isReloading;
    private LayerMask hitMask;

    private int Ammo
    {
        get => ammo;
        set
        {
            ammo = value;
            amountChanged.Invoke(ammo + "|" + avalibleAmmo);
            if(ammo == 0)
                TryReload();
        }
    }


    #endregion

    private Action<InputAction.CallbackContext> context;

    private bool shooting;

    #region Input

    public override void SetupItem(PlayerInput playerInput)
    {
        base.SetupItem(playerInput);
        print("Setup inputs for gun: " + name);
        _playerInput.ItemInteractions.PrimaryAction.performed += Input_PrimaryActionPerformed;
        _playerInput.ItemInteractions.PrimaryAction.canceled += Input_PrimaryActionCanceled;
        _playerInput.ItemInteractions.Reload.performed += Input_ReloadPerformed;
    }

    private void Input_PrimaryActionPerformed(InputAction.CallbackContext ctx)
    {
        if(isAutomatic)
            shooting = true;
        else if (Ammo > 0 && !isReloading && nextFire <= 0f) Shoot();
    }

    private void Input_PrimaryActionCanceled(InputAction.CallbackContext ctx)
    {
        shooting = false;
    }

    private void Input_ReloadPerformed(InputAction.CallbackContext ctx) => TryReload();

    public override void RemoveInput()
    {
        base.RemoveInput();
        
        _playerInput.ItemInteractions.PrimaryAction.performed -= Input_PrimaryActionPerformed;
        _playerInput.ItemInteractions.PrimaryAction.canceled -= Input_PrimaryActionCanceled;
        _playerInput.ItemInteractions.Reload.performed -= Input_ReloadPerformed;
    }

    #endregion

    private void Start()
    {
        Ammo = magazineSize;
        hitMask = LayerMask.GetMask("Ragdoll", "Default", "Movable");
        if (hasAuthority)
        {
            MultiplierData _data = PlayerInfo.MultiplierData;
            _maxSpread = maxSpread * _data.Movement;
            _minSpread = Mathf.Min((_data.Movement - 1f) / 3, _maxSpread);
            _spreadStep = spreadStep * _data.Stability;
            PlayerInfo.OnMultiplierData.AddListener(delegate(MultiplierData data)
            {
                _maxSpread = maxSpread * data.Movement;
                _minSpread = Mathf.Min((data.Movement - 1f) / 3, _maxSpread);
                _spreadStep = spreadStep * data.Stability;
            });
        }

        //PreGenerate shot sounds
        int shotAudios = Mathf.CeilToInt(shotEffect.length / fireRate) + 6;
        for (int i = 0; i < shotAudios; i++)
        {
            AudioSource audioSource = Instantiate(shotSourcePrefab, transform);
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.transform.localRotation = Quaternion.identity;
            shotSources.Enqueue(audioSource);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (reloadTimer > 0f)
            reloadTimer -= Time.deltaTime;
        else if(isReloading)
        {
            isReloading = false;
            int additionalAmmo = Math.Min(magazineSize - Ammo, avalibleAmmo);
            avalibleAmmo -= additionalAmmo;
            Ammo += additionalAmmo;
            actionEnded.Invoke();
        }

        //Comment ___________
        if(Spread > _minSpread/* && (!shooting || isReloading)*/)
            Spread = Mathf.Max(Spread - 2 *Time.deltaTime, _minSpread);
        else if (Spread < _minSpread)
            Spread = _minSpread;

        if (nextFire > 0f)
            nextFire -= Time.deltaTime;
        else if (shooting && Ammo > 0 && !isReloading)
            Shoot();
    }
    
    private void Shoot()
    {
        nextFire = fireRate;
        Ammo -= 1;
        PlayShotSound();
        CmdShoot(Spread * 5f);
        Spread = Mathf.Min(Spread + _spreadStep, _maxSpread);
    }

    

    private float noHoleThreshold = .05f;
    [Command]
    private void CmdShoot(float spread)
    {
        Vector3 shotDir = BulletSpread(spread);
        RpcClientShoot();
        
        #region Forward Cast
        List<RaycastHit> enterHits = Physics.RaycastAll(Camera.transform.position + shotDir * .2f, shotDir, range, hitMask).ToList();
        enterHits = enterHits.OrderBy(hit => hit.distance).ToList();
        #endregion
        
        if(enterHits.Count == 0) return;

        #region Exit Cast

        #region ExitCastInfo

        Vector3 cameraPosition = Camera.transform.position;
        bool rangeInCollider =
            Physics.CheckSphere(cameraPosition + shotDir * (range + .2f), .1f, hitMask);
        
        Vector3 reverseCastPoint =
            !rangeInCollider
                ? cameraPosition + shotDir * (range + .2f)
                : enterHits[enterHits.Count - 1].point;
        
        float reverseCastDistance = Vector3.Distance(cameraPosition + shotDir * .2f, reverseCastPoint);

        #endregion
        List<RaycastHit> exitHits = Physics.RaycastAll(reverseCastPoint, -shotDir, reverseCastDistance, hitMask).ToList();
        exitHits = exitHits.OrderBy(hit => hit.distance).ToList();
        exitHits.Reverse();
        #endregion
        

        // Debug.Log($"Enter hits: {enterHits.Count}, Exit hits: {exitHits.Count}");
        enters = enterHits;
        exits = exitHits;
        
        #region ApplyHit

        float availablePower = 1f;
        for (int i = 0; i < enterHits.Count; i++)
        {
            //Apply hit
            IHittable hittable = enterHits[i].collider.GetComponentInParent<IHittable>();

            bool isPlayer = false;
            if (hittable != null)
            {
                if(hittable is PlayerState playerState)
                {
                    isPlayer = true;
                    if (playerState.netId == parentNetId)
                        continue;
                }

                float hitPower =
                    i < exitHits.Count
                    ? Mathf.Min(new PenetrationInfo
                                {
                                    EnterPoint = enterHits[i].point,
                                    ExitPoint = exitHits[i].point,
                                    MaterialDensity = hittable.ScriptableMaterial.Density >= 0f
                                        ? hittable.ScriptableMaterial.Density
                                        : Mathf.Infinity
                                }.PenetrationValue / bulletPenetration, availablePower)
                    : availablePower;
                availablePower -= hitPower;

                float dropoff;
                if(enterHits[i].distance <= startDropoff)
                    dropoff = 1f;
                else if(enterHits[i].distance >= range)
                    dropoff = 0f;
                else
                    dropoff = 1 - Mathf.Sqrt((enterHits[i].distance - startDropoff) / (range - startDropoff));

                //Spawn enter hole
                if (!isPlayer)
                {
                    bool spawnHole = true;
                    if (i > 0) 
                        spawnHole = enterHits[i].distance - (200 - exitHits[i - 1].distance) > noHoleThreshold;

                    if (spawnHole)
                    {
                        BulletHole bulletHole = GameSystem.PreSpawnedBulletHoles.Dequeue();

                        bulletHole.transform.position = enterHits[i].point + enterHits[i].normal * -.019f;
                        bulletHole.transform.rotation = Quaternion.LookRotation(-enterHits[i].normal);
                        
                        bulletHole.Activate(hittable);
                        
                        GameSystem.PreSpawnedBulletHoles.Enqueue(bulletHole);
                    }
                }

                // Debug.Log($"{damage} * {dropoff} * {hitPower} = {damage * dropoff * hitPower}");
                hittable.Hit(new HitInfo
                {
                    Damage = damage * dropoff * hitPower,
                    Point = enterHits[i].point,
                    HeadshotMultiplier = headShotMultiplier,
                    // Force = force * power,
                    Direction = Camera.transform.forward
                }, this);

                
                if (availablePower <= 0f) break;

                //Spawn exit hole
                if (!isPlayer)
                {
                    bool spawnHole = true;
                    if (enterHits.Count > i + 1)
                        spawnHole = enterHits[i + 1].distance - (200 - exitHits[i].distance) > noHoleThreshold;

                    if (spawnHole)
                    {
                        BulletHole bulletHole = GameSystem.PreSpawnedBulletHoles.Dequeue();

                        bulletHole.transform.position = exitHits[i].point + exitHits[i].normal * -.019f;
                        bulletHole.transform.rotation = Quaternion.LookRotation(-exitHits[i].normal);
                        
                        bulletHole.Activate(hittable);
                        
                        GameSystem.PreSpawnedBulletHoles.Enqueue(bulletHole);
                    }
                }
                
            }
        }
        #endregion
    }

    [ClientRpc]
    private void RpcClientShoot()
    {
        if(!hasAuthority)
            PlayShotSound();
    }
    
    private void TryReload()
    {
        if(isReloading || ammo == magazineSize || avalibleAmmo == 0) return;
        isReloading = true;
        actionStarted.Invoke(reloadTime);
        reloadTimer = reloadTime;
    }
    
    private Vector3 BulletSpread(float spread)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        // float spread = isAiming ? bulletSpread / aimAccuracyMultiplier : bulletSpread;
        // spread = aimAccuracyMultiplier == 0f && isAiming ? 0f : spread;
        Vector2 spreadVector = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * Random.Range(0f, spread);
        Vector3 dir = Quaternion.AngleAxis(spreadVector.x, Camera.transform.right) * Camera.transform.forward;
        dir = Quaternion.AngleAxis(spreadVector.y, Camera.transform.up) * dir;
        return dir;
    }

    private List<RaycastHit> enters = new List<RaycastHit>();
    private List<RaycastHit> exits = new List<RaycastHit>();
    private readonly float _currentSpread;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (RaycastHit enter in enters)
        {
            Gizmos.DrawSphere(enter.point, .05f);
        }
        Gizmos.color = Color.red;
        foreach (RaycastHit exit in exits)
        {
            Gizmos.DrawSphere(exit.point, .05f);
        }
    }

    private void PlayShotSound()
    {
        AudioSource audioSource = shotSources.Dequeue();
        audioSource.clip = shotEffect;
        audioSource.Play();
        shotSources.Enqueue(audioSource);
    }
}


public struct PenetrationInfo
{
    private Vector3 enterPoint;
    private Vector3 exitPoint;
    private float materialDensity;

    public float PenetrationValue => materialDensity * Vector3.Distance(enterPoint, exitPoint);

    public Vector3 EnterPoint
    {
        //get => enterPoint;
        set => enterPoint = value;
    }
    public Vector3 ExitPoint
    {
        get => exitPoint;
        set => exitPoint = value;
    }

    //public float Distance => Vector3.Distance(enterPoint, exitPoint);

    public float MaterialDensity
    {
        set => materialDensity = value;
    }
}

public struct HitInfo
{
    private float damage;
    // private float force;
    private Vector3 direction;

    public Vector3 Point { get; set; }
    // public float Force { set => force = value; }
    // public Vector3 VectorForce => direction * force;
    public float Damage { get; set; }

    public Vector3 Direction
    {
        set => direction = value;
    }

    public float HeadshotMultiplier { get; set; }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Gun), true)]
public class GunEditor : NetworkBehaviourInspector
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!serializedObject.FindProperty("canAim").boolValue)
            DrawPropertiesExcluding(serializedObject, "aimAccuracyMultiplier");
        else
            DrawDefaultInspector();


        serializedObject.ApplyModifiedProperties();
        
        DrawNetworking();
    }
}
#endif