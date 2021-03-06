﻿/*
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
        public string championId;
        public Node origin;
        public Direction direction;
        public int progress;
        private Coroutine moveCoroutine;
        private Coroutine rotateCoroutine;
        public bool isAttacking = false;
        public bool isMoving = false;
        public bool isSliping = false;
        public bool isRotating = false;
        public bool isDead = false;
        public bool isWatered = false;
        public bool isBitten = false;
        public bool isInvincible = false;
        public float invincibleTime = 0.5f;
        public Transform body;
        public Animator animator;
        public bool isLeftLeg = false;
        public Transform explosionEffect;
        public bool gotItem = false;

        [Header("Damage Effect")]
        public Renderer[] bodyRenderer;
        public Material damagedMaterial;
        public Material originalMaterial;

        [Header("Stats")]
        public int maxHp;
        public int startingHp;
        public int currentHp;
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
        public void InitChampionComponent(Node node, Direction direction)
        {
            isAttacking = false;
            isMoving = false;
            isRotating = false;
            isDead = false;
            isWatered = false;
            isBitten = false;
            isInvincible = false;

            this.origin = node;
            this.direction = direction;

            // hp = maxHp = 8; //선택할수있는 캐릭터(?) 가다양해 지면 최대 hp가 달라질 수 있다
            maxSp = 100.0f;
            sp = 0;
            currentHp = startingHp;

            SetChampionPosition(this.origin);
            SetChampionRotation(this.direction);
        }

        public void SetChampionPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }
        public void SetChampionRotation(Direction direction)
        {
            Quaternion targetRotation = Quaternion.identity;
            if (direction == Direction.up) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (direction == Direction.right) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (direction == Direction.down) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            else if (direction == Direction.left) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
            body.rotation = targetRotation;
        }

        public void Attack(Node championNode, Node enemyNode, float attackDuration)
        {
            if (isAttacking) return;
            if (!string.IsNullOrEmpty(attackSfx)) SoundManager.Instance.Play(attackSfx);
            StartCoroutine(AttackHelper(championNode, enemyNode, attackDuration));
            animator.SetTrigger("jump");
        }

        IEnumerator AttackHelper(Node championNode, Node enemyNode, float attackDuration)
        {
            isAttacking = true;
            Vector3 initialPosition = new Vector3(championNode.x + 0.5f, 0, championNode.y + 0.5f);
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


                BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(origin);

                pm.pathManager.RemoveEnemyComponent(enemyComponent);
                currentBlockComponent.sectionComponent.enemyComponents.Remove(enemyComponent);
                Destroy(enemyComponent.gameObject);

                // CameraWork(currentBlockComponent);
            }

            isAttacking = false;
        }

        public void SetAnimatorCrouch(bool value)
        {
            animator.SetBool("isCrouch", value);
        }

        public void MoveChampion(Node beforeNode, Node afterNode, float moveDuration, MoveType moveType, bool isRotate = false)
        {
            // Debug.Log("ChampionComponent.MoveChampion()");
            SoundManager.Instance.Play("pop");
            if (isMoving) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveChampionHelper(beforeNode, afterNode, moveDuration, isRotate));
            if (moveType == MoveType.walk || moveType == MoveType.knockback)
            {
                isLeftLeg = !isLeftLeg;
                animator.SetBool("isLeftLeg", isLeftLeg);
                animator.SetTrigger("jump");
            }
            else if (moveType == MoveType.slip)
            {
                isSliping = true;
                //nothing
                //미끄러지는 애니메이션을 넣어야 하나?
            }
        }

        IEnumerator MoveChampionHelper(Node beforeNode, Node afterNode, float moveDuration, bool isRotate)
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
            // Debug.Log("ChampionComponent.OnMoveComplete()");
            isSliping = false;
            animator.SetBool("isCrouch", false);
            int addScore = 0;
            BlockComponent beforeBlockComponent = pm.pathManager.GetBlockComponentByOrigin(beforeNode);
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(this.origin);
            if (currentBlockComponent == null)
            {
                //there is no block blow player
                TakeDamage(currentHp, DamageType.drop);
            }
            else
            {
                if ((beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent))
                {
                    pm.pathManager.currentSectionComponent = currentBlockComponent.sectionComponent;
                }


                if ((beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent) &&
                (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight) &&
                (currentBlockComponent.blockData.progress > progress))
                {
                    //time to generate more section
                    pm.pathManager.AddSection(pm.playMode);
                }

                ItemComponent itemComponent = pm.pathManager.GetItemComponent(this.origin);
                if (itemComponent != null)
                {
                    if (!gotItem) { gotItem = true; }
                    pm.AddCombo();

                    if (!string.IsNullOrEmpty(itemComponent.sfx))
                        SoundManager.Instance.Play(itemComponent.sfx);
                    //eat item and destroy
                    if (itemComponent.itemType == ItemType.hp)
                    {
                        AddHp((int)itemComponent.value);
                        pm.uiManager.UpdateHpUI(true);
                        addScore += 2;
                        pm.score += 2;
                    }
                    else if (itemComponent.itemType == ItemType.sp)
                    {
                        AddSp(itemComponent.value);
                        pm.uiManager.UpdateSp();
                        addScore += 2;
                        pm.score += 2;
                    }
                    else if (itemComponent.itemType == ItemType.coin)
                    {
                        pm.totalCoin += (int)itemComponent.value;
                        pm.addCoin += (int)itemComponent.value;
                        GameManager.Instance.SetPlayerCoinToPref(pm.totalCoin);
                        pm.uiManager.UpdateCoin(true);
                        addScore += 1;
                        pm.score += 1;
                    }
                    else if (itemComponent.itemType == ItemType.heart)
                    {
                        int addHp = (int)itemComponent.value;
                        pm.champion.maxHp += addHp;
                        pm.champion.maxHp = Mathf.Clamp(pm.champion.maxHp, 1, 14);
                        pm.champion.currentHp += addHp;
                        pm.champion.currentHp = Mathf.Clamp(pm.champion.currentHp, 1, 14);
                        pm.uiManager.InitHpUI(pm.champion.maxHp, pm.champion.currentHp, true);
                        addScore += 5;
                        pm.score += 5;
                    }

                    pm.pathManager.RemoveItemComponent(itemComponent);
                    currentBlockComponent.sectionComponent.itemComponents.Remove(itemComponent);
                    // Destroy(itemComponent.gameObject);

                    ObjectPool.Recycle(itemComponent);
                }
                else
                {
                    pm.ResetCombo();
                    gotItem = false;
                }

                if (beforeBlockComponent != currentBlockComponent && currentBlockComponent.terrainGid == TerrainGid.water)
                {
                    isWatered = true;
                }

                if (currentBlockComponent.hasStepEffect) { currentBlockComponent.MakeStepEffect(); }

                if (currentBlockComponent.terrainGid == TerrainGid.cracked)
                {
                    currentBlockComponent.StartCollapse(1.5f, 1.5f, true);
                }
                else if (currentBlockComponent.terrainGid == TerrainGid.magma)
                {
                    MagmaComponent magmaComponent = currentBlockComponent.GetComponentInChildren<MagmaComponent>();
                    if (magmaComponent.isHot)
                    {
                        TakeDamage(magmaComponent.attack, DamageType.magma);
                    }
                }
                else if (currentBlockComponent.terrainGid == TerrainGid.ice)
                {
                    //한칸 앞으로 미끄러져 이동해야한다
                    Node targetNode = origin;
                    if (currentBlockComponent.blockData.direction == Direction.right)
                    { targetNode += new Node(1, 0); }
                    else if (currentBlockComponent.blockData.direction == Direction.up)
                    { targetNode += new Node(0, 1); }
                    else if (currentBlockComponent.blockData.direction == Direction.down)
                    { targetNode += new Node(0, -1); }
                    TrapComponent _trapComponent = pm.pathManager.GetTrapComponent(targetNode);
                    if (_trapComponent == null || !_trapComponent.isObstacle)
                    {
                        MoveChampion(origin, targetNode, GameManager.championMoveDuration, MoveType.slip, true);
                    }
                }
                else if (currentBlockComponent.terrainGid == TerrainGid.finish)
                {
                    //finish!
                    pm.pathManager.currentSectionComponent.StopCollapse();
                    pm.isClear = true;
                    gameObject.transform.SetParent(currentBlockComponent.objectContainer);
                    animator.SetTrigger("finish");
                    currentBlockComponent.Finish();
                    Vector3 targetRotation = pm.cameraController.pivot.rotation.eulerAngles + new Vector3(0, 180, 0);
                    pm.cameraController.AnimatePivotAngle(Quaternion.Euler(targetRotation), 0.5f);
                    pm.GameOver("Clear", "", true);
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

                // Debug.Log("currentBlockComponent.blockData.progress: " + currentBlockComponent.blockData.progress.ToString());
                // Debug.Log("progress: " + progress.ToString());
                if (currentBlockComponent.blockData.progress > progress)
                {
                    // Debug.Log("Here");
                    pm.AddDistance(currentBlockComponent.blockData.progress - progress, !gotItem);
                    progress = currentBlockComponent.blockData.progress;
                }

                // if (currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
                // {
                //     currentBlockComponent.sectionComponent.StartCollapse(progress);
                // }



                TrapComponent trapComponent = pm.pathManager.GetTrapComponent(this.origin);
                if (trapComponent != null)
                {
                    if (trapComponent.trapType == TrapType.thornfloor && trapComponent.isThornUp)
                    {
                        trapComponent.ThornfloorAttack();
                    }
                    else if (trapComponent.trapType == TrapType.jawmachine && !trapComponent.isInvoked)
                    {
                        trapComponent.JawmachineAttack();
                    }
                    else if (trapComponent.trapType == TrapType.pushpad)
                    {
                        trapComponent.PushpadAction(this, origin, beforeNode);
                    }
                }
            }

            if (!isDead &&
            // !isRotate &&
            currentBlockComponent != null &&
            currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight &&
            currentBlockComponent.blockData.progress >= progress &&
            beforeBlockComponent != currentBlockComponent)
            {
                // CameraWork(currentBlockComponent);
            }
            if (isRotate)
            {
                // Debug.Log("champion rotate");
                Quaternion targetRotation = Quaternion.identity;
                if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.right)
                {
                    if (currentBlockComponent.sectionComponent.beforeSectionComponent == null)
                    {
                        if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                        { targetRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
                        else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                        { targetRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                    }
                    else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent == null)
                    {
                        if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                        { targetRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
                        else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                        { targetRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                    }
                    else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.down)
                    { targetRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                    else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.up)
                    { targetRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
                }
                else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.up)
                {
                    targetRotation = Quaternion.Euler(pm.cameraController.championUpAngle);
                }
                else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.down)
                {
                    targetRotation = Quaternion.Euler(pm.cameraController.championDownAngle);
                }
                pm.cameraController.AnimatePivotAngle(targetRotation, 0.3f);
            }
            if (addScore > 0)
            {
                pm.uiManager.UpdateScoreUI(pm.score, addScore);
            }
        }

        void CameraWork(BlockComponent currentBlockComponent)
        {
            // Debug.Log("ChampionComponent.CameraWork()");
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
                    { initialRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
                    else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                    { initialRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                }
                else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent == null)
                {
                    if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                    { initialRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
                    else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                    { initialRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                }
                else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.down)
                { initialRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }
                else if (currentBlockComponent.sectionComponent.beforeSectionComponent.beforeSectionComponent.sectionData.direction == Direction.up)
                { initialRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }

                if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                { targetRotation = Quaternion.Euler(pm.cameraController.championUpAngle); }
                else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                { targetRotation = Quaternion.Euler(pm.cameraController.championDownAngle); }
            }
            else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.up)
            {
                initialRotation = Quaternion.Euler(pm.cameraController.championUpAngle);
                targetRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle);
            }
            else if (currentBlockComponent.sectionComponent.sectionData.direction == Direction.down)
            {
                initialRotation = Quaternion.Euler(pm.cameraController.championDownAngle);
                targetRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle);
            }

            pm.cameraController.AnimatePivotAngle(initialRotation, targetRotation, startPercent, endPercent, 0.3f);
        }


        public void RotateChampion(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            // Debug.Log("ChampionComponent.RotateChampion(" + afterDirection.ToString() + ")");
            rotateCoroutine = StartCoroutine(RotateChampionHelper(beforeDirection, afterDirection, rotateDuration));
            // pm.cameraController.SetPivotAngle(afterDirection);
        }

        IEnumerator RotateChampionHelper(Direction beforeDirection, Direction afterDirection, float rotateDuration)
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
            // Debug.Log("ChampionComponent.TakeDamage(" + damage.ToString() + ", " + damageType.ToString() + ")");
            if (isDead) return;
            if (isInvincible && damageType != DamageType.drop) return;
            else
            {
                isInvincible = true;
                Invoke("TakeDamageHelper", invincibleTime);
            }
            DamageEffect();
            Vector3 _canvasHudOffset = canvasHudOffset;
            if (damageType == DamageType.enemy)
            {
                _canvasHudOffset = new Vector3(0, 0, 0);
            }
            Vector3 attackPosition = new Vector3(origin.x + 0.5f, 0, origin.y + 0.5f);
            pm.uiManager.MakeCanvasMessageHud(attackPosition, "-" + damage.ToString(), _canvasHudOffset, Color.red, Color.white);

            currentHp -= damage;
            pm.uiManager.UpdateHpUI(true);
            if (currentHp <= 0)
            {
                if (!string.IsNullOrEmpty(deadSfx)) SoundManager.Instance.Play(deadSfx);

                currentHp = 0;
                isDead = true;
                pm.pathManager.currentSectionComponent.StopCollapse();
                if (damageType == DamageType.trap ||
                damageType == DamageType.time ||
                damageType == DamageType.enemy ||
                damageType == DamageType.magma ||
                damageType == DamageType.projectile ||
                damageType == DamageType.fire)
                {
                    animator.SetTrigger("dead_explosion");
                }
                else if (damageType == DamageType.drop)
                {
                    animator.SetTrigger("dead_drop");
                }
                pm.GameOver("Game", "Over");
            }
            else
            {
                if (!string.IsNullOrEmpty(damageSfx)) SoundManager.Instance.Play(damageSfx);

            }
        }

        void TakeDamageHelper()
        {
            isInvincible = false;
        }

        public void DamageEffect()
        {
            StartCoroutine(DamageEffectCo(invincibleTime));
        }

        IEnumerator DamageEffectCo(float invincibleTime)
        {
            //Debug.Log("DamageEffectCo");
            float blinkTime = 0.05f;
            while (0 <= invincibleTime)
            {
                SetDamageMaterial();
                yield return new WaitForSeconds(blinkTime);
                SetOriginalMaterial();
                yield return new WaitForSeconds(blinkTime);
                invincibleTime -= (blinkTime * 2);
            }
        }

        void SetDamageMaterial()
        {
            for (int i = 0; i < bodyRenderer.Length; i++)
            {
                if (bodyRenderer[i] != null)
                    bodyRenderer[i].material = damagedMaterial;
            }
        }

        void SetOriginalMaterial()
        {
            // Debug.Log("UnitFace.SetOriginalMaterial()");
            for (int i = 0; i < bodyRenderer.Length; i++)
            {
                if (bodyRenderer[i] != null)
                    bodyRenderer[i].material = originalMaterial;
                // bodyRenderer [i].material = originalMaterial;
            }
        }

        public void MakeExplosionEffect()
        {
            Transform effectTransform = Instantiate(explosionEffect) as Transform;
            effectTransform.position = transform.position + new Vector3(0, 1.0f, 0);
        }

        public void AddHp(int value)
        {
            currentHp += value;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);
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

    public enum MoveType
    {
        walk = 0, slip = 1, knockback = 2
    }

    public enum DamageType
    {
        drop = 0, trap = 1, time = 2, enemy = 3, magma = 4, projectile = 5, fire = 6
    }
}