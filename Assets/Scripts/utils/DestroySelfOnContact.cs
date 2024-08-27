using Unity.Netcode;
using UnityEngine;

public class DestroySelfOnContact : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (IsServer) // Ensure only the server can trigger the destruction
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
            Debug.LogWarning("Attempted to despawn a projectile that is not fully spawned.");
            Destroy(gameObject); // Fallback to standard destroy if no NetworkObject
        }
    }
}
