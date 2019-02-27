﻿using con2;
using con2.game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IInputConsumer, IPunchable
{
    [Range(0.0f, 300.0f)]
    public float MovementSpeed;

    [Range(0.0f, 50.0f)]
    public float MaxMovementSpeed;

    [Range(0.0f, 1.0f)]
    public float MovementRotationSpeed;

    [Range(0.0f, 50.0f)]
    public float Gravity;

    [Range(0.0f, 1.0f)]
    public float FacingRotationSpeed;

    private Vector3 movementDirection = new Vector3();
    FightStun m_stun;
    Rigidbody m_rb;

    void Start()
    {
        m_stun = GetComponent<FightStun>();
        m_rb = GetComponent<Rigidbody>();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.MAX_ID)) //joystick movement
        {
            // Movement
            Vector2 joystick = input.m_movementDirection;
            movementDirection.x = joystick.x;
            movementDirection.y = 0.0f;
            movementDirection.z = joystick.y;
            
            if (movementDirection.sqrMagnitude > 1)
            {
                movementDirection.Normalize();
            }

            movementDirection *= MovementSpeed;
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        // Apply stun factor
        movementDirection *= m_stun.getMovementModifier();

        // If player asked for input
        if (!Mathf.Approximately(movementDirection.magnitude, 0.0f))
        {
            m_rb.AddForce(movementDirection * MovementSpeed * MovementRotationSpeed * Time.deltaTime, ForceMode.Acceleration);

            // Cap movement speed
            if (m_rb.velocity.magnitude > MaxMovementSpeed)
            {
                m_rb.velocity = Vector3.ClampMagnitude(m_rb.velocity, MaxMovementSpeed);
            }
        }

        // Gravity
        m_rb.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

        // Smooth rotation according to velocity
        bool nonZeroInput = movementDirection.magnitude > 0.1f;
        if (nonZeroInput)
        {
            Vector3 targetRotation = movementDirection.normalized;

            Quaternion facing = new Quaternion();
            facing.SetLookRotation(targetRotation);
            facing = Quaternion.Slerp(transform.rotation, facing, FacingRotationSpeed);

            transform.rotation = facing;
        }

        movementDirection.x = 0.0f;
        movementDirection.y = 0.0f;
        movementDirection.z = 0.0f;
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        m_rb.AddForce(-knockVelocity, ForceMode.Impulse);
        //m_rb.velocity = knockVelocity;
        m_stun.Stun(stunTime);
    }

    public void ModulateMovementDrag(float dragFraction)
    {
        m_rb.drag *= dragFraction;  
    }

    public void ModulateMovementSpeed(float movementModulator)
    {
        MaxMovementSpeed *= movementModulator;
        MovementSpeed *= movementModulator;
        FacingRotationSpeed /= movementModulator;
        MovementRotationSpeed /= movementModulator;
    }
}
