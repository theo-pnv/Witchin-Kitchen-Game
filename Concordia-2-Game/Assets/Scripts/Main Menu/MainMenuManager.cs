﻿using System.Collections;
using System.Collections.Generic;
using con2.game;
using con2.messages;
using UnityEngine;
using Key = con2.PlayerPrefsKeys;

namespace con2.main_menu
{

    public class MainMenuManager : MonoBehaviour
    {
        void Start()
        {
            // https://www.reddit.com/r/Unity3D/comments/4qrrbl/unity_build_seems_to_idle_at_a_very_high_cpu_usage/
            // Avoid computing more frames than needed (120fps at most)
            // Avoid high CPU usage
            QualitySettings.vSyncCount = 1;

            ResetStaticVars();

            // Setting default values in case this is the first time the app is started
            if (!PlayerPrefs.HasKey(Key.HOST_ADDRESS))
            {
                PlayerPrefs.SetString(Key.HOST_ADDRESS, "http://dev.audience.witchin-kitchen.com/");
            }

            var audienceManager = FindObjectOfType<AudienceInteractionManager>();
            if (audienceManager)
            {
                Destroy(audienceManager.gameObject);
            }
        }

        private void ResetStaticVars()
        {
            GameInfo.RoomId = "000";
            GameInfo.Viewers = new List<Viewer>();
            GameInfo.PlayerNumber = 0;
            GameInfo.ThemeIngredient = (int) game.Ingredient.NOT_AN_INGREDIENT;
        }
    }

}
