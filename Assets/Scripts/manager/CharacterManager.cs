/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class CharacterManager : MonoBehaviour
    {
        #region Variables
        private static CharacterManager _instance;
        public static CharacterManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<CharacterManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        public Dictionary<string, CharacterModel> characterDictionary = new Dictionary<string, CharacterModel>();
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

        public void InitCharacterManager()
        {
            characterDictionary.Clear();
            CharacterModel doona = new CharacterModel("doona", "Doona", 100.0f, 10.0f, "empty", "doona");

            CharacterModel babarian = new CharacterModel("babarian", "Babarian", 100.0f, 10.0f, "empty", "babarian");
            CharacterModel amazon = new CharacterModel("amazon", "Amazon", 100.0f, 10.0f, "empty", "amazon");
            CharacterModel sorceress = new CharacterModel("sorceress", "Sorceress", 100.0f, 10.0f, "empty", "sorceress");
            CharacterModel necromancer = new CharacterModel("necromancer", "Necromancer", 100.0f, 10.0f, "empty", "necromancer");
            CharacterModel paladin = new CharacterModel("paladin", "Paladin", 100.0f, 10.0f, "empty", "paladin");

            characterDictionary.Add(doona.characterId, doona);

            characterDictionary.Add(babarian.characterId, babarian);
            characterDictionary.Add(amazon.characterId, amazon);
            characterDictionary.Add(sorceress.characterId, sorceress);
            characterDictionary.Add(necromancer.characterId, necromancer);
            characterDictionary.Add(paladin.characterId, paladin);
        }
        #endregion
    }
}