using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MirrorProject.TestSceneTwo;
using Unity.Mathematics;
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
    // [SerializeField] protected float recoilAmount = 10f;
    [SerializeField][Tooltip("Spread angle in one direction")] protected float bulletSpread = 1.7f;
    [SerializeField][Tooltip("Higher is more accurate, 1 is the same")] protected float aimAccuracyMultiplier = 10f;
    [Header("Gun Other")]
    [SerializeField] private Transform exitPoint = null;

    [SerializeField] private GameObject hitObject = null;
    
    private float nextFire;
    private int ammo;
    private float reloadTimer = 0f;
    private bool isReloading = false;
    private LayerMask hitMask;
    
    public int Ammo
    {
        get => ammo;
        set
        {
            ammo = value;
            amountChanged.Invoke(ammo);
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
        _playerInput.ItemInteractions.PrimaryAction.performed += Inpt_PrimaryActionPerformed;
        _playerInput.ItemInteractions.PrimaryAction.canceled += Inpt_PrimaryActionCancled;
        _playerInput.ItemInteractions.Reload.performed += Inpt_ReloadPerformed;
    }
    void Inpt_PrimaryActionPerformed(InputAction.CallbackContext ctx)
    {
        if(isAutomatic)
            shooting = true;
        else if (Ammo > 0 && !isReloading && nextFire <= 0f) Shoot();
    }
    void Inpt_PrimaryActionCancled(InputAction.CallbackContext ctx)
    {
        shooting = false;
    }
    void Inpt_ReloadPerformed(InputAction.CallbackContext ctx)
    {
        if(isReloading || ammo == magazineSize || avalibleAmmo == 0) return;
        isReloading = true;
        actionStarted.Invoke(reloadTime);
        reloadTimer = reloadTime;
    }
    public override void RemoveInput()
    {
        base.RemoveInput();
        // return;
        print("Removed inputs for gun: " + name);
        _playerInput.ItemInteractions.PrimaryAction.performed -= Inpt_PrimaryActionPerformed;
        _playerInput.ItemInteractions.PrimaryAction.canceled -= Inpt_PrimaryActionCancled;
        _playerInput.ItemInteractions.Reload.performed -= Inpt_ReloadPerformed;
    }

    #endregion

    private void Start()
    {
        Ammo = magazineSize;
        hitMask = LayerMask.GetMask("Ragdoll", "Default");
    }

    protected override void Update()
    {
        // print(nameof(hasAuthority) + " : " + hasAuthority);
        base.Update();
        if (reloadTimer > 0f)
            reloadTimer -= Time.deltaTime;
        else if(isReloading)
        {
            isReloading = false;
            int additionalAmmo = Math.Min(magazineSize - Ammo, avalibleAmmo);
            Ammo += additionalAmmo;
            avalibleAmmo -= additionalAmmo;
            actionEnded.Invoke();
        }

        if (nextFire > 0f)
            nextFire -= Time.deltaTime;
        else if (shooting && Ammo > 0 && !isReloading)
            Shoot();

        //shot = isAutomatic && shot;
    }
    
    private void Shoot()
    {
        nextFire = fireRate;
        Ammo -= 1;
        CmdShoot();
    }

    [Command]
    private void CmdShoot()
    {
        Vector3 shotDir = BulletSpread();
        BulletData bulletData = GameSystem.EventSingleton.bulletData;
        
        #region Forward Cast
        List<RaycastHit> enterHits = Physics.RaycastAll(Camera.transform.position + shotDir * .2f, shotDir, range, hitMask).ToList();
        enterHits = enterHits.OrderBy(hit => hit.distance).ToList();
        #endregion
        
        if(enterHits.Count == 0) return;

        #region Exit Cast

        #region ExitCastInfo

        bool rangeInCollider =
            Physics.CheckSphere(Camera.transform.position + shotDir * (range + .2f), .1f, hitMask);
        
        Vector3 reverseCastPoint =
            !rangeInCollider
                ? Camera.transform.position + shotDir * (range + .2f)
                : enterHits[enterHits.Count - 1].point;
        
        float reverseCastDistance = Vector3.Distance(Camera.transform.position + shotDir * .2f, reverseCastPoint);

        #endregion
        List<RaycastHit> exitHits = Physics.RaycastAll(reverseCastPoint, -shotDir, reverseCastDistance, hitMask).ToList();
        exitHits = exitHits.OrderBy(hit => hit.distance).ToList();
        exitHits.Reverse();
        #endregion
        

        Debug.Log($"Enter hits: {enterHits.Count}, Exit hits: {exitHits.Count}");
        enters = enterHits;
        exits = exitHits;
        
        #region ApplyHit

        float avaliblePower = 1f;
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
                                }.PenetrationValue / bulletPenetration, avaliblePower)
                    : avaliblePower;
                avaliblePower -= hitPower;

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
                    GameObject bulletHole = Instantiate(bulletData.decalProjector,
                        enterHits[i].point + enterHits[i].normal * -.0003f,
                        Quaternion.LookRotation(-enterHits[i].normal)).gameObject;
                    NetworkServer.Spawn(bulletHole);
                }

                Debug.Log($"{damage} * {dropoff} * {hitPower} = {damage * dropoff * hitPower}");
                hittable.Hit(new HitInfo
                {
                    Damage = damage * dropoff * hitPower,
                    Point = enterHits[i].point,
                    HeadshotMultiplier = headShotMultiplier,
                    // Force = force * power,
                    Direction = Camera.transform.forward
                });
                if (avaliblePower <= 0f) break;

                //Spawn exit hole
                if (!isPlayer)
                {
                    GameObject bulletHole = Instantiate(bulletData.decalProjector,
                        exitHits[i].point + exitHits[i].normal * -.0003f,
                        Quaternion.LookRotation(-exitHits[i].normal)).gameObject;
                    NetworkServer.Spawn(bulletHole);
                }
                
            }
        }
        #endregion

        return;
        
        print("shot");
        bool spawnHole = true;

        if (Physics.Raycast(Camera.transform.position + Camera.transform.forward * .2f, shotDir, out RaycastHit hitPoint, range, hitMask))
        {
            if(!hitPoint.collider.TryGetComponent(out Destroyable hittable))
                hittable = hitPoint.collider.GetComponentInParent<Destroyable>();
            
            if (hittable != null)
            {
                spawnHole = !(hittable is PlayerState);


                float calculatedDamage = hitPoint.distance > startDropoff ? damage - (hitPoint.distance - startDropoff) / (range - startDropoff) * (damage - minDamage) : damage;
                // calculatedDamage = hitPoint.collider.CompareTag("Head") ? calculatedDamage * headShotMultiplier : calculatedDamage;

                hittable.Hit(new HitInfo(){Damage = calculatedDamage, Direction = shotDir, Point = hitPoint.point});
            }
            else
            {
                print($"hit {hitPoint.collider.name} isn't of type {nameof(Destroyable)}");
            }

            ////////////////////////

            if (spawnHole)
            {
                GameObject bulletHole = Instantiate(bulletData.decalProjector, hitPoint.point + hitPoint.normal * -.0003f, Quaternion.LookRotation(-hitPoint.normal)).gameObject;
                NetworkServer.Spawn(bulletHole);
            }

            #region Commented

            // GameObject hitObj = Instantiate(hitObject, hitPoint.point, Quaternion.identity);
            // // hitObj.transform.SetParent(hitPoint.transform, true);
            // NetworkServer.Spawn(hitObj);

            /*float color = hitPoint.distance > startDropoff ? damage - (((hitPoint.distance - startDropoff) / (range - startDropoff)) * damage) : damage;*/
            
            //hitObj.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.yellow, Color.red, color / damage);
            // hitObj.GetComponent<SelfDestroy>().destructionTime = markTimer;

            #endregion
        }
    }
    
    private Vector3 BulletSpread()
    {
        float spread = bulletSpread;
        // float spread = isAiming ? bulletSpread / aimAccuracyMultiplier : bulletSpread;
        // spread = aimAccuracyMultiplier == 0f && isAiming ? 0f : spread;
        Vector3 dir = Quaternion.AngleAxis(Random.Range(-spread, spread), Camera.transform.right) * Camera.transform.forward;
        dir = Quaternion.AngleAxis(Random.Range(-spread, spread), Camera.transform.up) * dir;
        return dir;
    }

    private List<RaycastHit> enters = new List<RaycastHit>();
    private List<RaycastHit> exits = new List<RaycastHit>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (RaycastHit enter in enters)
        {
            Gizmos.DrawSphere(enter.point, .1f);
        }
        Gizmos.color = Color.red;
        foreach (RaycastHit exit in exits)
        {
            Gizmos.DrawSphere(exit.point, .1f);
        }
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