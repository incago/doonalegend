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
        public Button button_restart;
        public Text text_score, text_kill, text_coin;
        public Button button_left, button_forward, button_right;

        [Header("HP")]
        public Slider slider_hp;
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
                pm.ResetGame();
            });

            button_left.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.left); });
            button_forward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.forward); });
            button_right.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.right); });
        }

        public void InitUIManager()
        {
            slider_hp.value = slider_hp.maxValue = 100.0f;
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
        }

        public void UpdateCoin()
        {
            text_coin.text = 0.ToString();
        }
        #endregion
    }
}