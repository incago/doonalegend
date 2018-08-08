/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoonaLegend
{
    public class SceneTransition : MonoBehaviour
    {
        #region Variables
        public Animator animator;
        #endregion

        #region Method
        public void FadeOut()
        {
            Debug.Log("SceneTransition.FadeOut()");
            animator.SetTrigger("fadeout");
        }

        public void FadeIn(Action callback)
        {
            Debug.Log("SceneTransition.FadeIn()");
            animator.SetTrigger("fadein");
            StartCoroutine(FadeInHelper(0.6666f, callback));
        }

        IEnumerator FadeInHelper(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }
        #endregion
    }
}