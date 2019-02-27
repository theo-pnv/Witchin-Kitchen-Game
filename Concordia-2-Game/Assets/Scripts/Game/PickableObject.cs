﻿using UnityEngine;

public class PickableObject : MonoBehaviour
{
    // The object's rigidbody
    private Rigidbody m_rb;
    public float m_maxSpeedFractionWhenHolding = .85f;
    public Ingredient m_ingredientType = Ingredient.NOT_AN_INGREDIENT;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    // Called by the carrying player's Update() to force the object to follow it
    public void UpdatePosition(Vector3 currentVel)
    {
        transform.position = transform.parent.position;
        m_rb.velocity = currentVel/m_rb.mass;
        Debug.Log(m_rb.velocity);
    }

    // Get picked up
    public void PickUp(Transform newParent)
    {
        // Reset rotation
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.parent = newParent;

        transform.position = newParent.position;

        // Disable the use of gravity, remove the velocity, and freeze rotation (will all be driven by player movement)
        m_rb.useGravity = false;
        m_rb.isKinematic = false;
        m_rb.velocity = Vector3.zero;
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    //Get dropped
    public void Drop(Vector3 throwVector)
    {
        // Re-Enable the use of gravity on the object and remove all constraints
        m_rb.useGravity = true;
        m_rb.isKinematic = true;
        m_rb.constraints = RigidbodyConstraints.None;

        // Get thrown forward
        m_rb.AddForce(throwVector, ForceMode.Impulse);

        // Unparent the object from the player
        transform.parent = null;
    }

    public float GetMaxSpeedFractionWhenHolding()
    {
        return m_maxSpeedFractionWhenHolding;
    }

}
