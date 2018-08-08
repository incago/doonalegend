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
    public class HeartSlot : MonoBehaviour
    {
        #region Variables
        public Image image_bg;
        public Image image_fill;
        public Sprite sprite_heart_full, sprite_heart_half;
        #endregion

        #region Method
        public void SetHeartSlot(HeartStatus heartStatus)
        {
            image_fill.enabled = true;
            if (heartStatus == HeartStatus.empty) image_fill.enabled = false;
            else if (heartStatus == HeartStatus.half) image_fill.sprite = sprite_heart_half;
            else if (heartStatus == HeartStatus.full) image_fill.sprite = sprite_heart_full;
            else { Debug.Log("????"); }

        }
        #endregion
    }

    public enum HeartStatus
    {
        empty = 0, half = 1, full = 2
    }
}