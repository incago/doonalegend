/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class ItemComponent : MonoBehaviour
    {
        #region Variables
        private SectionComponent sectionComponent;
        public ItemType itemType;
        public float value;
        public Node origin;
        public string sfx;
        #endregion

        #region Method
        public void InitItemComponent(SectionComponent sectionComponent, Node origin)
        {
            this.sectionComponent = sectionComponent;
            this.origin = origin;

            SetItemPosition(this.origin);
        }

        void SetItemPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }
        #endregion
    }

    public enum ItemType
    {
        coin = 0, hp = 1, sp = 2, heart
    }
}