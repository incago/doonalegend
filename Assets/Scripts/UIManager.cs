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
    public class UIManager : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public PathManager pathManager;
        public Button button_addsection;
        public Text text_score, text_kill, text_coin;
        public Button button_left, button_forward, button_right, button_backward;

        [Header("HP")]
        public Slider slider_hp;
        public Image fill_hp;
        public Color color_hp_green, color_hp_yellow, color_hp_red;

        [Header("SP")]
        public Slider slider_sp;

        [Header("Pause")]
        public GameObject panel_pause;
        public Button button_pause, button_resume;

        [Header("Gameover")]
        public GameObject panel_gameover;
        public Button button_restart;
        public Text text_bestscore, text_currentscore, text_getcoin;
        #endregion

        #region Method
        void Awake()
        {
            button_addsection.onClick.AddListener(() =>
            {
                pathManager.AddSection();
            });

            button_restart.onClick.AddListener(() =>
            {
                HideGameOverPanel();
                pm.ResetGame();
            });

            button_pause.onClick.AddListener(() =>
            {
                panel_pause.SetActive(true);
                Time.timeScale = 0.0f;
            });

            button_resume.onClick.AddListener(() =>
            {
                panel_pause.SetActive(false);
                Time.timeScale = 1.0f;
            });

            // button_left.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.left); });
            // button_forward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.forward); });
            // button_right.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.right); });

            button_left.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.left; });
            button_right.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.right; });

            // button_backward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.backward); });
        }

        public void InitUIManager()
        {
            slider_hp.value = slider_hp.maxValue = 100.0f;
            fill_hp.color = color_hp_green;
            slider_sp.maxValue = 100.0f;
            slider_sp.value = 0.0f;
            text_score.text = "0";
            text_kill.text = "0";
        }

        public void UpdateScore(int score)
        {
            text_score.text = score.ToString();
        }

        public void UpdateHp()
        {
            slider_hp.value = pm.player.hp;
            float percent = (float)slider_hp.value / (float)slider_hp.maxValue;
            if (percent < 0.5f)
            { fill_hp.color = Color.Lerp(color_hp_red, color_hp_yellow, percent * 2); }
            else
            { fill_hp.color = Color.Lerp(color_hp_yellow, color_hp_green, (percent - 0.5f) * 2); }
        }

        public void UpdateSp()
        {
            slider_sp.value = pm.player.sp;
        }

        public void UpdateCoin()
        {
            int coinValue = GameManager.Instance.GetPlayerCoinFromPref();
            text_coin.text = coinValue.ToString();
        }

        public void ShowGameOverPanel()
        {
            panel_gameover.SetActive(true);
            text_bestscore.text = GameManager.Instance.GetBestScoreFromPref().ToString();
            text_currentscore.text = pm.score.ToString();
            text_getcoin.text = pm.addCoin.ToString();
        }

        public void HideGameOverPanel()
        {
            panel_gameover.SetActive(false);
        }
        #endregion
    }
}