/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INCAGO_TMX;

namespace DoonaLegend
{
    public class TrapComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        private SectionComponent sectionComponent;
        public TrapGid trapGid;
        public TrapType trapType;
        public Node origin;
        public Direction direction;
        public int attack;
        public Animator animator;
        public bool isObstacle;

        [Header("Thornfloor")]
        public bool isThornUp = false;

        [Header("Axemachine")]
        public Direction axeDirection;

        [Header("Jawmachine")]
        public bool isInvoked = false;

        // [Header("Punchmacine")]

        [Header("Crossbow")]
        public Transform fireTransform;
        public ProjectileComponent arrowPrefab;
        public float arrowSpeed = 5.0f;
        #endregion

        #region Method
        public void InitTrapComponent(SectionComponent sectionComponent, Node origin, Direction direction, TiledObject tiledObject)
        {
            animator.SetTrigger("reset");
            this.sectionComponent = sectionComponent;
            this.origin = origin;
            this.direction = direction;
            this.axeDirection = direction;

            SetTrapPosition(this.origin, this.direction);
            float delay = 0.0f;
            if (tiledObject.properties != null) { delay = tiledObject.properties.delay; }

            if (trapType == TrapType.thornfloor || trapType == TrapType.punchmachine || trapType == TrapType.crossbow)
            {
                Invoke("StartTrap", delay);
            }
        }

        void StartTrap()
        {
            animator.SetTrigger("start");
        }

        void SetTrapPosition(Node node, Direction direction)
        {
            // gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
            gameObject.transform.localPosition = Vector3.zero;
            if (direction == Direction.up) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
            else if (direction == Direction.right) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0)); }
            else if (direction == Direction.down) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0)); }
            else if (direction == Direction.left) { gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270.0f, 0)); }
        }

        public void SetThornUp(int isThornUp)
        {
            this.isThornUp = isThornUp == 1;
            if (isThornUp == 1)
            {
                if (pm.champion.origin == origin)
                {
                    pm.champion.TakeDamage(attack, DamageType.trap);
                }
            }
        }

        public void ThornfloorAttack()
        {
            pm.champion.TakeDamage(attack, DamageType.trap);
        }

        //1 = up, 2 = right, 3 = down, 4 = left
        public void AxemachineAttack(int value)
        {
            Direction attackDirection = Direction.up;
            // attackDirection = (Direction)((value + (int)this.direction - 1) % 4);
            if (this.direction == Direction.up)
            {
                attackDirection = (Direction)value;
            }
            else if (this.direction == Direction.right)
            {
                attackDirection = (Direction)((value + 1) % 4);
            }
            else if (this.direction == Direction.down)
            {
                attackDirection = (Direction)((value + 2) % 4);
            }
            else if (this.direction == Direction.left)
            {
                attackDirection = (Direction)((value + 3) % 4);
            }
            if (attackDirection == Direction.none) attackDirection = Direction.left;
            this.axeDirection = attackDirection;
            Node targetNode = origin;
            if (this.axeDirection == Direction.up) targetNode += new Node(0, 1);
            else if (this.axeDirection == Direction.right) targetNode += new Node(1, 0);
            else if (this.axeDirection == Direction.down) targetNode += new Node(0, -1);
            else if (this.axeDirection == Direction.left) targetNode += new Node(-1, 0);
            if (pm.champion.origin == targetNode)
            {
                pm.champion.TakeDamage(attack, DamageType.trap);
            }
        }

        public void BlademachineAttack()
        {
            bool isChampionInRange = false;
            for (int i = origin.x - 1; i <= origin.x + 1; i++)
            {
                for (int j = origin.y - 1; j <= origin.y + 1; j++)
                {
                    if (pm.champion.origin == new Node(i, j)) isChampionInRange = true;
                }
            }

            if (isChampionInRange)
            {
                pm.champion.TakeDamage(attack, DamageType.trap);
            }
        }

        public void JawmachineAttack()
        {
            isInvoked = true;
            pm.champion.isBitten = true;
            pm.champion.TakeDamage(attack, DamageType.trap);
            animator.SetTrigger("invoke");
        }

        public void PunchmachineAttack()
        {
            // Debug.Log("PunchmachineAttack()");
            Node targetNode = origin;
            Node knockBackNode = origin;
            // TODO: knockBackNode에 설 수 있는지 확인해야 함.
            //그 자리에 obstacle이 있다면 걍 제자리에서 데미지만 받아야 함.
            if (this.direction == Direction.up)
            {
                targetNode += new Node(0, 1);
                knockBackNode += new Node(0, 2);
            }
            else if (this.direction == Direction.right)
            {
                targetNode += new Node(1, 0);
                knockBackNode += new Node(2, 0);
            }
            else if (this.direction == Direction.down)
            {
                targetNode += new Node(0, -1);
                knockBackNode += new Node(0, -2);
            }
            else if (this.direction == Direction.left)
            {
                targetNode += new Node(-1, 0);
                knockBackNode += new Node(-2, 0);
            }
            // Debug.Log("pm.player.origin: " + pm.player.origin.ToString());
            // Debug.Log("targetNode: " + targetNode.ToString());
            if (pm.champion.origin == targetNode)
            {
                TrapComponent trapComponent = pm.pathManager.GetTrapComponent(knockBackNode);
                if (trapComponent == null || !trapComponent.isObstacle)
                {
                    pm.champion.TakeDamage(attack, DamageType.trap);
                    pm.champion.MoveChampion(targetNode, knockBackNode, 0.2f, MoveType.knockback, false);
                    //밀쳐내기
                    //기존에 이동중이라면 이동과 관련된 코루틴을 정지시키고
                    //목적지만 파라미터로 받는(출발지는 현재 플레이어의 위치)
                    //이동 함수를 사용하여 밀어낸다
                }
            }
        }

        public void CrossbowAttack()
        {
            // Debug.Log("TrapComponent.CrossbowAttack()");
            ProjectileComponent projectileComponent = Instantiate(arrowPrefab, fireTransform.position, fireTransform.rotation);
            projectileComponent.InitProjectileComponent(attack, arrowSpeed);
        }

        public void FlamemachineAttack()
        {
            Node firstNode = origin;
            Node secondNode = origin;

            if (this.direction == Direction.up)
            {
                firstNode += new Node(0, 1);
                secondNode += new Node(0, 2);
            }
            else if (this.direction == Direction.right)
            {
                firstNode += new Node(1, 0);
                secondNode += new Node(2, 0);
            }
            else if (this.direction == Direction.down)
            {
                firstNode += new Node(0, -1);
                secondNode += new Node(0, -2);
            }
            else if (this.direction == Direction.left)
            {
                firstNode += new Node(-1, 0);
                secondNode += new Node(-2, 0);
            }

            if (pm.champion.origin == firstNode || pm.champion.origin == secondNode)
            {
                pm.champion.TakeDamage(attack, DamageType.fire);
            }
        }

        #endregion
    }

    public enum TrapType
    {
        thornfloor = 0,
        axemachine = 1,
        blademachine = 2,
        jawmachine = 3,
        punchmachine = 4,
        crossbow = 5,
        flamemachine = 6
    }
}