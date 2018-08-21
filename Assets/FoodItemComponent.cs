/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class FoodItemComponent : ItemComponent
    {
        #region Variables
        [Header("Food")]
        public Transform renderPosition;
        public GameObject[] foods;
        public bool isRendered = false;
        #endregion

        #region Method
        public void InitFoodRender()
        {
            if (isRendered) return;
            isRendered = true;
            GameObject foodInstance = Instantiate(foods[Random.Range(0, foods.Length)]) as GameObject;
            foodInstance.transform.SetParent(renderPosition);
            foodInstance.transform.localPosition = Vector3.zero;
        }

        #endregion
    }
}