using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) { return; }

        // Automatically find the Health component on the parent
        if (health == null)
        {
            health = GetComponentInParent<Health>();
        }

        // Automatically find the Image component in the children
        if (healthBarImage == null)
        {
            healthBarImage = GetComponentInChildren<Image>();
        }

        // Check if the health reference was found
        if (health != null)
        {
            health.CurrentHealth.OnValueChanged += HandleHealthChanged;
            HandleHealthChanged(0, health.CurrentHealth.Value);
        }
        else
        {
            Debug.LogError("Health component not found on the parent.");
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient || health == null) { return; }
        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)newHealth / health.MaxHealth;
        }
        else
        {
            Debug.LogError("HealthBar Image component not found in children.");
        }
    }
}
