﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static con2.GamepadAction;


namespace con2.game
{

    public class MashButton : ACookingMinigame
    {
        public ID m_inputToMash = ID.INTERACT;
        private int m_numberOfPressesRequired;
        public float m_timeToLoseProgress = 1.0f;

        private int m_currentNumberOfPresses = 0;
        private float m_lastMashTime = 0;

        private SpoonMash m_masher;
        private MashAGame m_mashUI;

        public override void BalanceMinigame()
        {
            switch (m_stationOwner.PlayerRank)
            {
                case PlayerManager.Rank.FIRST:
                    m_numberOfPressesRequired = 16;
                    break;
                case PlayerManager.Rank.LAST:
                    m_numberOfPressesRequired = 6;
                    break;
                default:
                    m_numberOfPressesRequired = 11;
                    break;
            }        
        }

        override public void StartMinigameSpecifics()
        {
            m_currentNumberOfPresses = 0;
            m_lastMashTime = 0;

            m_masher = GetComponentInChildren<SpoonMash>();
            m_masher.StartMash();

            m_mashUI = m_prompt.GetComponent<MashAGame>();
        }

        public override bool TryToConsumeInput(GamepadAction input)
        {
            if (input.GetActionID().Equals(m_inputToMash))
            {
                m_lastMashTime = Time.time;
                m_currentNumberOfPresses++;
                m_masher.Mash();
                m_mashUI.MakingProgress();
                if (m_currentNumberOfPresses >= m_numberOfPressesRequired)
                {
                    EndMinigame();
                }
                return true;
            }
            return false;
        }

        override public void UpdateMinigameSpecifics()
        {
            if (m_currentNumberOfPresses > 0)
            {
                if (Time.time - m_lastMashTime > m_timeToLoseProgress)
                {
                    m_lastMashTime = Time.time;
                    m_currentNumberOfPresses--;
                }
            }
        }

        override public void EndMinigameSpecifics()
        {
            m_masher.EndMash();
        }

    }
}
