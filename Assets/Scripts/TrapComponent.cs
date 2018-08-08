/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class TrapComponent : MonoBehaviour
    {
        #region Variables
        private SectionComponent sectionComponent;
        public TrapType trapType;
        public Node origin;
        public int attack;
        #endregion

        #region Method
        public void InitItemComponent(SectionComponent sectionComponent, Node origin)
        {
            this.sectionComponent = sectionComponent;
            this.origin = origin;

            SetTrapPosition(this.origin);
        }

        void SetTrapPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }
        #endregion
    }

    public enum TrapType
    {
        thornfloor = 0
    }
}