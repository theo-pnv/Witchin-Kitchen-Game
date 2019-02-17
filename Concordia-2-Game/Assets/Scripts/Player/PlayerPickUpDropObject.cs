﻿using con2;
using UnityEngine;

public class PlayerPickUpDropObject : MonoBehaviour, IInputConsumer
{
    public Transform m_characterHands;
    public Transform m_characterFeet;
    public float m_throwForce = 5;
    public float m_movementReduction = 1.5f;
    private float m_playerHeight;
    private Rigidbody m_playerRB;
    private PlayerMovement m_playerMovement;

    // The actual pickable object
    private PickableObject m_PickableObject;

    void Start()
    {
        m_playerHeight = GetComponent<Collider>().bounds.size.y;
        m_playerRB = GetComponent<Rigidbody>();
        m_playerMovement = GetComponent<PlayerMovement>();
    }

    public bool ConsumeInput(GamepadAction input)
    {
        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.PUNCH))
        {
            if (IsHoldingObject())
                return true;
        }

        if (input.GetActionID().Equals(con2.GamepadAction.ButtonID.INTERACT))
        {
            if (IsHoldingObject())
                DropObject();
            else if (GetNearestItem())
                PickUpObject();
            else
                return false;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (IsHoldingObject())
        {
            // Keeps the object in hands at the same position and orientation
            m_PickableObject.UpdatePosition(m_characterHands.localPosition);
        }
    }

    private bool GetNearestItem()
    {
        RaycastHit rayItemHit;

        // Check if an pickable object is in range
        if (Physics.Raycast(transform.position - new Vector3(0, m_playerHeight / 2.5f, 0), transform.TransformDirection(Vector3.forward), out rayItemHit, 1f))
        {
            m_PickableObject = rayItemHit.transform.gameObject.GetComponent<PickableObject>();
        }
        return m_PickableObject;
    }

    // Pick up a nearby object
    private void PickUpObject()
    {
        // Slow down the player
        m_playerMovement.MaxMovementSpeed -= m_movementReduction;

        // Have the object adjust its physics
        m_PickableObject.PickUp(transform);

        // Reposition the player hands (location)
        //mCharacterHands.localPosition = new Vector3(0.0f, playerSize.y + objectSize.y / 2.0f, 0.0f);
    }

    // Drop the object in hands
    private void DropObject()
    {
        // Restore max movement speed
        m_playerMovement.MaxMovementSpeed += m_movementReduction;

        // Have the object adjust its physics and get thrown
        m_PickableObject.Drop(m_playerRB.velocity + (transform.forward*m_throwForce));

        // Reset picked up object
        m_PickableObject = null;
    }

    // Get the value of mIsHoldingObject
    public bool IsHoldingObject()
    {
        return m_PickableObject;
    }
}
