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
    public class GameOverPanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Animator animator_gameover;

        [Header("Labels")]
        public Animator animator_labels;
        public Text text_score, text_highestscore;
        public Button button_gift;

        [Header("Continue")]
        public Animator animator_continue;
        public Button button_endgame;
        public Button button_continuewithad, button_continuewithcoin;

        [Header("Menu")]
        public Animator animator_menu;
        public Button button_setting;
        public Button button_restart;

        #endregion

        #region Method
        void Awake()
        {
            button_setting.onClick.AddListener(() =>
            {
                OnSettingButtonClick();
            });

            button_restart.onClick.AddListener(() =>
            {
                Debug.Log("UIManager.button_restart.onClick()");
                pm.gameOverPanel.HideGameOverPanel();
                animator_menu.SetTrigger("slideout");
                pm.uiManager.animator_toppanel.SetTrigger("slideout");
                pm.mainPanel.FadeIn(pm.RestartGame);
                // sceneTransition.FadeIn(pm.RestartGame);
            });

            button_endgame.onClick.AddListener(() =>
            {
                animator_labels.SetTrigger("slidein");
                animator_menu.SetTrigger("slidein");
                animator_continue.SetTrigger("slideout");
            });
            button_continuewithad.onClick.AddListener(() =>
            {
                OnContinueButtonClick();
            });
            button_continuewithcoin.onClick.AddListener(() =>
            {
                OnContinueButtonClick();
            });
        }

        public void ShowGameOverPanel()
        {
            animator_gameover.SetTrigger("fadein");
            animator_labels.SetTrigger("reset");
        }

        public void HideGameOverPanel()
        {
            animator_gameover.SetTrigger("fadeout");
        }

        void OnSettingButtonClick()
        {
            HideGameOverPanel();
            pm.settingPanel.OpenSettingPanel(PlaySceneState.gameover);
            animator_menu.SetTrigger("slideout");
            pm.uiManager.animator_toppanel.SetTrigger("slideout");
        }

        void OnContinueButtonClick()
        {
            pm.ContinueGame();
        }
        #endregion
    }
}