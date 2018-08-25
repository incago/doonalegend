/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        public static float championMoveDuration = 0.15f;
        #endregion

        #region Method
        void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                InitGameManager();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void InitGameManager()
        {
            Debug.Log("GameManager.InitGameManager()");
            SpriteManager.Instance.InitSpriteManager();
            CharacterManager.Instance.InitCharacterManager();
            PrefabManager.Instance.InitPrefabManager();
        }

        public int GetPlayerCoinFromPref()
        {
            return PlayerPrefs.GetInt("coin", 0);
        }

        public void SetPlayerCoinToPref(int value)
        {
            PlayerPrefs.SetInt("coin", value);
            PlayerPrefs.Save();
        }

        public UISizeType GetUISizeType()
        {
            return (UISizeType)PlayerPrefs.GetInt("uiSize", 0);
        }

        public void SetUISizeType(UISizeType uiSizeType)
        {
            PlayerPrefs.SetInt("uiSize", (int)uiSizeType);
            PlayerPrefs.Save();
        }

        public bool GetEffectActive()
        {
            return PlayerPrefs.GetInt("useEffect", 1) == 1;
        }

        public void SetEffectActive(bool value)
        {
            PlayerPrefs.SetInt("useEffect", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public bool GetPaddingActive()
        {
            return PlayerPrefs.GetInt("usePadding", 0) == 1;
        }

        public void SetPaddingActive(bool value)
        {
            PlayerPrefs.SetInt("usePadding", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public bool GetSoundActive()
        {
            return PlayerPrefs.GetInt("useSound", 1) == 1;
        }

        public void SetSoundActive(bool value)
        {
            SoundManager.Instance.SetSFXState(value);
            PlayerPrefs.SetInt("useSound", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public PlayMode GetPlayMode()
        {
            return (PlayMode)PlayerPrefs.GetInt("playMode", 1);
        }

        public void SetPlayMode(PlayMode playMode)
        {
            PlayerPrefs.SetInt("playMode", (int)playMode);
            PlayerPrefs.Save();
        }

        public int GetBestScore()
        {
            return PlayerPrefs.GetInt("bestScore", 0);
        }
        public bool SetBestScore(int value)
        {
            int currentBestScore = GetBestScore();
            if (value > currentBestScore)
            {
                PlayerPrefs.SetInt("bestScore", value);
                PlayerPrefs.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetLastPlayChampion()
        {
            return PlayerPrefs.GetString("lastPlayChampion", "doona");
        }
        public void SetLastPlayChampion(string championId)
        {
            PlayerPrefs.SetString("lastPlayChampion", championId);
            PlayerPrefs.Save();
        }
        #endregion
    }


}