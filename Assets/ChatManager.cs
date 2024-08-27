using UnityEngine;
using Unity.Netcode;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private GameObject chatPrefab; // Reference to the Chat prefab

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SpawnChatObject();
        }
    }

    private void SpawnChatObject()
    {
        // Only spawn on the server to ensure proper ownership
        if (IsServer)
        {
            Debug.Log("Spawning Chat Object");

            // Instantiate the Chat object from the prefab
            GameObject chatInstance = Instantiate(chatPrefab);

            if (chatInstance == null)
            {
                Debug.LogError("Failed to instantiate Chat prefab.");
                return;
            }

            // Get the NetworkObject component from the instantiated Chat object
            NetworkObject networkObject = chatInstance.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError("NetworkObject component missing on Chat prefab.");
                return;
            }

            // Spawn the object with ownership assigned to the client that owns this instance
            networkObject.SpawnWithOwnership(OwnerClientId);
            Debug.Log("Chat Object Spawned with Ownership");
        }
    }

}
