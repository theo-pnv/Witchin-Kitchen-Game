﻿using con2;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDropObject : MonoBehaviour, IInputConsumer, IPunchable
{
    public Transform m_characterHands;
    public float m_throwForce = 5;
    private Rigidbody m_playerRB;
    private PlayerMovement m_playerMovement;

    // The actual pickable object
    private PickableObject m_heldObject;

    //Objects in range to be grabbed
    private List<KeyValuePair<GameObject, PickableObject>> m_nearbyObjects;

    void Start()
    {
        m_playerRB = GetComponent<Rigidbody>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_nearbyObjects = new List<KeyValuePair<GameObject, PickableObject>>();
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
                DropObject(m_playerRB.velocity + (transform.forward * m_throwForce));
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
            m_heldObject.UpdatePosition(m_playerRB.velocity);
        }
    }

    private bool GetNearestItem()
    {
        Vector3 playerPosition = transform.position;
        float closestObject = Vector3.negativeInfinity.magnitude;
        foreach (KeyValuePair<GameObject, PickableObject> someNearbyObject in m_nearbyObjects)
        {
            float distanceFromPlayer = (someNearbyObject.Key.transform.position - playerPosition).magnitude;
            if (distanceFromPlayer < closestObject)
            {
                closestObject = distanceFromPlayer;
                m_heldObject = someNearbyObject.Value;
            }
        }
        return m_heldObject;
    }

    // Pick up a nearby object
    private void PickUpObject()
    {
        // Slow down the player
        m_playerMovement.MaxMovementSpeed *= m_heldObject.GetMaxSpeedFractionWhenHolding();

        // Have the object adjust its physics
        m_heldObject.PickUp(m_characterHands);

        // Reposition the player hands (location)
        //mCharacterHands.localPosition = new Vector3(0.0f, playerSize.y + objectSize.y / 2.0f, 0.0f);
    }

    // Drop the object in hands
    private void DropObject(Vector3 throwVector)
    {
        // Restore max movement speed
        m_playerMovement.MaxMovementSpeed /= m_heldObject.GetMaxSpeedFractionWhenHolding();

        // Have the object adjust its physics and get thrown
        m_heldObject.Drop(throwVector);

        // Reset picked up object
        m_heldObject = null;
    }

    public void Punch(Vector3 knockVelocity, float stunTime)
    {
        if (IsHoldingObject())
        {
            Vector3 knockVector = -knockVelocity.normalized;
            DropObject(new Vector3(knockVector.x, knockVector.z) * m_throwForce);
        }
    }

    // Get the value of mIsHoldingObject
    public bool IsHoldingObject()
    {
        return m_heldObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        PickableObject pickable = other.gameObject.GetComponent<PickableObject>();
        if (pickable)
        {
            m_nearbyObjects.Add(new KeyValuePair<GameObject, PickableObject>(other.gameObject, pickable));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        KeyValuePair<GameObject, PickableObject> leavingObject = m_nearbyObjects.Find(item => item.Key.Equals(other.gameObject));
        m_nearbyObjects.Remove(leavingObject);
    }
}