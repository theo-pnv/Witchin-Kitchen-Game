﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class GlobalFightState : MonoBehaviour
    {
        // Public
        public static GlobalFightState get()
        {
            return instance;
        }

        public void AddFighter(GameObject fighter)
        {
            fighters.Add(fighter);
        }

        public float PunchUpwardsForce = 1.0f;
        public float PunchForceMultiplier = 5.0f;
        public float PunchStunSeconds = 1.0f;
        public float PunchReloadSeconds = 1.33f;
        public float PunchLingerSeconds = 0.33f;

        public List<FighterPunch.PunchEvent> Punches = new List<FighterPunch.PunchEvent>();


        // Private

        private static GlobalFightState instance;

        private List<GameObject> fighters = new List<GameObject>();


        // Events

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            foreach (var punch in Punches)
            {
                var knockVelocity = punch.TargetPosition - punch.SourcePosition;
                knockVelocity.y = PunchUpwardsForce;
                knockVelocity.Normalize();
                knockVelocity *= PunchForceMultiplier;

                var targetBody = punch.Target.GetComponent<Rigidbody>();
                targetBody.velocity = knockVelocity;

                var stun = punch.Target.GetComponent<FightStun>();
                stun.Stun(PunchStunSeconds);
                PlayerPickUpDropObject pickUpSystem = punch.Target.GetComponent<PlayerPickUpDropObject>();
                if (pickUpSystem)
                {
                    pickUpSystem.ForceDropObject(-knockVelocity.normalized);
                }
            }

            // Setup for next frame
            Punches.Clear();
        }
    }

}
