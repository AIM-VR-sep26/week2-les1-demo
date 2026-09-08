using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class PlayerMover : MonoBehaviour
{
    [SerializeField] private float speed = 0f;

    private Rigidbody body;
    private bool slidingToHalt;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }
    
    
    private void FixedUpdate()
    {
        Vector2 input = ReadInput();
        Vector3 horizontalVelocity = new Vector3(input.x, 0, input.y);
        body.linearVelocity = new Vector3(horizontalVelocity.x, 0, horizontalVelocity.z) * speed;
    }

    private static Vector2 ReadInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        return input;
    }
}
