﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace con2.game
{

    public class FightStun : MonoBehaviour
    {
        public AnimationCurve modifierCurve;

        private float timerElapsed = 0.0f;
        private float timerMax = 0.0f;

        private void FixedUpdate()
        {
            timerElapsed = Mathf.Min(timerMax, timerElapsed + Time.deltaTime);
        }

        public void Stun(float seconds)
        {
            timerMax = seconds;
            timerElapsed = 0.0f;
        }

        public float getMovementModifier()
        {
            // Avoid division by 0
            if (timerMax == 0.0f)
                return 1.0f;

            return modifierCurve.Evaluate(timerElapsed / timerMax);
        }
    }

}
