using UnityEngine;
using Unity.Netcode;

public class LifeTime : NetworkBehaviour
{
    [SerializeField] private float lifetime = 1f;

    private void Start()
    {
        if (IsServer) // Only the server should handle the destruction
        {
            Invoke(nameof(DestroyProjectile), lifetime);
        }
    }

    private void DestroyProjectile()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(); // Properly despawn the network object on the server
        }
        else
        {
            Destroy(gameObject); // Fallback to standard destroy if no NetworkObject
        }
    }
}
