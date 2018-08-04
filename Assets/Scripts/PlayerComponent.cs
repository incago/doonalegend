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
        public bool isLeftLeg = false;
        public Transform explosionEffect;

        [Header("HP")]
        public float maxHp;
        public float hp;

        [Header("SP")]
        public float maxSp;
        public float sp;

        #endregion

        #region Method
        public void InitPlayerComponent(Node node, Direction direction)
        {
            this.origin = node;
            this.direction = direction;

            hp = maxHp = 100.0f; //선택할수있는 캐릭터(?) 가다양해 지면 최대 hp가 달라질 수 있다
            maxSp = 100.0f;
            sp = 0;

            SetPlayerPosition(this.origin);
            SetPlayerRotation(this.direction);
        }

        void SetPlayerPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }
        void SetPlayerRotation(Direction direction)
        {
            Quaternion targetRotation = Quaternion.identity;
            if (direction == Direction.up) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (direction == Direction.right) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (direction == Direction.down) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            else if (direction == Direction.left) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
            body.rotation = targetRotation;
        }

        public void MovePlayer(Node beforeNode, Node afterNode, float moveDuration, bool isRotate = false)
        {
            moveCoroutine = StartCoroutine(MovePlayerHelper(beforeNode, afterNode, moveDuration, isRotate));
            isLeftLeg = !isLeftLeg;
            animator.SetBool("isLeftLeg", isLeftLeg);
            animator.SetTrigger("jump");
        }

        IEnumerator MovePlayerHelper(Node beforeNode, Node afterNode, float moveDuration, bool isRotate)
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
            OnMoveComplete(beforeNode, isRotate);
            //if need callback here

        }
        void OnMoveComplete(Node beforeNode, bool isRotate)
        {
            BlockComponent beforeBlockComponent = pm.pathManager.GetBlockComponentByOrigin(beforeNode);
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(this.origin);
            if (currentBlockComponent == null)
            {
                //there is no block blow player
                TakeDamage(hp, DamageType.drop);
            }
            else
            {
                if ((beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent) &&
                (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight) &&
                (currentBlockComponent.blockData.progress > progress))
                {
                    //time to generate more section
                    pm.pathManager.AddSection();
                }

                // if ((beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent) &&
                // (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.corner))
                // {
                //     pm.cameraController.SetPivotAngle(currentBlockComponent.sectionComponent.sectionData.direction);
                // }


                if (currentBlockComponent.blockData.progress > progress)
                {
                    progress = currentBlockComponent.blockData.progress;
                    pm.AddScore(currentBlockComponent.blockData.progress - beforeBlockComponent.blockData.progress);
                }

                ItemComponent itemComponent = pm.pathManager.GetItemComponent(this.origin);
                if (itemComponent != null)
                {
                    //eat item and destroy
                    if (itemComponent.itemType == ItemType.potion_hp)
                    {
                        AddHp(10.0f);
                        pm.uiManager.UpdateHp();
                    }
                    else if (itemComponent.itemType == ItemType.potion_sp)
                    {
                        AddSp(10.0f);
                        pm.uiManager.UpdateSp();
                    }
                    else if (itemComponent.itemType == ItemType.coin)
                    {
                        pm.totalCoin += 1;
                        pm.addCoin += 1;
                        GameManager.Instance.SetPlayerCoinToPref(pm.totalCoin);
                        pm.uiManager.UpdateCoin();
                    }
                    pm.pathManager.RemoveItemComponent(itemComponent);
                    Destroy(itemComponent.gameObject);
                }

                TrapComponent trapComponent = pm.pathManager.GetTrapComponent(this.origin);
                if (trapComponent != null)
                {
                    if (trapComponent.trapType == TrapType.thornfloor)
                    {
                        TakeDamage(50.0f, DamageType.trap);
                        pm.uiManager.UpdateHp();
                    }
                }
            }

            if (!isDead &&
            // !isRotate &&
            currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
            {
                int currentProgress = currentBlockComponent.blockData.progress;
                int minProgressInSection = currentBlockComponent.sectionComponent.minProgress;
                int maxProgressInSection = currentBlockComponent.sectionComponent.maxProgress;
                float startPercent = (float)(currentProgress - minProgressInSection - 1) / (float)(maxProgressInSection - minProgressInSection);
                float endPercent = (float)(currentProgress - minProgressInSection) / (float)(maxProgressInSection - minProgressInSection);
                Quaternion initialRotation = Quaternion.identity;
                Quaternion targetRotation = Quaternion.identity;
                if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.right)
                {
                    if (currentBlockComponent.sectionComponent.beforeSectionComponent == null)
                    {
                        if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                        { initialRotation = Quaternion.Euler(pm.cameraController.playerRightUpAngle); }
                        else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                        { initialRotation = Quaternion.Euler(pm.cameraController.playerRightDownAngle); }
                    }
                    else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.down)
                    { initialRotation = Quaternion.Euler(pm.cameraController.playerRightDownAngle); }
                    else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.up)
                    { initialRotation = Quaternion.Euler(pm.cameraController.playerRightUpAngle); }

                    if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                    { targetRotation = Quaternion.Euler(pm.cameraController.playerUpAngle); }
                    else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                    { targetRotation = Quaternion.Euler(pm.cameraController.playerDownAngle); }
                }
                else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.up)
                {
                    initialRotation = Quaternion.Euler(pm.cameraController.playerUpAngle);
                    targetRotation = Quaternion.Euler(pm.cameraController.playerRightUpAngle);
                }
                else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.down)
                {
                    initialRotation = Quaternion.Euler(pm.cameraController.playerDownAngle);
                    targetRotation = Quaternion.Euler(pm.cameraController.playerRightDownAngle);
                }



                pm.cameraController.AnimatePivotAngle(initialRotation, targetRotation, startPercent, endPercent, 0.3f);
            }
        }


        public void RotatePlayer(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            // Debug.Log("PlayerComponent.RotatePlayer(" + afterDirection.ToString() + ")");
            rotateCoroutine = StartCoroutine(RotatePlayerHelper(beforeDirection, afterDirection, rotateDuration));
            // pm.cameraController.SetPivotAngle(afterDirection);
        }

        IEnumerator RotatePlayerHelper(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            isRotating = true;
            Quaternion initialRotation = body.rotation;
            Quaternion targetRotation = Quaternion.identity;
            if (afterDirection == Direction.up) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (afterDirection == Direction.right) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (afterDirection == Direction.down) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            else if (afterDirection == Direction.left) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
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

        public void TakeDamage(float damage, DamageType damageType)
        {
            hp -= damage;
            if (hp <= 0)
            {
                hp = 0;
                isDead = true;
                if (damageType == DamageType.trap || damageType == DamageType.time)
                {
                    animator.SetTrigger("dead_explosion");
                }
                else if (damageType == DamageType.drop)
                {
                    animator.SetTrigger("dead_drop");
                }
                pm.GameOver();
            }
        }

        public void MakeExplosionEffect()
        {
            Transform effectTransform = Instantiate(explosionEffect) as Transform;
            effectTransform.position = transform.position + new Vector3(0, 1.0f, 0);
        }

        public void AddHp(float value)
        {
            hp += value;
            hp = Mathf.Clamp(hp, 0, maxHp);
        }

        public void AddSp(float value)
        {
            sp += value;
            sp = Mathf.Clamp(sp, 0, maxSp);
        }

        public void UseSp(float value)
        {
            sp -= value;
            sp = Mathf.Clamp(sp, 0, maxSp);
        }

        #endregion
    }

    public enum DamageType
    {
        drop = 0, trap = 1, time = 2
    }
}