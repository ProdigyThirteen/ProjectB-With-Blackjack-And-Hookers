using System;
using System.Collections;
using System.Collections.Generic;
using JE.General;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private UnityEvent _onDeliver;

    private void Start()
    {
        if(PackageTracker.Instance)
        {
            PackageTracker.Instance.DeliveryPoints.Add(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider objectCollider)
    {
        if (objectCollider.TryGetComponent(out PackageSystem pS))
        {
            if (pS.PackageAmount < 1) return;

            _onDeliver?.Invoke();
            pS.DeliverPackages();

            return;
        }

        GameObject baseObject = objectCollider.gameObject.transform.root.gameObject;
        if (!baseObject.TryGetComponent(out PackageSystem packageSystem)) return;
        if (packageSystem.PackageAmount < 1) return;
        
        _onDeliver?.Invoke();
        packageSystem.DeliverPackages();
    }
}
