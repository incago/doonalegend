/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class MagmaComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public BlockComponent blockComponent;
        public bool isHot = false;
        public int attack = 2;
        #endregion

        #region Method

        //마그마 블럭이 생성되면 마그마 애니메이션이 기본적으로 동작하고 있다
        //마그마가 올라갔다 내려갔다를 반복한다
        //플레이어가 밟지 않은 마그마가 특정타이밍(올라가고 내려가고)에 플레이어를 채크할 필요는 없다
        public void SetHotFlag(int isHot)
        {
            this.isHot = isHot == 1;
            if (isHot == 1)
            {
                if (pm.champion.origin == blockComponent.blockData.origin)
                {
                    pm.champion.TakeDamage(attack, DamageType.magma);
                }
            }
        }
        #endregion
    }
}