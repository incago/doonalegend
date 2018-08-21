/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoonaLegend
{
    public class MainPanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Animator animator;
        Action callback;

        [Header("Play")]
        public Button button_play;

        [Header("More")]
        public Button button_more;
        public bool isMorePanelOpened;
        public GameObject wrapper_more;
        public Button button_setting, button_leaderboard;


        #endregion

        #region Method

        void Awake()
        {
            button_more.onClick.AddListener(() =>
            {
                ToggleMorePanel();
            });

            button_play.onClick.AddListener(() =>
            {
                PlayGame();
            });

            button_setting.onClick.AddListener(() =>
            {
                OnSettingButtonClick();
            });

            button_leaderboard.onClick.AddListener(() =>
            {
                OnLeaderboardButtonClick();
            });

            isMorePanelOpened = false;
            wrapper_more.SetActive(false);
        }

        void PlayGame()
        {
            pm.uiManager.animator_control.SetTrigger("slidein");
            pm.uiManager.animator_toppanel.SetTrigger("slidein");
            pm.uiManager.animator_score.SetTrigger("fadein");
            animator.SetTrigger("fadeout");
            pm.playSceneState = PlaySceneState.ready;

        }

        void ToggleMorePanel()
        {
            isMorePanelOpened = !isMorePanelOpened;
            wrapper_more.SetActive(isMorePanelOpened);
        }

        public void FadeIn(Action callback)
        {
            // Debug.Log("SceneTransition.FadeIn()");
            this.callback = callback;
            animator.SetTrigger("fadein");
            // StartCoroutine(FadeInHelper(0.5833f, callback));
        }

        public void FastIn()
        {
            animator.SetTrigger("fastin");
            // Debug.Log("SceneTransition.FastIn()");
        }

        public void FastOut()
        {
            animator.SetTrigger("fastout");
            // Debug.Log("SceneTransition.FastIn()");
        }

        public void ExcuteCallBack()
        {
            isMorePanelOpened = false;
            wrapper_more.SetActive(isMorePanelOpened);
            this.callback();
        }

        // IEnumerator FadeInHelper(float delay, Action callback)
        // {
        //     yield return new WaitForSeconds(delay);
        //     Debug.Log("MainPanel.FadeInHelper() : callBack()");
        //     callback();
        // }

        void OnSettingButtonClick()
        {
            FastOut();
            pm.settingPanel.OpenSettingPanel(PlaySceneState.main);
        }

        void OnLeaderboardButtonClick()
        {
            //TODO
        }
        #endregion
    }
}