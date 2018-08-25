/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DoonaLegend
{
    public class ActionButton : MonoBehaviour, IPointerDownHandler
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        #endregion

        #region Method
        #endregion
        public void OnPointerDown(PointerEventData eventData)
        {
            // Debug.Log("Action Button down");
            pm.champion.SetAnimatorCrouch(true);
            // throw new System.NotImplementedException();
        }
    }
}