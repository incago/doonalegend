/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
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
        // public Button button_addsection;
        public Canvas canvas;
        [Header("Top Panel")]
        public Animator animator_toppanel;
        public Text text_distance, text_kill;
        public Animator animator_distance, animator_kill;

        [Header("Coin")]
        public Animator animator_coin;
        public Text text_coin;

        [Header("Control")]
        public Animator animator_control;
        public Button button_left, button_forward, button_right, button_backward;

        [Header("HP")]
        public HorizontalLayoutGroup layoutgroup_heart;
        public Animator animator_heart;
        public Transform container_heart;
        public Transform heartSlotPrefab;
        public List<HeartSlot> hearts;
        public GridLayoutGroup grid;
        // public Color[] heartColors;

        [Header("SP")]
        public Slider slider_sp;

        [Header("Score")]
        public Text text_score;
        public Text text_score_add;
        public NicerOutline text_scoreoutline;
        public Animator animator_score;

        // [Header("Pause")]
        // public Animator animator_pause;
        // public GameObject panel_pause;
        // public Button button_pause, button_resume;

        [Header("Gameover")]
        public GameObject panel_gameover;
        public Text text_bestscore, text_currentscore, text_getcoin;

        [Header("Scene Transition")]
        public SceneTransition sceneTransition;

        [Header("HUD")]
        public Transform container_hud;
        public CanvasHud canvasHudPrefab;
        #endregion

        #region Method
        void Awake()
        {
            // sceneTransition.gameObject.SetActive(true);
            animator_score.gameObject.SetActive(true);
            animator_toppanel.gameObject.SetActive(true);
            animator_control.gameObject.SetActive(true);
            pm.gameOverPanel.animator_menu.gameObject.SetActive(true);
            pm.gameOverPanel.animator_continue.gameObject.SetActive(true);

            // button_addsection.onClick.AddListener(() =>
            // {
            //     pathManager.AddSection();
            // });

            /*
                    button_restart.onClick.AddListener(() =>
                    {
                        Debug.Log("UIManager.button_restart.onClick()");
                        pm.gameOverPanel.HideGameOverPanel();
                        animator_menu.SetTrigger("slideout");
                        pm.mainPanel.FadeIn(pm.RestartGame);
                        // sceneTransition.FadeIn(pm.RestartGame);
                    });
             */

            button_left.onClick.AddListener(() => { pm.AddActionToQueue(PlayerInput.left); });
            button_forward.onClick.AddListener(() => { pm.AddActionToQueue(PlayerInput.forward); });
            button_right.onClick.AddListener(() => { pm.AddActionToQueue(PlayerInput.right); });

            // button_left.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.left; });
            // button_right.onClick.AddListener(() => { pm.inputManager.lastInput = PlayerInput.right; });

            // button_backward.onClick.AddListener(() => { pm.PlayerAction(PlayerInput.backward); });
        }

        void Start()
        {
            // sceneTransition.FadeOut();
        }

        public void InitUIManager()
        {
            InitHpUI(pm.champion.maxHp, pm.champion.startingHp);
            // slider_hp.value = slider_hp.maxValue = 100.0f;
            // fill_hp.color = color_hp_green;
            slider_sp.maxValue = 100.0f;
            slider_sp.value = 0.0f;
            // text_distance.text = "0";
            // text_kill.text = "0";
        }

        public void UpdateDistance(int score, bool withAnimation = false)
        {
            text_distance.text = score.ToString();
            if (withAnimation)
            {
                animator_distance.SetTrigger("update");
            }
        }

        public void UpdateKill(int kill, bool withAnimation = false)
        {
            text_kill.text = kill.ToString();
            if (withAnimation)
            {
                animator_kill.SetTrigger("update");
            }
        }

        public void UpdateSp()
        {
            slider_sp.value = pm.champion.sp;
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

        public void InitHpUI(int maxHp, int startingHp, bool withAnimation = false)
        {
            hearts.Clear();
            DestroyChildGameObject(container_heart);
            if (maxHp > 14) maxHp = 14;
            int heartCount = maxHp % 2 == 1 ? (maxHp + 1) / 2 : maxHp / 2;

            grid.constraintCount = Mathf.Clamp(heartCount, 1, 7);
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

                if (i < startingHp / 2) heartStatus = HeartStatus.full;
                else if ((i == startingHp / 2) && (startingHp % 2 == 1)) heartStatus = HeartStatus.half;
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
                int currentHp = pm.champion.currentHp;
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

        public void UpdateScoreUI(int value, int value2, Color? color = null, Color? outline = null)
        {
            UpdateScoreUI(value.ToString(), value2 == 0 ? "" : "+" + value2.ToString(), color, outline);
        }

        public void UpdateScoreUI(string message1, string message2, Color? color = null, Color? outline = null)
        {
            message2 = ""; //hot fix
            text_score.text = message1;
            text_score_add.text = message2;
            text_score.color = color ?? Color.white;
            text_scoreoutline.effectColor = outline ?? Color.black;
            animator_score.SetTrigger("update");
        }

        public void MakeCanvasMessageHud(Transform target, string message, Vector3 localOffset, Color fontColor, Color outlineColor)
        {
            // Debug.Log("UIManager.MakeCanvasMessageHud()");
            MakeCanvasMessageHud(target.position, message, localOffset, fontColor, outlineColor);
        }

        public void MakeCanvasMessageHud(Vector3 position, string message, Vector3 localOffset, Color fontColor, Color outlineColor)
        {
            // Debug.Log("UIManager.MakeCanvasMessageHud()");
            CanvasHud messageHUDInstance = Instantiate(canvasHudPrefab) as CanvasHud;
            messageHUDInstance.transform.SetParent(container_hud);
            messageHUDInstance.transform.localScale = Vector3.one;
            messageHUDInstance.Init(position, message, localOffset, fontColor, outlineColor);
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