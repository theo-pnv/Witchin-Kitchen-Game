﻿using con2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class StirMinigame : ACookingMinigame
    {
        private static Vector2
            TOP = new Vector2(0.0f, 1.0f).normalized
            , RIGHT = new Vector2(1.0f, 0.0f).normalized
            , BOTTOM = -TOP
            , LEFT = -RIGHT;

        private static Vector2[] GOALS = { TOP, RIGHT, BOTTOM, LEFT };

        private Vector2 m_pointingDirection;
        private int m_turnsRequired, m_fullTurnCount, m_currentGoal, m_turnDirection;

        private Spin2Win m_spoonSpinner;
        private StirGame m_stirUI;

        public override void BalanceMinigame()
        {
            switch (Owner.PlayerRank)
            {
                case PlayerManager.Rank.FIRST:
                    m_turnsRequired = 4;
                    break;
                case PlayerManager.Rank.LAST:
                    m_turnsRequired = 1;
                    break;
                default:
                    m_turnsRequired = 2;
                    break;
            }
        }

        override public void StartMinigameSpecifics()
        {
            m_pointingDirection = Vector3.zero;
            m_currentGoal = 0;
            m_fullTurnCount = 0;

            if (Random.Range(0.0f, 1.0f) < 0.5f)
            {
                m_turnDirection = -1;
            }
            else
            {
                m_turnDirection = 1;
            }

            m_stirUI = m_prompt.GetComponent<StirGame>();
            m_stirUI.m_spinDir = m_turnDirection;

            m_spoonSpinner = GetComponentInChildren<Spin2Win>();
            m_spoonSpinner.SetTargetYAngle(m_currentGoal*90);
            m_spoonSpinner.SetTargetRotation(m_turnDirection);
        }

        public override bool TryToConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(con2.GamepadAction.ID.HORIZONTAL2))
            {
                float joystick = input.m_axisValue;
                if (Mathf.Abs(joystick) > 0.1f)
                    m_pointingDirection.x = joystick;
                return true;
            }
            if (input.GetActionID().Equals(con2.GamepadAction.ID.VERTICAL2))
            {
                float joystick = input.m_axisValue;
                if (Mathf.Abs(joystick) > 0.1f)
                    m_pointingDirection.y = joystick;
                return true;
            }
            return false;
        }

        override public void UpdateMinigameSpecifics()
        {
            if (Vector3.Dot(m_pointingDirection.normalized, GOALS[m_currentGoal].normalized) > 0.8f)    //The dot product of two parallel normalized vectors is 1
            {
                CycleCurrentGoal();
            }
        }

        override public void EndMinigameSpecifics()
        {

        }

        private void CycleCurrentGoal()
        {
            m_currentGoal += m_turnDirection;
            m_currentGoal %= GOALS.Length;

            if (m_currentGoal < 0)
            {
                m_currentGoal = GOALS.Length + m_currentGoal;
            }

            if (GOALS[m_currentGoal] == TOP)
            {
                FullTurnComplete();
            }

            m_spoonSpinner.SetTargetYAngle(m_currentGoal * 90);
            m_stirUI.MakingProgress();
        }

        private void FullTurnComplete()
        {
            m_fullTurnCount++;

            if (m_fullTurnCount == m_turnsRequired)
            {
                EndMinigame();
            }
        }
    }

}
