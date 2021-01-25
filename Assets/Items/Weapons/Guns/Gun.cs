using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
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
    [SerializeField] protected float range = 200f;
    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float minDamage = 8f;
    [SerializeField] protected float headShotMultiplier = 1.5f;
    [SerializeField] protected float hitForce = 8f;
    [SerializeField] private int magazineSize = 40;
    [SerializeField] protected float fireRate = 0.1f; //DONE
    [SerializeField] private float reloadTime = 2f;
    // [SerializeField] protected float recoilAmount = 10f;
    [SerializeField][Tooltip("Spread angle in one direction")] protected float bulletSpread = 1.7f;
    [SerializeField][Tooltip("Higher is more accurate, 1 is the same")] protected float aimAccuracyMultiplier = 10f;
    [SerializeField] [Tooltip("Range at wich damage starts dropping")] protected float startDropoff = 100f;
    [Header("Gun Other")]
    [SerializeField] protected LayerMask hitMask = 0;

    [SerializeField] private GameObject hitObject = null;
    
    private float nextFire;
    private int ammo;
    private float reloadTimer = 0f;
    private bool isReloading = false;
    
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
        if(isReloading || ammo == magazineSize) return;
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
    }

    protected override void Update()
    {
        print(nameof(hasAuthority) + " : " + hasAuthority);
        base.Update();
        if (reloadTimer > 0f)
            reloadTimer -= Time.deltaTime;
        else if(isReloading)
        {
            isReloading = false;
            Ammo = magazineSize;
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
        print("shot");
        Vector3 shotDir = BulletSpread();

        RaycastHit hitPoint;
        if (Physics.Raycast(Camera.transform.position + Camera.transform.forward * .2f, shotDir, out hitPoint, range, hitMask))
        {
            if(!hitPoint.collider.TryGetComponent(out Hittable hittable))
                hittable = hitPoint.collider.GetComponentInParent<Hittable>();
            
            if (hittable != null)
            {
                // VisualEffect vfx = Instantiate(hitParticle, hitPoint.point, Quaternion.LookRotation(hitPoint.normal)).GetComponentInChildren<VisualEffect>();

                // vfx.SetGradient("Particle Color", hittable.ParticleGradient);


                float calculatedDamage = hitPoint.distance > startDropoff ? damage - (hitPoint.distance - startDropoff) / (range - startDropoff) * (damage - minDamage) : damage;
                // calculatedDamage = hitPoint.collider.CompareTag("Head") ? calculatedDamage * headShotMultiplier : calculatedDamage;

                //print("Bullet distance: " + hitPoint.distance);
                //print("Bullet damage: " + calculatedDamage);

                hittable.Hit(calculatedDamage);
            }
            else
            {
                print($"hit {hitPoint.collider.name} isn't of type {nameof(Hittable)}");
            }

            ////////////////////////
            /// 

            GameObject hitObj = Instantiate(hitObject, hitPoint.point, Quaternion.identity);
            // hitObj.transform.SetParent(hitPoint.transform, true);
            NetworkServer.Spawn(hitObj);

            /*float color = hitPoint.distance > startDropoff ? damage - (((hitPoint.distance - startDropoff) / (range - startDropoff)) * damage) : damage;*/
            
            //hitObj.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.yellow, Color.red, color / damage);
            // hitObj.GetComponent<SelfDestroy>().destructionTime = markTimer;
        }
        else
        {
            print("raycast has missed");
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
}