using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class Chat : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextMeshProUGUI text;

    private static int playerCounter = 1;
    private string playerName;

    public override void OnNetworkSpawn()
    {
        GameObject chatTextObject = GameObject.Find("ChatTxt");
        if (chatTextObject != null)
        {
            text = chatTextObject.GetComponent<TextMeshProUGUI>();
        }

        AssignPlayerName();

        if (IsOwner)
        {
            UpdatePlayerNameServerRpc(playerName);
        }

        if (inputReader != null)
        {
            inputReader.SendEvent += OnSend;
        }
    }

    private void AssignPlayerName()
    {
        Debug.Log($"AssignPlayerName called for Client ID: {NetworkManager.Singleton.LocalClientId}");

        if (IsHost && IsOwner)
        {
            playerName = "Player 1 (Host)";
        }
        else
        {
            // Ensure each client gets a unique player number starting from 2.
            // This assumes playerCounter starts at 2 for the first client.
            int clientNumber = (int)(NetworkManager.Singleton.LocalClientId + 1);
            playerName = $"Player {clientNumber}";
        }

        Debug.Log($"Player name assigned: {playerName}");
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerNameServerRpc(string name)
    {
        // Increase the player counter only on the server
        playerCounter++;

        // Synchronize the player name across all clients
        UpdatePlayerNameClientRpc(name);
    }

    [ClientRpc]
    private void UpdatePlayerNameClientRpc(string name)
    {
        playerName = name;
        Debug.Log($"Player name synchronized: {playerName}");
    }

    private void OnSend(string message)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is not set, cannot send message.");
            return;
        }

        FixedString128Bytes fullMessage = $"{playerName}: {message}";
        SubmitMessageServerRpc(fullMessage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitMessageServerRpc(FixedString128Bytes message)
    {
        UpdateMessageClientRpc(message);
        Debug.Log("Message Sent: " + message);
    }

    [ClientRpc]
    public void UpdateMessageClientRpc(FixedString128Bytes message)
    {
        text.text += "\n" + message.ToString();
        Debug.Log("Message Received: " + message);

        StartCoroutine(RemoveMessageAfterDelay(message.ToString(), 3f));
    }

    private IEnumerator RemoveMessageAfterDelay(string message, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (text.text.Contains(message))
        {
            text.text = text.text.Replace($"\n{message}", "");
        }
    }

    private void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.SendEvent -= OnSend;
        }
    }
}
