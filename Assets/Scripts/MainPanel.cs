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
        public Button button_champion;
        public Color color_campaign_space, color_infinity_space;
        public Material backgroundMaterial;

        [Header("Play")]
        // public PlayMode playMode;
        public Button button_play;
        public Button button_campaign, button_infinity;
        public Image image_outline_campaign, image_outline_infinity;

        [Header("More")]
        public Button button_more;
        public bool isMorePanelOpened;
        public GameObject wrapper_more;
        public Button button_setting, button_leaderboard;


        #endregion

        #region Method

        public void InitMainPanel()
        {
            // Debug.Log("MainPanel.InitMainPanel()");
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
            button_champion.onClick.AddListener(() =>
            {
                OnChampionButtonClick();
            });

            button_leaderboard.onClick.AddListener(() =>
            {
                OnLeaderboardButtonClick();
            });

            isMorePanelOpened = false;
            wrapper_more.SetActive(false);

            button_campaign.onClick.AddListener(() =>
            {
                if (pm.playMode != PlayMode.campaign) //이미 캠페인 모드라면 스테이지를 재 생성할 필요가 없다
                {
                    SetPlayMode(PlayMode.campaign);
                    pm.cameraController.AnimateCameraBackgroundColor(color_infinity_space, color_campaign_space, 0.5f);
                }
            });
            button_infinity.onClick.AddListener(() =>
            {
                if (pm.playMode != PlayMode.infinity)
                {
                    pm.cameraController.AnimateCameraBackgroundColor(color_campaign_space, color_infinity_space, 0.5f);
                }
                SetPlayMode(PlayMode.infinity, true);
            });
        }

        public void SetPlayMode(PlayMode playMode, bool withRestart = false)
        {
            GameManager.Instance.SetPlayMode(playMode);
            pm.playMode = playMode;
            if (playMode == PlayMode.campaign)
            {
                image_outline_campaign.enabled = true;
                image_outline_infinity.enabled = false;
                pm.RestartGame();
            }
            else if (playMode == PlayMode.infinity)
            {
                image_outline_campaign.enabled = false;
                image_outline_infinity.enabled = true;
                if (withRestart) pm.RestartGame();
            }
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

        void OnChampionButtonClick()
        {
            // Debug.Log("MainPanel.OnChampionButtonClick()");
            FastOut();
            pm.championPanel.OpenChampionPanel(PlaySceneState.main);
        }

        void OnLeaderboardButtonClick()
        {
            //TODO
        }
        #endregion
    }

    public enum PlayMode
    {
        campaign = 0, infinity = 1
    }
}