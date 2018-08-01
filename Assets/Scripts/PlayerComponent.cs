/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class PlayerComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Node origin;
        public Direction direction;
        public int progress;
        private Coroutine moveCoroutine;
        private Coroutine rotateCoroutine;
        public bool isMoving = false;
        public bool isRotating = false;
        public bool isDead = false;
        public Transform body;
        public Animator animator;

        [Header("HP")]
        public float maxHp;
        public float hp;

        #endregion

        #region Method
        public void InitPlayerComponent(Node node, Direction direction)
        {
            this.origin = node;
            this.direction = direction;

            hp = maxHp = 100.0f; //선택할수있는 캐릭터(?) 가다양해 지면 최대 hp가 달라질 수 있다

            SetPlayerPosition(this.origin);
        }

        void SetPlayerPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }

        public void MovePlayer(Node beforeNode, Node afterNode, float moveDuration)
        {
            moveCoroutine = StartCoroutine(MovePlayerHelper(beforeNode, afterNode, moveDuration));
            animator.SetTrigger("jump");
        }

        IEnumerator MovePlayerHelper(Node beforeNode, Node afterNode, float moveDuration)
        {
            isMoving = true;
            Vector3 initialPosition = new Vector3(beforeNode.x + 0.5f, 0, beforeNode.y + 0.5f);
            Vector3 targetPosition = new Vector3(afterNode.x + 0.5f, 0, afterNode.y + 0.5f);
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / moveDuration);
                transform.position = Vector3.Lerp(initialPosition, targetPosition, percent);
                yield return null;
            }

            origin = afterNode;
            isMoving = false;
            OnMoveComplete(beforeNode);
            //if need callback here

        }
        void OnMoveComplete(Node beforeNode)
        {
            BlockComponent beforeBlockComponent = pm.pathManager.GetBlockComponentByOrigin(beforeNode);
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(this.origin);
            if (currentBlockComponent == null)
            {
                //there is no block blow player
                TakeDamage(hp);
                animator.SetTrigger("dead_drop");
            }
            else
            {
                if (currentBlockComponent.blockData.progress > progress)
                {
                    progress = currentBlockComponent.blockData.progress;
                    pm.AddScore(currentBlockComponent.blockData.progress - beforeBlockComponent.blockData.progress);
                }
                if (beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent && currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
                {
                    //time to generate more section
                    pm.pathManager.AddSection();
                }

                ItemComponent itemComponent = pm.pathManager.GetItemComponent(this.origin);
                if (itemComponent != null)
                {
                    //eat item and destroy
                    if (itemComponent.itemType == ItemType.potion_hp)
                    {
                        AddHp(5.0f);
                        pm.uiManager.UpdateHp();
                    }
                    else if (itemComponent.itemType == ItemType.coin)
                    {
                        //add coin
                        pm.uiManager.UpdateCoin();
                    }
                    pm.pathManager.RemoveItemComponent(itemComponent);
                    Destroy(itemComponent.gameObject);
                }
            }
        }

        public void RotatePlayer(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            // Debug.Log("PlayerComponent.RotatePlayer(" + afterDirection.ToString() + ")");
            rotateCoroutine = StartCoroutine(RotatePlayerHelper(beforeDirection, afterDirection, rotateDuration));
            pm.cameraController.SetPivotAngle(afterDirection);
        }

        IEnumerator RotatePlayerHelper(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            isRotating = true;
            Quaternion initialRotation = body.rotation;
            Quaternion targetRotation = Quaternion.identity;
            if (afterDirection == Direction.up) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
            else if (afterDirection == Direction.right) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (afterDirection == Direction.down) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (afterDirection == Direction.left) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / rotateDuration);
                body.rotation = Quaternion.Lerp(initialRotation, targetRotation, percent);
                yield return null;
            }

            direction = afterDirection;
            isRotating = false;
            //if need callback here
        }

        public void TakeDamage(float damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                hp = 0;
                isDead = true;
            }
        }

        public void AddHp(float value)
        {
            hp += value;
            hp = Mathf.Clamp(hp, 0, maxHp);
        }

        #endregion
    }
}