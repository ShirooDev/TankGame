using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;

    // Reference to the InputReader ScriptableObject
    [SerializeField] private InputReader inputReader;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        // Get the AimPosition from the InputReader
        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        // Rotate the turret towards the aiming position
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y);
    }
}
