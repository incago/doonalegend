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
    public class GameOverPanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Animator animator_gameover;
        #endregion

        #region Method
        void Awake()
        {

        }

        public void ShowGameOverPanel()
        {
            animator_gameover.SetTrigger("fadein");
        }

        public void HideGameOverPanel()
        {
            animator_gameover.SetTrigger("fadeout");
        }
        #endregion
    }
}