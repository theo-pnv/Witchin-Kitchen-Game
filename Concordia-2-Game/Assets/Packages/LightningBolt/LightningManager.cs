﻿using con2.messages;
using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{
    public class LightningManager : MonoBehaviour, ISpellSubscriber
    {
        private LightningBoltScript m_lightningThing;
        public float m_timeBetweenStrikes = 0.1f;
        public int m_numOfStrikes = 3;
        private MainGameManager m_mainGameManager;

        void Start()
        {
            m_lightningThing = GetComponent<LightningBoltScript>();
            m_mainGameManager = FindObjectOfType<MainGameManager>();

            var managers = GameObject.FindGameObjectWithTag(Tags.MANAGERS_TAG);
            var eventManager = managers.GetComponentInChildren<SpellsManager>();
            for (int i = 0; i < (int)Spells.SpellID.max_id; i++)
                eventManager.AddSubscriber((Spells.SpellID)i, this);
        }

        public void ActivateSpellMode(messages.Player targetedPlayer)
        {
            m_lightningThing.EndObject = m_mainGameManager.GetPlayerById(targetedPlayer.id).gameObject;
            StartCoroutine(TriggerLightning());
        }

        private IEnumerator TriggerLightning()
        {
            int i = m_numOfStrikes;
            while (i > 0)
            {
                i--;
                m_lightningThing.Trigger();
                yield return new WaitForSeconds(m_timeBetweenStrikes);
            }
        }
    }
}
