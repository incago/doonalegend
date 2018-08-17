/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoonaLegend
{
    public class SplashManager : MonoBehaviour
    {
        #region Variables
        #endregion

        #region Method
        void Start()
        {
            Invoke("LoadPlayScene", 1.0f);
        }

        void LoadPlayScene()
        {
            SceneManager.LoadScene("play");
        }


        #endregion
    }
}