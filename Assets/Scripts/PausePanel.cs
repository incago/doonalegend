/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoonaLegend
{
    public class PausePanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public bool isPaused = false;
        public Animator animator_pause;
        public Button button_pause, button_resume;
        #endregion

        #region Method

        void Awake()
        {
            button_pause.onClick.AddListener(() =>
            {
                Pause();
            });

            button_resume.onClick.AddListener(() =>
            {
                Resume();
            });
        }

        public void Pause()
        {
            pm.playSceneState = PlaySceneState.pause;
            Time.timeScale = 0.0f;
            animator_pause.SetTrigger("fadein");
            isPaused = true;
            pm.uiManager.animator_control.SetTrigger("slideout");
        }

        public void Resume()
        {
            animator_pause.SetTrigger("fadeout");
            pm.uiManager.animator_control.SetTrigger("slidein");
        }

        public void ResumeHelper()
        {
            pm.playSceneState = PlaySceneState.play;
            Time.timeScale = 1.0f;
            isPaused = false;
        }

        public void CountSound()
        {
            SoundManager.Instance.Play("pop");
        }


        #endregion
    }
}