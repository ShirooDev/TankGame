using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DealDamageOnContact : NetworkBehaviour
{
    [SerializeField] private int damage = 5;
    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) return;

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            // Avoid self-damage
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }

        // Ensure only the server handles the destruction
        if (IsServer)
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn(); // Despawn the network object on the server
        }
        else
        {
            Destroy(gameObject); // Fallback to standard destroy if no NetworkObject
        }
    }
}
