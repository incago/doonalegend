/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class InputManager : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        #endregion

        #region Method
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            { pm.PlayerAction(PlayerInput.left); }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            { pm.PlayerAction(PlayerInput.right); }
            if (Input.GetKey(KeyCode.UpArrow))
            { pm.PlayerAction(PlayerInput.forward); }
        }
        #endregion
    }

    public enum PlayerInput
    {
        left = 0, right = 1, forward = 2, backward = 3
    }
}