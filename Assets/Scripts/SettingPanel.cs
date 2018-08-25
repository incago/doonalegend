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
    public class SettingPanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Button button_back;
        public Animator animator;
        public PlaySceneState from;
        public SettingPanelStatus settingPanelStatus;
        public Text text_version;
        public Transform container;

        [Header("Panels")]
        public GameObject panel_root;
        public GameObject panel_credit;

        [Header("UI Size")]
        public UISizeType uiSizeType;
        public Button button_uisize;
        public Text text_uisize;
        public Image image_uisize;
        public Sprite sprite_uisize_phone, sprite_uisize_tablet;
        public float canvasheight = 1136.0f;
        public float canvaswidth_phone, canvaswidth_tablet;
        public RectTransform champion_preview;
        public Camera camera_champion;

        [Header("Sound")]
        public Button button_sound;
        public Text text_sound;
        public Image image_sound;
        public Sprite sprite_sound_on, sprite_sound_off;

        [Header("Padding")]
        public Button button_padding;
        public Text text_padding;
        public Image image_padding;
        public Sprite sprite_padding_on, sprite_padding_off;

        [Header("Padding - Related")]
        public RectTransform main_bottom;
        public RectTransform main_title;
        public RectTransform play_control;
        public RectTransform play_top;
        public RectTransform play_score;
        public RectTransform gameover_continue;
        public RectTransform gameover_menu;
        public RectTransform gameover_labels;
        public RectTransform container_champion;

        [Header("Effect")]
        public Button button_effect;
        public Text text_effect;
        public Image image_effect;
        public Sprite sprite_effect_on, sprite_effect_off;
        public ParticleSystem effect_spacecube;
        public Light sun;

        [Header("Credit")]
        public Button button_credit;
        public Button button_contact;
        public string contactEmail = "asnetgames@gmail.com";
        #endregion

        #region Method
        void Awake()
        {
            text_version.text = "Version " + Application.version;
            uiSizeType = GameManager.Instance.GetUISizeType();
            SetUISize(uiSizeType);
            if (uiSizeType == UISizeType.phone)
            {
                image_uisize.sprite = sprite_uisize_phone;
                text_uisize.text = "UI - Phone";
            }
            else if (uiSizeType == UISizeType.tablet)
            {
                image_uisize.sprite = sprite_uisize_tablet;
                text_uisize.text = "UI - Tablet";
            }
            bool useSound = GameManager.Instance.GetSoundActive();
            text_sound.text = "Sound - " + (useSound ? "On" : "Off");
            image_sound.sprite = useSound ? sprite_sound_on : sprite_sound_off;

            bool useEffect = GameManager.Instance.GetEffectActive();
            text_effect.text = "Effects - " + (useEffect ? "On" : "Off");
            image_effect.sprite = useEffect ? sprite_effect_on : sprite_effect_off;
            effect_spacecube.gameObject.SetActive(useEffect);
            sun.shadows = useEffect ? LightShadows.Hard : LightShadows.None;

            bool usePadding = GameManager.Instance.GetPaddingActive();
            text_padding.text = "Padding - " + (usePadding ? "On" : "Off");
            image_padding.sprite = usePadding ? sprite_padding_on : sprite_padding_off;
            AdjustPadding(usePadding);

            button_back.onClick.AddListener(() => { OnBackButtonClick(); });
            button_uisize.onClick.AddListener(() => { OnUISizeButtonClick(); });
            button_effect.onClick.AddListener(() => { OnEffectButtonClick(); });
            button_padding.onClick.AddListener(() => { OnPaddingButtonClick(); });
            button_sound.onClick.AddListener(() => { OnSoundButtonClick(); });
            button_credit.onClick.AddListener(() => { OnCreditButtonClick(); });
            button_contact.onClick.AddListener(() =>
            {
                Application.OpenURL("mailto:" + contactEmail);
            });


        }

        public void OpenSettingPanel(PlaySceneState from)
        {
            this.from = from;
            this.settingPanelStatus = SettingPanelStatus.root;
            animator.SetTrigger("fadein");
        }

        public void CloseSettingPanel()
        {
            pm.mainPanel.isMorePanelOpened = false;
            pm.mainPanel.wrapper_more.SetActive(pm.mainPanel.isMorePanelOpened);
            animator.SetTrigger("fadeout");
        }

        void OnBackButtonClick()
        {
            if (settingPanelStatus == SettingPanelStatus.root)
            {
                if (from == PlaySceneState.main)
                {
                    CloseSettingPanel();
                    pm.mainPanel.FastIn();
                }
                else if (from == PlaySceneState.gameover)
                {
                    CloseSettingPanel();
                    pm.gameOverPanel.ShowGameOverPanel();
                    pm.gameOverPanel.animator_menu.SetTrigger("slidein");
                    pm.gameOverPanel.animator_labels.SetTrigger("slidein");
                    pm.uiManager.animator_toppanel.SetTrigger("slidein");
                }
            }
            else if (settingPanelStatus == SettingPanelStatus.credit)
            {
                settingPanelStatus = SettingPanelStatus.root;
                panel_root.SetActive(true);
                panel_credit.SetActive(false);
            }
        }

        void OnUISizeButtonClick()
        {
            UISizeType targetUISizeType = UISizeType.phone;
            if (GameManager.Instance.GetUISizeType() == UISizeType.phone)
            {
                targetUISizeType = UISizeType.tablet;
                image_uisize.sprite = sprite_uisize_tablet;
                text_uisize.text = "UI - Tablet";
            }
            else if (GameManager.Instance.GetUISizeType() == UISizeType.tablet)
            {
                targetUISizeType = UISizeType.phone;
                image_uisize.sprite = sprite_uisize_phone;
                text_uisize.text = "UI - Phone";
            }
            this.uiSizeType = targetUISizeType;
            GameManager.Instance.SetUISizeType(targetUISizeType);
            SetUISize(targetUISizeType);
        }

        public void SetUISize(UISizeType uiSizeType)
        {
            if (uiSizeType == UISizeType.phone)
            {
                pm.uiManager.canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_phone, canvasheight);
                pm.cameraController.mainCamera.orthographicSize = pm.cameraController.camerasize_phone;
                champion_preview.sizeDelta = new Vector2(canvaswidth_phone, canvaswidth_phone);
                camera_champion.orthographicSize = 3;
            }
            else if (uiSizeType == UISizeType.tablet)
            {
                pm.uiManager.canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_tablet, canvasheight);
                pm.cameraController.mainCamera.orthographicSize = pm.cameraController.camerasize_tablet;
                champion_preview.sizeDelta = new Vector2(canvaswidth_tablet, canvaswidth_tablet);
                camera_champion.orthographicSize = 4;
            }
        }

        void OnEffectButtonClick()
        {
            bool useEffect = !GameManager.Instance.GetEffectActive();
            text_effect.text = "Effects - " + (useEffect ? "On" : "Off");
            image_effect.sprite = useEffect ? sprite_effect_on : sprite_effect_off;
            GameManager.Instance.SetEffectActive(useEffect);
            effect_spacecube.gameObject.SetActive(useEffect);
            sun.shadows = useEffect ? LightShadows.Hard : LightShadows.None;
        }

        void OnPaddingButtonClick()
        {
            bool usePadding = !GameManager.Instance.GetPaddingActive();
            text_padding.text = "Padding - " + (usePadding ? "On" : "Off");
            image_padding.sprite = usePadding ? sprite_padding_on : sprite_padding_off;
            GameManager.Instance.SetPaddingActive(usePadding);
            AdjustPadding(usePadding);
        }

        public void AdjustPadding(bool usePadding)
        {
            if (usePadding)
            {
                container.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -100);
                main_title.anchoredPosition = new Vector2(0, -214);
                main_bottom.sizeDelta = new Vector2(0, 312);
                play_control.sizeDelta = new Vector2(0, 230);
                play_top.sizeDelta = new Vector2(0, 230);
                play_score.anchoredPosition = new Vector2(0, -270);
                gameover_continue.sizeDelta = new Vector2(0, 382);
                gameover_menu.sizeDelta = new Vector2(0, 166);
                gameover_labels.sizeDelta = new Vector2(0, -395);
                container_champion.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -100);
            }
            else
            {
                container.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                main_title.anchoredPosition = new Vector2(0, -164);
                main_bottom.sizeDelta = new Vector2(0, 262);
                play_control.sizeDelta = new Vector2(0, 180);
                play_top.sizeDelta = new Vector2(0, 180);
                play_score.anchoredPosition = new Vector2(0, -220);
                gameover_continue.sizeDelta = new Vector2(0, 332);
                gameover_menu.sizeDelta = new Vector2(0, 116);
                gameover_labels.sizeDelta = new Vector2(0, -295);
                container_champion.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            }
        }

        void OnSoundButtonClick()
        {
            bool useSound = !GameManager.Instance.GetSoundActive();
            text_sound.text = "Sound - " + (useSound ? "On" : "Off");
            image_sound.sprite = useSound ? sprite_sound_on : sprite_sound_off;
            GameManager.Instance.SetSoundActive(useSound);
        }

        void OnCreditButtonClick()
        {
            panel_root.SetActive(false);
            panel_credit.SetActive(true);
            settingPanelStatus = SettingPanelStatus.credit;
        }
        #endregion
    }

    public enum SettingPanelStatus
    {
        root = 0, credit = 1
    }


    public enum UISizeType
    {
        phone = 0, tablet = 1
    }
}