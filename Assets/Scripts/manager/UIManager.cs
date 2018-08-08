/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        public Text text_score, text_kill;

        [Header("Coin")]
        public Animator animator_coin;
        public Text text_coin;

        [Header("Control")]
        public Animator animator_control;
        public Button button_left, button_forward, button_right, button_backward;

        [Header("Menu")]
        public Animator animator_menu;
        public Button button_restart;

        [Header("HP")]
        // public Slider slider_hp;
        // public Image fill_hp;
        // public Color color_hp_green, color_hp_yellow, color_hp_red;
        public HorizontalLayoutGroup layoutgroup_heart;
        public Animator animator_heart;
        public Transform container_heart;
        public Transform heartSlotPrefab;
        public List<HeartSlot> hearts;
        public GridLayoutGroup grid;
        // public Color[] heartColors;

        [Header("SP")]
        public Slider slider_sp;

        // [Header("Pause")]
        // public Animator animator_pause;
        // public GameObject panel_pause;
        // public Button button_pause, button_resume;

        [Header("Gameover")]
        public GameObject panel_gameover;
        public Text text_bestscore, text_currentscore, text_getcoin;

        [Header("Scene Transition")]
        public SceneTransition sceneTransition;
        #endregion

        #region Method
        void Awake()
        {
            sceneTransition.gameObject.SetActive(true);
            animator_menu.gameObject.SetActive(true);

            button_addsection.onClick.AddListener(() =>
            {
                pathManager.AddSection();
            });

            button_restart.onClick.AddListener(() =>
            {
                pm.gameOverPanel.HideGameOverPanel();
                animator_menu.SetTrigger("slideout");
                sceneTransition.FadeIn(pm.RestartGame);
            });

            button_left.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.left); });
            button_forward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.forward); });
            button_right.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.right); });

            // button_left.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.left; });
            // button_right.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.right; });

            // button_backward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.backward); });
        }

        void Start()
        {
            sceneTransition.FadeOut();
        }

        public void InitUIManager()
        {
            InitHpUI(pm.player.maxHp, pm.player.hp);
            // slider_hp.value = slider_hp.maxValue = 100.0f;
            // fill_hp.color = color_hp_green;
            slider_sp.maxValue = 100.0f;
            slider_sp.value = 0.0f;
            text_score.text = "0";
            text_kill.text = "0";
        }

        public void UpdateScore(int score)
        {
            text_score.text = score.ToString();
        }



        public void UpdateSp()
        {
            slider_sp.value = pm.player.sp;
        }

        public void UpdateCoin(bool withAnimation = false)
        {
            int coinValue = GameManager.Instance.GetPlayerCoinFromPref();
            text_coin.text = coinValue.ToString();
            if (withAnimation)
            {
                animator_coin.SetTrigger("update");
            }
        }

        public void ShowGameOverPanel()
        {
            panel_gameover.SetActive(true);
            // text_bestscore.text = GameManager.Instance.GetBestScoreFromPref().ToString();
            // text_currentscore.text = pm.score.ToString();
            // text_getcoin.text = pm.addCoin.ToString();
        }

        public void HideGameOverPanel()
        {
            panel_gameover.SetActive(false);
        }

        public void InitHpUI(int maxHp, int currentHp, bool withAnimation = false)
        {
            hearts.Clear();
            DestroyChildGameObject(container_heart);
            if (maxHp > 32) maxHp = 32;
            int heartCount = maxHp % 2 == 1 ? (maxHp + 1) / 2 : maxHp / 2;

            grid.constraintCount = Mathf.Clamp(heartCount, 1, 8);
            // Debug.Log("maxHp: " + maxHp.ToString());
            // Debug.Log("heartCount: " + heartCount.ToString());
            for (int i = 0; i < heartCount; i++)
            {
                Transform heartTransform = Instantiate(heartSlotPrefab) as Transform;
                heartTransform.SetParent(container_heart);
                heartTransform.localScale = Vector3.one;
                heartTransform.position = Vector2.zero;
                HeartSlot heartSlot = heartTransform.GetComponent<HeartSlot>();
                hearts.Add(heartSlot);
                int bgColorIndex = (maxHp - 1) / 8;
                int heartColorIndex = bgColorIndex + 1;
                HeartStatus heartStatus = HeartStatus.empty;

                // int remainHp = currentHp % 8;
                // Debug.Log("i: " + i.ToString());
                // Debug.Log("remainHp: " + remainHp.ToString());

                if (i < currentHp / 2) heartStatus = HeartStatus.full;
                else if ((i == currentHp / 2) && (currentHp % 2 == 1)) heartStatus = HeartStatus.half;
                else heartStatus = HeartStatus.empty;

                heartSlot.SetHeartSlot(heartStatus);

            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(container_heart.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutgroup_heart.GetComponent<RectTransform>());
            // Canvas.ForceUpdateCanvases();
            // layoutgroup_heart.SetLayoutHorizontal();
            if (withAnimation) animator_heart.SetTrigger("update");
        }

        public void UpdateHpUI(bool withAnimation = false)
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                int currentHp = pm.player.hp;
                HeartStatus heartStatus = HeartStatus.empty;
                if (i < currentHp / 2) heartStatus = HeartStatus.full;
                else if ((i == currentHp / 2) && (currentHp % 2 == 1)) heartStatus = HeartStatus.half;
                else heartStatus = HeartStatus.empty;
                hearts[i].SetHeartSlot(heartStatus);
            }
            if (withAnimation) animator_heart.SetTrigger("update");
            // slider_hp.value = pm.player.hp;
            // float percent = (float)slider_hp.value / (float)slider_hp.maxValue;
            // if (percent < 0.5f)
            // { fill_hp.color = Color.Lerp(color_hp_red, color_hp_yellow, percent * 2); }
            // else
            // { fill_hp.color = Color.Lerp(color_hp_yellow, color_hp_green, (percent - 0.5f) * 2); }
        }

        public void DestroyChildGameObject(Transform parent)
        {
            var childItems = new List<GameObject>();
            foreach (Transform child in parent) childItems.Add(child.gameObject);
            childItems.ForEach(child => Destroy(child));
        }
        #endregion
    }
}