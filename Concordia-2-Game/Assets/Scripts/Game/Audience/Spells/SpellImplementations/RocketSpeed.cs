﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class RocketSpeed : ASpell
    {

        public float m_rocketSpeedDuration = 7.0f;
        public float m_movementModulator = 2.75f, m_rotationModulator = 0.25f;

        void Start()
        {
            SetUpSpell();
        }

        public override IEnumerator SpellImplementation()
        {
            if (!m_inProgress)
            {
                m_inProgress = true;
                var player = m_mainManager.GetPlayerById(_TargetedPlayer.id);
                var playerMovement = player.GetComponentInChildren<PlayerMovement>();
                var rainbowTrail = player.GetComponentInChildren<TrailRenderer>();

                playerMovement.ModulateMaxMovementSpeed(m_movementModulator);
                playerMovement.ModulateMovementSpeed(m_movementModulator);
                playerMovement.ModulateRotationSpeed(m_rotationModulator);
                rainbowTrail.emitting = true;

                yield return new WaitForSeconds(m_rocketSpeedDuration);

                playerMovement.ModulateMaxMovementSpeed(1.0f / m_movementModulator);
                playerMovement.ModulateMovementSpeed(1.0f / m_movementModulator);
                playerMovement.ModulateRotationSpeed(1.0f / m_rotationModulator);
                rainbowTrail.emitting = false;

                m_inProgress = false;
            }
        }

        public override Spells.SpellID GetSpellID()
        {
            return Spells.SpellID.rocket_speed;
        }
    }
}
