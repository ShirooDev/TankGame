using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.FireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.FireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool isFiring)
    {
        if (!isFiring) return;

        // Request the server to spawn the authoritative projectile
        RequestProjectileSpawnServerRpc();
    }

    [ServerRpc]
    private void RequestProjectileSpawnServerRpc(ServerRpcParams rpcParams = default)
    {
        // Server spawns the authoritative networked projectile
        var projectileInstance = Instantiate(serverProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        Rigidbody2D rb = projectileInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = projectileSpawnPoint.up * projectileSpeed;
        }

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out var dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        // Disable Interpolation for fast projectiles
        var networkTransform = projectileInstance.GetComponent<NetworkTransform>();
     

        // Assign ownership to the client who requested it
        projectileInstance.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }


    private void Update()
    {
        // Any additional update logic here, if necessary
    }
}
