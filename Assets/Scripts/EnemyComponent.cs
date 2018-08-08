/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class EnemyComponent : MonoBehaviour
    {
        #region Variables
        public string enemyId;
        public Node origin;
        public Direction direction;
        public Transform body;
        public Animator animator;
        public bool isDead = false;

        [Header("Stats")]
        public int maxHp;
        public int hp;
        public int attack;
        #endregion

        #region Method
        public void InitEnemyComponent(Node node, Direction direction)
        {
            this.origin = node;
            this.direction = direction;

            SetEnemyPosition(this.origin);
            SetEnemyRotation(this.direction);
        }

        void SetEnemyPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }
        void SetEnemyRotation(Direction direction)
        {
            Quaternion targetRotation = Quaternion.identity;
            if (direction == Direction.up) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (direction == Direction.right) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (direction == Direction.down) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            else if (direction == Direction.left) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
            body.rotation = targetRotation;
        }

        public bool TakeDamage(ChampionComponent player, int damage)
        {
            // Debug.Log("EnemyComponent.TakeDamage(" + damage.ToString() + ")");
            hp -= damage;
            if (hp <= 0)
            {
                hp = 0;
                isDead = true;
                return false;
            }
            else
            {
                player.TakeDamage(attack, DamageType.enemy);
                return true;
            }
        }

        #endregion
    }
}