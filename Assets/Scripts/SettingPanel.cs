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

        [Header("UI Size")]
        public Button button_uisize;
        public Text text_uisize;
        public Image image_uisize;
        public Sprite sprite_uisize_phone, sprite_uisize_tablet;
        public float canvasheight = 1136.0f;
        public float canvaswidth_phone, canvaswidth_tablet;

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

        [Header("Effect")]
        public Button button_effect;
        public Text text_effect;
        public Image image_effect;
        public Sprite sprite_effect_on, sprite_effect_off;
        #endregion

        #region Method
        void Awake()
        {
            text_version.text = "Version " + Application.version;
            UISizeType uiSizeType = GameManager.Instance.GetUISizeType();
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

            bool usePadding = GameManager.Instance.GetPaddingActive();
            text_padding.text = "Padding - " + (usePadding ? "On" : "Off");
            image_padding.sprite = usePadding ? sprite_padding_on : sprite_padding_off;

            button_back.onClick.AddListener(() => { OnBackButtonClick(); });
            button_uisize.onClick.AddListener(() => { OnUISizeButtonClick(); });
            button_effect.onClick.AddListener(() => { OnEffectButtonClick(); });
            button_padding.onClick.AddListener(() => { OnPaddingButtonClick(); });
            button_sound.onClick.AddListener(() => { OnSoundButtonClick(); });
        }

        public void OpenSettingPanel(PlaySceneState from)
        {
            this.from = from;
            this.settingPanelStatus = SettingPanelStatus.main;
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
            if (settingPanelStatus == SettingPanelStatus.main)
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
            // else if (settingPanelStatus == SettingPanelStatus.main)
            // {

            // }
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
            GameManager.Instance.SetUISizeType(targetUISizeType);
            SetUISize(targetUISizeType);
        }

        public void SetUISize(UISizeType uiSizeType)
        {
            if (uiSizeType == UISizeType.phone)
            {
                pm.uiManager.canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_phone, canvasheight);
                pm.cameraController.mainCamera.orthographicSize = pm.cameraController.camerasize_phone;

            }
            else if (uiSizeType == UISizeType.tablet)
            {
                pm.uiManager.canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_tablet, canvasheight);
                pm.cameraController.mainCamera.orthographicSize = pm.cameraController.camerasize_tablet;
            }
        }

        void OnEffectButtonClick()
        {
            bool useEffect = !GameManager.Instance.GetEffectActive();
            text_effect.text = "Effects - " + (useEffect ? "On" : "Off");
            image_effect.sprite = useEffect ? sprite_effect_on : sprite_effect_off;
            GameManager.Instance.SetEffectActive(useEffect);
        }

        void OnPaddingButtonClick()
        {
            bool usePadding = !GameManager.Instance.GetPaddingActive();
            text_padding.text = "Padding - " + (usePadding ? "On" : "Off");
            image_padding.sprite = usePadding ? sprite_padding_on : sprite_padding_off;
            GameManager.Instance.SetPaddingActive(usePadding);
        }

        void OnSoundButtonClick()
        {
            bool useSound = !GameManager.Instance.GetSoundActive();
            text_sound.text = "Sound - " + (useSound ? "On" : "Off");
            image_sound.sprite = useSound ? sprite_sound_on : sprite_sound_off;
            GameManager.Instance.SetSoundActive(useSound);
        }
        #endregion
    }

    public enum SettingPanelStatus
    {
        main
    }


    public enum UISizeType
    {
        phone = 0, tablet = 1
    }
}