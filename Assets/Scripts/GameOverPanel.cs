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
        public Text text_uppermessage, text_lowermessage;

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
        public Button button_champion;
        public Button button_setting;
        public Button button_restart;

        #endregion

        #region Method
        void Awake()
        {
            button_champion.onClick.AddListener(() =>
            {
                OnChampionButtonClick();
            });
            button_setting.onClick.AddListener(() =>
            {
                OnSettingButtonClick();
            });

            button_restart.onClick.AddListener(() =>
            {
                // Debug.Log("UIManager.button_restart.onClick()");
                pm.gameOverPanel.HideGameOverPanel();
                animator_menu.SetTrigger("slideout");
                pm.uiManager.animator_toppanel.SetTrigger("slideout");
                pm.mainPanel.FadeIn(pm.RestartGame);
                // sceneTransition.FadeIn(pm.RestartGame);
            });

            button_endgame.onClick.AddListener(() =>
            {
                pm.uiManager.animator_score.SetTrigger("reset");
                animator_labels.SetTrigger("slidein");
                animator_menu.SetTrigger("slidein");
                animator_continue.SetTrigger("slideout");
                SetScoreLabel(pm.score);
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
        void OnChampionButtonClick()
        {
            HideGameOverPanel();
            pm.championPanel.OpenChampionPanel(PlaySceneState.gameover);
            animator_menu.SetTrigger("slideout");
            pm.uiManager.animator_toppanel.SetTrigger("slideout");
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

        public void SetMessage(string upperMessage, string lowerMessage)
        {
            text_uppermessage.text = upperMessage;
            text_lowermessage.text = lowerMessage;
        }

        public void SetScoreLabel(int score)
        {
            text_score.text = score.ToString();
            bool isNewBestScore = GameManager.Instance.SetBestScore(score);
            string colorString = isNewBestScore ? "red" : "white";
            text_highestscore.text = "<color=" + colorString + ">Highest Score " + GameManager.Instance.GetBestScore().ToString() + "</color>";
        }
        #endregion
    }
}