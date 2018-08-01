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
        }
        #endregion
    }
}