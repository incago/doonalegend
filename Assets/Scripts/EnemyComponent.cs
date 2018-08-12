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
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public EnemyGid enemyGid;
        public Node origin;
        public Direction direction;
        public Transform body;
        public Animator animator;
        public bool isDead = false;

        [Header("Stats")]
        public int maxHp;
        public int hp;
        public int attack;

        [Header("SFX")]
        public string damageSfx;
        public string deadSfx;
        public string attackSfx;

        [Header("Etc")]
        public Vector3 canvasHudOffset;
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

        public void Attack(ChampionComponent champion)
        {
            if (!string.IsNullOrEmpty(attackSfx)) SoundManager.Instance.Play(attackSfx);
            champion.TakeDamage(attack, DamageType.enemy);
        }

        public bool TakeDamage(ChampionComponent player, int damage)
        {
            // Debug.Log("EnemyComponent.TakeDamage(" + damage.ToString() + ")");

            pm.uiManager.MakeCanvasMessageHud(gameObject.transform, "-" + damage.ToString(), canvasHudOffset, Color.red, Color.white);

            hp -= damage;
            if (hp <= 0)
            {
                if (!string.IsNullOrEmpty(deadSfx)) SoundManager.Instance.Play(deadSfx);
                hp = 0;
                isDead = true;
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(damageSfx)) SoundManager.Instance.Play(damageSfx);
                Attack(player);
                return true;
            }
        }

        #endregion
    }
}