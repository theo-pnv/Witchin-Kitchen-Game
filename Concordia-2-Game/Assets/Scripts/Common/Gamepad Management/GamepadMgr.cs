﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public class GamepadMgr : MonoBehaviour
    {
        public static GamepadMgr instance;

        public const int NUM_PADS = 4;

        private List<Gamepad> gamepads = new List<Gamepad>();

        public void Awake()
        {
            instance = this;

            for (int i = 0; i < NUM_PADS; ++i)
            {
                gamepads.Add(new Gamepad(i));
            }
        }

        void OnEnable()
        {
            EarlyUpdate.EarlyUpdateEvent += EarlyUpdateCallback;
        }

        void OnDisable()
        {
            EarlyUpdate.EarlyUpdateEvent -= EarlyUpdateCallback;
        }

        // Will be called before any other component is Update()'d,
        // to avoid possible 1-frame delay
        private void EarlyUpdateCallback()
        {
            foreach (Gamepad pad in gamepads)
            {
                pad.Poll();
            }
        }

        /// <summary>
        /// Return the instance of the gamepad associated to the player
        /// </summary>
        /// <param name="playerIdx"></param>
        /// <returns></returns>
        public static Gamepad Pad(int playerIdx)
        {
            return instance.gamepads[playerIdx];
        }
    }
}
