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
    public class UIManager : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public PathManager pathManager;
        public Button button_addsection;
        public Button button_restart;
        #endregion

        #region Method
        void Awake()
        {
            button_addsection.onClick.AddListener(() =>
            {
                pathManager.AddSection();
            });

            button_restart.onClick.AddListener(() =>
            {
                pm.ResetGame();
            });
        }
        #endregion
    }
}