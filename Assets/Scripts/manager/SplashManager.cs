/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DoonaLegend
{
    public class SplashManager : MonoBehaviour
    {
        #region Variables
        public Canvas canvas;
        public float canvasheight = 1136.0f;
        public float canvaswidth_phone, canvaswidth_tablet;
        #endregion

        #region Method
        void Start()
        {
            UISizeType uiSizeType = GameManager.Instance.GetUISizeType();
            SetUISize(uiSizeType);
            Invoke("LoadPlayScene", 1.0f);
        }

        public void SetUISize(UISizeType uiSizeType)
        {
            if (uiSizeType == UISizeType.phone)
            {
                canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_phone, canvasheight);
            }
            else if (uiSizeType == UISizeType.tablet)
            {
                canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(canvaswidth_tablet, canvasheight);
            }
        }

        void LoadPlayScene()
        {
            SceneManager.LoadScene("play");
        }


        #endregion
    }
}