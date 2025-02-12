﻿using System.Collections;
using System.Collections.Generic;
using con2;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace con2.game
{

    public class PauseMenuInstantiator : MonoBehaviour, IInputConsumer
    {

        #region Private Variables

        private static bool IsPaused;

        [SerializeField]
        private GameObject _PauseUIPrefab;

        private GameObject _PauseUIInstance;


        #endregion

        #region Custom Methods

        public void Resume()
        {
            IsPaused = false;
            Destroy(_PauseUIInstance);
        }

        public void Pause()
        {
            IsPaused = true;
            _PauseUIInstance = Instantiate(_PauseUIPrefab);
        }

        private void ActivatePauseUI()
        {
            if (IsPaused)   //the player is trying to leave the pause menu
                Resume();
            else
                Pause();
            Debug.Log("Paused: " + IsPaused);
        }

        public bool ConsumeInput(GamepadAction input)
        {
            switch (input.GetActionID())
            {
                case GamepadAction.ID.START:
                    ActivatePauseUI();
                    return true;
                case GamepadAction.ID.PUNCH:
                    if (IsPaused)
                    {
                        Resume();
                        return true;
                    }

                    break;
            }
            return false;
        }

        #endregion
    }

}
