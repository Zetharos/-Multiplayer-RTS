using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds;
    [SerializeField] private float launchForce = 10f;

    void Start()
    {
        rigidBody.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback] private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity)) 
        {
            if(networkIdentity.connectionToClient == connectionToClient) { return; } //If trigger occurs between the same player's units, don't do anything.
        }

        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    [Server] private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
