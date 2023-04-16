using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour
{
    [SerializeField] private bool debugMode;
    [SerializeField] private List<Weapon> allWeapon;
    //[SerializeField] private GameEvent _onOnAmmoChanged;
    //[SerializeField] private GameEvent _onReloadStart;
    //[SerializeField] private GameEvent _onReloading;
    //[SerializeField] private GameEvent _onReloadEnd;
    [SerializeField] private LayerMask detectMask;
    [SerializeField] private Transform gunSocket;

    [Viewable] [SerializeField] private Weapon currentWeapon;

    private void OnEnable()
    {
        currentWeapon = allWeapon[0];
    }

    void Update()
    {
        if (debugMode) DebugMouse();

        currentWeapon.SetAim(Camera.main.transform.forward * 200.0f);
        if (InputManager.Instance.HandleFireInput().IsPressed()) 
            currentWeapon.Shoot(GetTargetPos(currentWeapon.Range), OnFireSuccess);
    }

    public void OnFireSuccess()
    {
        AudioManager.PlaySoundEffect(currentWeapon.FireSoundID, true);
    }

    void DebugMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;  else if (Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;
    }

    public Vector3 GetTargetPos(float range)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if (Physics.Linecast(Camera.main.transform.position, gunSocket.position, detectMask))
        {
            Debug.DrawLine(Camera.main.transform.position, gunSocket.position, Color.green);
            return ray.GetPoint(range);
        } 
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, range, detectMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.magenta);
                return hit.point;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.magenta);
                return ray.GetPoint(range);
            }
        }
    }
}
