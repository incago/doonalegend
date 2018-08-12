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
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public ItemGid itemGid;
        private SectionComponent sectionComponent;
        public ItemType itemType;
        public float value;
        public Node origin;
        public Direction direction;
        public string sfx;
        #endregion

        #region Method
        public void InitItemComponent(SectionComponent sectionComponent, Node origin, Direction direction)
        {
            this.sectionComponent = sectionComponent;
            this.origin = origin;
            this.direction = direction;

            SetItemPositionAndDirection(this.origin, this.direction);
        }

        void SetItemPositionAndDirection(Node node, Direction direction)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
            if (direction == Direction.up) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
            else if (direction == Direction.right) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0)); }
            else if (direction == Direction.down) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0)); }
            else if (direction == Direction.left) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270.0f, 0)); }
        }
        #endregion
    }

    public enum ItemType
    {
        coin = 0, hp = 1, sp = 2, heart = 3
    }
}