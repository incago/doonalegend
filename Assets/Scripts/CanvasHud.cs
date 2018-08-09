/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace DoonaLegend
{
    public class CanvasHud : MonoBehaviour
    {
        #region Variables
        public Text text;
        public NicerOutline outline;
        public Animator animator;
        public Vector3 worldOffset;
        public Vector3 screenOffset;
        public float lifeTime = 0.5f;
        #endregion


        #region Unity Methods
        public void Init(Transform _target, string message, Vector3 localOffset, Color fontColor, Color outlineColor)
        {
            Init(_target.position, message, localOffset, fontColor, outlineColor);
        }

        public void Init(Vector3 position, string message, Vector3 localOffset, Color fontColor, Color outlineColor)
        {
            SetMessageText(message);
            SetMessageColor(fontColor, outlineColor);
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(position + worldOffset + localOffset);
            transform.position = screenPoint + screenOffset;
        }

        void Awake()
        {
            Invoke("DestroyHud", lifeTime);
        }

        public void SetMessageText(string text)
        {
            this.text.text = text;
        }
        public void SetMessageColor(Color fontColor, Color outlineColor)
        {
            text.color = fontColor;
            outline.effectColor = outlineColor;
        }

        void DestroyHud()
        {
            Destroy(gameObject);
        }

        #endregion
    }
}
