/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class InputManager : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public PlayerInput lastInput;
        #endregion

        #region Method

        void Awake()
        {
            lastInput = PlayerInput.none;
        }
        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // { lastInput = PlayerInput.left; }
            // if (Input.GetKeyDown(KeyCode.RightArrow))
            // { lastInput = PlayerInput.right; }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            { pm.PlayerAction(PlayerInput.left); }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            { pm.PlayerAction(PlayerInput.right); }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            { pm.PlayerAction(PlayerInput.forward); }
            if (Input.GetKeyDown(KeyCode.Slash))
            { pm.PlayerAction(PlayerInput.forward); }
            if (Input.GetKeyDown(KeyCode.RightShift))
            { pm.PlayerAction(PlayerInput.forward); }

            // if (Input.GetKeyDown(KeyCode.DownArrow))
            // { pm.PlayerAction(PlayerInput.backward); }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (pm.pausePanel.isPaused)
                {
                    pm.pausePanel.animator_pause.SetTrigger("reset");
                    Time.timeScale = 1.0f;
                }
                // pm.uiManager.panel_pause.SetActive(false);
                pm.gameOverPanel.HideGameOverPanel();
                pm.uiManager.animator_control.SetTrigger("slidein");
                pm.gameOverPanel.animator_menu.SetTrigger("slideout");
                pm.ResetGame();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (pm.pausePanel.isPaused) pm.pausePanel.Resume();
                else pm.pausePanel.Pause();
            }
        }
        #endregion
    }

    public enum PlayerInput
    {
        none = 0, left = 1, right = 2, forward = 3, backward = 4, dash = 5, attack = 6
    }
}