/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;

namespace DoonaLegend
{
    [Serializable]
    public class CharacterModel
    {
        #region Variables
        public string characterId;
        public string name;
        public float hp;
        public float attack;
        public string skill;
        public string prefab;

        #endregion

        #region Method
        public CharacterModel(string characterId, string name, float hp, float attack, string skill, string prefab)
        {
            this.characterId = characterId;
            this.name = name;
            this.hp = hp;
            this.attack = attack;
            this.skill = skill;
            this.prefab = prefab;
        }
        #endregion
    }
}