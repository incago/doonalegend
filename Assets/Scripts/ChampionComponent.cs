/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class ChampionComponent : MonoBehaviour
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
        public bool isAttacking = false;
        public bool isMoving = false;
        public bool isRotating = false;
        public bool isDead = false;
        public bool isWatered = false;
        public Transform body;
        public Animator animator;
        public bool isLeftLeg = false;
        public Transform explosionEffect;

        [Header("Stats")]
        public int maxHp;
        public int hp;
        public int attack;

        [Header("SP")]
        public float maxSp;
        public float sp;

        [Header("SFX")]
        public string damageSfx;
        public string deadSfx;
        public string attackSfx;

        [Header("Etc")]
        public Vector3 canvasHudOffset;

        #endregion

        #region Method
        public void InitPlayerComponent(Node node, Direction direction)
        {
            this.origin = node;
            this.direction = direction;

            // hp = maxHp = 8; //선택할수있는 캐릭터(?) 가다양해 지면 최대 hp가 달라질 수 있다
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

        public void Attack(Node playerNode, Node enemyNode, float attackDuration)
        {
            if (isAttacking) return;
            if (!string.IsNullOrEmpty(attackSfx)) SoundManager.Instance.Play(attackSfx);
            StartCoroutine(AttackHelper(playerNode, enemyNode, attackDuration));
            animator.SetTrigger("jump");
        }

        IEnumerator AttackHelper(Node playerNode, Node enemyNode, float attackDuration)
        {
            isAttacking = true;
            Vector3 initialPosition = new Vector3(playerNode.x + 0.5f, 0, playerNode.y + 0.5f);
            Vector3 targetPosition = new Vector3(enemyNode.x + 0.5f, 0, enemyNode.y + 0.5f);
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / attackDuration);
                transform.position = Vector3.Lerp(initialPosition, targetPosition, percent);
                yield return null;
            }
            EnemyComponent enemyComponent = pm.pathManager.GetEnemyComponent(enemyNode);
            if (enemyComponent.TakeDamage(this, attack))
            {

                animator.SetTrigger("jump");
                percent = 0;
                while (percent <= 1)
                {
                    percent += Time.deltaTime * (1.0f / attackDuration);
                    transform.position = Vector3.Lerp(targetPosition, initialPosition, percent);
                    yield return null;
                }
            }
            else
            {
                pm.AddKill(1);
                origin = enemyNode;
                pm.pathManager.RemoveEnemyComponent(enemyComponent);
                Destroy(enemyComponent.gameObject);

                BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(origin);
                CameraWork(currentBlockComponent);
            }

            isAttacking = false;
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
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(beforeNode);
            BlockComponent afterBlockComponent = pm.pathManager.GetBlockComponentByOrigin(afterNode);
            origin = afterNode;
            isMoving = true;
            Vector3 initialPosition = new Vector3(beforeNode.x + 0.5f, 0, beforeNode.y + 0.5f);
            Vector3 targetPosition = new Vector3(afterNode.x + 0.5f, 0, afterNode.y + 0.5f);
            if (currentBlockComponent.terrainGid == TerrainGid.water)
            {
                initialPosition += new Vector3(0, -0.083333f * 2, 0);
            }
            if (afterBlockComponent != null && afterBlockComponent.terrainGid == TerrainGid.water)
            {
                targetPosition += new Vector3(0, -0.083333f * 2, 0);
            }
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / moveDuration);
                transform.position = Vector3.Lerp(initialPosition, targetPosition, percent);
                yield return null;
            }

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

                if ((beforeBlockComponent != currentBlockComponent && (currentBlockComponent.terrainGid == TerrainGid.water)))
                {
                    isWatered = true;
                }


                // 새로운 섹션에 도착했고 해당 섹션은 스트레이트 세션이다
                // 도착한 섹션에다가 자동파괴명령을 내리자
                if (beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent)
                {
                    if (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
                    {
                        currentBlockComponent.sectionComponent.StartCollapse(1.5f, 1.5f);
                    }
                    else if (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.corner)
                    {
                        currentBlockComponent.sectionComponent.StartCollapse(1.5f, 3.0f);
                    }
                }

                if (currentBlockComponent.blockData.progress > progress)
                {
                    progress = currentBlockComponent.blockData.progress;
                    pm.AddDistance(currentBlockComponent.blockData.progress - beforeBlockComponent.blockData.progress);
                }

                // if (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
                // {
                //     currentBlockComponent.sectionComponent.StartCollapse(progress);
                // }

                ItemComponent itemComponent = pm.pathManager.GetItemComponent(this.origin);
                if (itemComponent != null)
                {
                    if (!string.IsNullOrEmpty(itemComponent.sfx))
                        SoundManager.Instance.Play(itemComponent.sfx);
                    //eat item and destroy
                    if (itemComponent.itemType == ItemType.hp)
                    {
                        AddHp((int)itemComponent.value);
                        pm.uiManager.UpdateHpUI(true);
                        pm.uiManager.MakeCanvasMessageHud(gameObject.transform, "+" + ((int)itemComponent.value).ToString(), canvasHudOffset, Color.green, Color.black);
                    }
                    else if (itemComponent.itemType == ItemType.sp)
                    {
                        AddSp(itemComponent.value);
                        pm.uiManager.UpdateSp();
                        pm.uiManager.MakeCanvasMessageHud(gameObject.transform, "+" + ((int)itemComponent.value).ToString(), canvasHudOffset, Color.magenta, Color.black);
                    }
                    else if (itemComponent.itemType == ItemType.coin)
                    {
                        pm.totalCoin += (int)itemComponent.value;
                        pm.addCoin += (int)itemComponent.value;
                        GameManager.Instance.SetPlayerCoinToPref(pm.totalCoin);
                        pm.uiManager.UpdateCoin(true);
                        pm.uiManager.MakeCanvasMessageHud(gameObject.transform, "+" + ((int)itemComponent.value).ToString(), canvasHudOffset, Color.yellow, Color.black);
                    }
                    else if (itemComponent.itemType == ItemType.heart)
                    {
                        int addHp = (int)itemComponent.value;
                        pm.player.maxHp += addHp;
                        pm.player.maxHp = Mathf.Clamp(pm.player.maxHp, 1, 32);
                        pm.player.hp += addHp;
                        pm.uiManager.InitHpUI(pm.player.maxHp, pm.player.hp, true);
                        pm.uiManager.MakeCanvasMessageHud(gameObject.transform, "Feeling Healty!", canvasHudOffset, Color.white, Color.black);
                    }
                    pm.pathManager.RemoveItemComponent(itemComponent);
                    Destroy(itemComponent.gameObject);
                }

                TrapComponent trapComponent = pm.pathManager.GetTrapComponent(this.origin);
                if (trapComponent != null)
                {
                    if (trapComponent.trapType == TrapType.thornfloor)
                    {
                        TakeDamage(trapComponent.attack, DamageType.trap);
                    }
                }
            }

            if (!isDead &&
            // !isRotate &&
            currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight &&
            currentBlockComponent.blockData.progress >= progress &&
            beforeBlockComponent != currentBlockComponent)
            {
                CameraWork(currentBlockComponent);
            }
        }

        void CameraWork(BlockComponent currentBlockComponent)
        {
            // Debug.Log("PlayerComponent.CameraWork()");
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

        public void TakeDamage(int damage, DamageType damageType)
        {
            // Debug.Log("PlayerComponent.TakeDamage(" + damage.ToString() + ")");
            if (isDead) return;

            Vector3 _canvasHudOffset = canvasHudOffset;
            if (damageType == DamageType.enemy)
            {
                _canvasHudOffset = new Vector3(0, 0, 0);
            }
            Vector3 attackPosition = new Vector3(origin.x + 0.5f, 0, origin.y + 0.5f);
            pm.uiManager.MakeCanvasMessageHud(attackPosition, "-" + damage.ToString(), _canvasHudOffset, Color.red, Color.white);

            hp -= damage;
            pm.uiManager.UpdateHpUI(true);
            if (hp <= 0)
            {
                if (!string.IsNullOrEmpty(deadSfx)) SoundManager.Instance.Play(deadSfx);

                hp = 0;
                isDead = true;
                if (damageType == DamageType.trap || damageType == DamageType.time || damageType == DamageType.enemy)
                {
                    animator.SetTrigger("dead_explosion");
                }
                else if (damageType == DamageType.drop)
                {
                    animator.SetTrigger("dead_drop");
                }
                pm.GameOver();
            }
            else
            {
                if (!string.IsNullOrEmpty(damageSfx)) SoundManager.Instance.Play(damageSfx);

            }
        }

        public void MakeExplosionEffect()
        {
            Transform effectTransform = Instantiate(explosionEffect) as Transform;
            effectTransform.position = transform.position + new Vector3(0, 1.0f, 0);
        }

        public void AddHp(int value)
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
        drop = 0, trap = 1, time = 2, enemy = 3
    }
}