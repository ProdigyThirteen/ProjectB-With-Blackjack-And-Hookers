using JE.DamageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGamePlayer : GamePlayer
{
    public override void HandleDeath(HealthSystem healthSystem)
    {
        if (!gameObject.TryGetComponent(out GamePlayer gamePlayer))
            return;

        transform.SetPositionAndRotation(gamePlayer.PlayerTeamData.GetRandomSpawnPoint(), Quaternion.identity);

        healthSystem.RestoreHealth(healthSystem.MaximumHealth);

        if (!gameObject.TryGetComponent(out Rigidbody currentBody))
            return;

        currentBody.velocity = Vector3.zero;
    }
}
