using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> FireEvent;
    public event Action<string> SendEvent; // Use string to send specific messages

    public Vector2 AimPosition { get; private set; }
    private Controls controls;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            FireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }

    public void OnSend(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Determine which key was pressed
            string keyPressed = context.control.displayName.ToLower();

            if (keyPressed == "enter")
            {
                SendEvent?.Invoke("Hello");
            }
            else if (keyPressed == "1")
            {
                SendEvent?.Invoke("gg");
            }
            else if (keyPressed == "2")
            {
                SendEvent?.Invoke("lol");
            }
        }
    }
}
