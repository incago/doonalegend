/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class ChampionPreview : MonoBehaviour
    {
        #region Variables
        public string championId;
        public int championIndex;
        public int maxHp, startHp;
        public Renderer championRenderer;
        public Animator animator;
        public Transform body;
        public float scaleDuration = 0.2f;
        // private float bodySize = 1.0f;
        private Coroutine scaleDownCoroutine;
        private Coroutine scaleUpCoroutine;
        #endregion

        #region Method
        public void ScaleDownChampionPreview()
        {
            if (scaleUpCoroutine != null) StopCoroutine(scaleUpCoroutine);
            if (scaleDownCoroutine != null) StopCoroutine(scaleDownCoroutine);
            scaleDownCoroutine = StartCoroutine(ScaleDownChampionPreviewHelper());
        }

        IEnumerator ScaleDownChampionPreviewHelper()
        {
            float percent = 0;
            Vector3 initialScale = body.localScale;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / ((body.localScale.x - 1.0f) * scaleDuration));
                body.localScale = Vector3.Lerp(initialScale, Vector3.one, percent);
                yield return null;
            }
        }

        public void SclaeUpChampionPreview()
        {
            if (scaleUpCoroutine != null) StopCoroutine(scaleUpCoroutine);
            if (scaleDownCoroutine != null) StopCoroutine(scaleDownCoroutine);
            scaleUpCoroutine = StartCoroutine(SclaeUpChampionPreviewHelper());
        }

        IEnumerator SclaeUpChampionPreviewHelper()
        {
            float percent = 0;
            Vector3 initialScale = body.localScale;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / ((2.0f - body.localScale.x) * scaleDuration));
                body.localScale = Vector3.Lerp(initialScale, new Vector3(2.0f, 2.0f, 2.0f), percent);
                yield return null;
            }
        }
        #endregion
    }
}