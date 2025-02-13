﻿using con2;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class PlayerPickUpDropObject : MonoBehaviour, IInputConsumer, IPunchable
    {
        public Transform m_characterHands;
        public float m_throwForce = 5;
        private Rigidbody m_playerRB;
        private PlayerMovement m_playerMovement;
        private AnimControl m_anim;

        // The actual pickable object
        private PickableObject m_heldObject;
        private float m_speedReduction;

        //Objects in range to be grabbed
        private List<KeyValuePair<GameObject, PickableObject>> m_nearbyObjects;

        //Player's cauldron
        private GameObject m_ownedCauldron;
        public const float AIM_ASSIST_THROW_DISTANCE = 4.0f, AIM_ASSIST_FACING_DEGREE = 30.0f;

        void Start()
        {
            m_playerRB = GetComponent<Rigidbody>();
            m_playerMovement = GetComponent<PlayerMovement>();
            m_anim = GetComponentInChildren<AnimControl>();
            m_nearbyObjects = new List<KeyValuePair<GameObject, PickableObject>>();
        }

        public bool ConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(con2.GamepadAction.ID.PUNCH)
                || input.GetActionID().Equals(con2.GamepadAction.ID.XBUTTON))
            {
                if (IsHoldingObject())
                    return true;
            }

            if (input.GetActionID().Equals(con2.GamepadAction.ID.INTERACT))
            {
                if (IsHoldingObject())
                    DropObject(m_playerRB.velocity*0.5f + m_throwForce*transform.forward, true);
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
            for (int i = 0; i < m_nearbyObjects.Count; i++)
            {
                KeyValuePair<GameObject, PickableObject> someNearbyObject = m_nearbyObjects[i];
                if (someNearbyObject.Key == null)
                {
                    m_nearbyObjects.Remove(someNearbyObject);
                    i--;
                }
                else
                {
                    float distanceFromPlayer = (someNearbyObject.Key.transform.position - playerPosition).magnitude;
                    if (distanceFromPlayer < closestObject)
                    {
                        closestObject = distanceFromPlayer;
                        m_heldObject = someNearbyObject.Value;
                    }
                }
            }

            return m_heldObject;
        }

        // Pick up a nearby object
        private void PickUpObject()
        {
            var tempHeld = m_heldObject;

            // Have the object adjust its physics, and tell us if it can be grabbed now
            if (m_heldObject.PickUp(m_characterHands))
            {
                if (tempHeld.Equals(m_heldObject))  // if we actually picked up the thing we were trying to pick up (fkn newt side effects)
                {
                    m_anim.Carry();

                    // Slow down the player
                    m_speedReduction = m_heldObject.GetMaxSpeedFractionWhenHolding();
                    m_playerMovement.MaxMovementSpeed *= m_speedReduction;
                }
            }
            else
            {
                m_heldObject = null;
            }
        }

        public void ForcePickUpObject(PickableObject obj)
        {
            m_heldObject = obj;
            PickUpObject();
        }

        public void UpdateHeldObjectWeight()
        {
            if (IsHoldingObject())
            {
                m_playerMovement.MaxMovementSpeed /= m_speedReduction;
                m_speedReduction = m_heldObject.GetMaxSpeedFractionWhenHolding();
                m_playerMovement.MaxMovementSpeed *= m_speedReduction;
            }
        }

        // Drop the object in hands
        private void DropObject(Vector3 throwVector, bool thrownOnPurpose)
        {
            m_anim.Drop();

            // Restore max movement speed
            m_playerMovement.MaxMovementSpeed /= m_speedReduction;

            m_heldObject.SetThrownIntentionally(thrownOnPurpose);

            if (CheckNearAimAssistedStation())
            {
                m_heldObject.AimAssistFly(m_ownedCauldron.transform.position, throwVector);
            }
            else
            {
                // Have the object adjust its physics and get thrown normally
                m_heldObject.Drop(throwVector);
            }

            // Reset picked up object
            m_heldObject = null;
            m_speedReduction = 1;
        }

        public void Punch(Vector3 knockVelocity, float stunTime)
        {
            if (IsHoldingObject())
            {
                m_heldObject.UpdatePosition(Vector3.zero);
                Vector3 knockVector = -knockVelocity.normalized;
                DropObject(new Vector3(knockVector.x, 0, knockVector.z) * m_throwForce, false);
            }
        }

        // Get the value of mIsHoldingObject
        public bool IsHoldingObject()
        {
            return m_heldObject;
        }

        // Get the value of mIsHoldingObject
        public Transform GetHeldObject()
        {
            if (IsHoldingObject())
                return m_heldObject.transform;
            else
                return null;
        }

        #region AimAssist

        public void SetAimAssistedStation(GameObject station)
        {
            m_ownedCauldron = station;
        }

        protected bool CheckNearAimAssistedStation()
        {
            float distanceToKitchenStation = (transform.position - m_ownedCauldron.transform.position).magnitude;
            if (distanceToKitchenStation <= AIM_ASSIST_THROW_DISTANCE && CheckFacingAimAssistedStation())
            {
                return true;
            }
            return false;
        }

        private bool CheckFacingAimAssistedStation()
        {
            Vector3 playerFacing = this.transform.TransformDirection(Vector3.forward);
            Vector3 playerToStation = m_ownedCauldron.transform.position - transform.position;
            double currentAngle = Mathf.Acos(Vector3.Dot(playerFacing, playerToStation) / (playerFacing.magnitude * playerToStation.magnitude));
            return currentAngle <= Mathf.Deg2Rad * 2 * AIM_ASSIST_FACING_DEGREE;
        }

        #endregion

        #region RangeTriggers

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

        #endregion
    }
}
