using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayerInput : NetworkBehaviour
{
    [SerializeField]
    private InputActionReference m_movementReference;
    public Vector2 MovementInput { get; private set; }
    public event Action OnPickUpPressed;
    public event Action OnInteractPressed;

    private Vector2 m_rawInput;
    [SerializeField]
    private float m_smoothTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if(IsOwner == false)
        {
            return;
        }
        m_rawInput = m_movementReference.action.ReadValue<Vector2>();
        MovementInput = Vector2.MoveTowards
            (MovementInput, m_rawInput, Time.deltaTime / m_smoothTime);
        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnPickUpPressed?.Invoke();
        }
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnInteractPressed?.Invoke();
        }
    }
}
