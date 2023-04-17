using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public enum WeaponState { Idle, Firing, Reloading }

    [Header("Content")]
    [SerializeField] protected Transform content;

    [Header("Rotation settings")]
    [SerializeField] protected bool rotateVertical;
    [Range(0, 180)]
    [SerializeField] protected float rightRotationLimit = 180f;
    [Range(0, 180)]
    [SerializeField] protected float leftRotationLimit = 180f;
    [Range(0, 180)]
    [SerializeField] protected float upRotationLimit = 180f;
    [Range(0, 180)]
    [SerializeField] protected float downRotationLimit = 180f;
    [Range(0, 300)]
    [SerializeField] protected float turnSpeed = 300f;

    [Header("Shooting")]
    [SerializeField] protected string fireSoundID;
    [SerializeField] protected WeaponSO weaponSO;
    [SerializeField] protected Transform[] firePoint;

    [SerializeField][SyncVar] protected int currentAmmo = 0;
    [SerializeField] protected WeaponState weaponState;
    [SerializeField][SyncVar] protected float timer = 0;

    protected Vector3 aimPoint;
    public void SetAim(Vector3 AimPosition) => aimPoint = AimPosition;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponSO.maxAmmo;
    public float Range => weaponSO.range;


    public string FireSoundID =>fireSoundID;

    //public override void OnStartServer();
    //{        
    //    base.OnStartServer();

    //    Debug.Log("Start called");
    //    ModifyAmmo(weaponSO.maxAmmo);
    //    timer = weaponSO.fireRate;
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("Start called");
        ModifyAmmo(weaponSO.maxAmmo);
        timer = weaponSO.fireRate;
    }

    protected virtual void Update()
    {
        if (!IsOwner)
            return;

        HandleRotation();

        timer -= Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 targetPositionInLocalSpace = aimPoint;
        
        targetPositionInLocalSpace.y = rotateVertical ? Mathf.Clamp(targetPositionInLocalSpace.y, -upRotationLimit, downRotationLimit) : 0;
        targetPositionInLocalSpace.x = Mathf.Clamp(targetPositionInLocalSpace.x, -leftRotationLimit, rightRotationLimit);

        Vector3 limitedRotation = Vector3.RotateTowards(Vector3.forward, targetPositionInLocalSpace, float.MaxValue, float.MaxValue);
        Quaternion whereToRotate = Quaternion.LookRotation(limitedRotation);
        
        content.rotation = Quaternion.RotateTowards(content.rotation, whereToRotate, turnSpeed * Time.deltaTime);
    }

    [ServerRpc]
    public void Shoot(Vector3 targetPos)
    {
        if (timer <= 0 && currentAmmo > 0)
        {
            Debug.Log("Shooting");
            ShootProjectile(targetPos);
            ModifyAmmo(-1);
            timer = weaponSO.fireRate;
        }
        else 
        {
            Debug.Log("Timer: " + timer + " Current Ammo: " + currentAmmo);
        }
    }

    private void ShootProjectile(Vector3 targetPos)
    {
        //if (currentAmmo > 0)
        //    weaponState = WeaponState.Firing;

        foreach (Transform point in firePoint)
        {
            Projectile projectile = Instantiate(weaponSO.projectile, point.position, point.rotation);
            projectile.GetComponent<DartProjectile>().SetUp(weaponSO.projectileSpeed);
            ServerManager.Spawn(projectile.gameObject, Owner);
            //projectile.SetTarget(targetPos);
        }
    }

    [ServerRpc (RequireOwnership = true)]
    private void ModifyAmmo(int newValue)
    {
        Debug.Log("Modifying ammo by " + newValue);
        currentAmmo += newValue;
        currentAmmo = Mathf.Clamp(currentAmmo, 0, weaponSO.maxAmmo);
    }
}
