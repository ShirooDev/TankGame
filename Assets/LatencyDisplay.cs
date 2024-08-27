using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LatencyDisplay : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI latencyText;

    private float timeBetweenPings = 1.0f; // Interval in seconds to update ping
    private float nextPingTime;

    private void Update()
    {
        if (IsClient && !IsServer && Time.time >= nextPingTime)
        {
            // Schedule the next ping update
            nextPingTime = Time.time + timeBetweenPings;

            // Send a ping to the server
            PingServerRpc(Time.time);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PingServerRpc(float clientSendTime, ServerRpcParams rpcParams = default)
    {
        // Send the ping back to the client with the current server time
        PongClientRpc(clientSendTime, Time.time, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void PongClientRpc(float clientSendTime, float serverReceiveTime, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Calculate the latency in milliseconds
            float latency = (Time.time - clientSendTime) * 1000f;
            latencyText.text = $"Latency: {latency:F1} ms";
        }
    }
}
