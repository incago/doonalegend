/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class SpriteManager : MonoBehaviour
    {
        #region Variables
        private static SpriteManager _instance;
        public static SpriteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<SpriteManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        private static Dictionary<string, Sprite[]> _sprites = new Dictionary<string, Sprite[]>();
        private static Dictionary<string, string[]> _names = new Dictionary<string, string[]>();
        private static List<string> _textureMap = new List<string>();
        public bool showSpriteName = false;
        #endregion

        #region Method
        void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                //Init();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void InitSpriteManager()
        {
            // Debug.Log("SpriteManager.InitSpriteManager()");
            // _textureMap.Add("icon");


            //_textureMap.Add("weapon"); /Resources/Textures/weapon.png 파일이 존재해야 합니다. 해당 파일은 SpriteMode가 Multiple로 설정되어 있어야 합니다.

            foreach (string textureName in _textureMap)
            {
                RegisterSprite(textureName);
            }
        }

        void RegisterSprite(string textureName)
        {
            if (!_sprites.ContainsKey(textureName))
                _sprites.Add(textureName, Resources.LoadAll<Sprite>("texture/" + textureName));
            if (!_names.ContainsKey(textureName))
                _names.Add(textureName, new string[_sprites[textureName].Length]);

            for (int i = 0; i < _sprites[textureName].Length; i++)
            {
                _names[textureName][i] = _sprites[textureName][i].name;
            }
        }

        /// <summary>
        /// Sprite sprite = SpriteManager.Instance.GetSprite("icon_coin"); 과 같은 방식으로 사용하여 스프라이트를 반환받습니다.
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string spriteName)
        {
            if (showSpriteName) Debug.Log(spriteName);
            string[] name = spriteName.Split('_');
            return _sprites[name[0]][Array.IndexOf(_names[name[0]], spriteName)];
        }
        #endregion
    }
}