using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField][SyncVar] protected int currentAmmo;
    [SerializeField] protected WeaponState weaponState;
    [SerializeField] protected float timer = 0;

    protected Vector3 aimPoint;
    public void SetAim(Vector3 AimPosition) => aimPoint = AimPosition;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => weaponSO.maxAmmo;
    public float Range => weaponSO.range;


    public string FireSoundID =>fireSoundID;

    [ServerRpc]
    protected virtual void Start()
    {
        ModifyAmmo(weaponSO.maxAmmo);
    }

    protected virtual void Update()
    {
        HandleRotation();
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
    public void Shoot(Vector3 targetPos, Action onFireSuccess = null)
    {
        if (timer <= 0)
        {
            ShootProjectile(targetPos, onFireSuccess);
            ModifyAmmo(-1);
            timer = weaponSO.fireRate;
        }
        else timer -= Time.deltaTime;
    }

    public virtual void ShootProjectile(Vector3 targetPos, Action onFireSuccess = null)
    {
        if (currentAmmo > 0)
            weaponState = WeaponState.Firing;
    }

    [ServerRpc (RequireOwnership = true)]
    public void ModifyAmmo(int newValue)
    {
        currentAmmo += newValue;
    }
}
