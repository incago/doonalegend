/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class ProjectileComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }

        public int attack;
        public float speed;
        public float timeToAlive = 5.0f;

        #endregion

        #region Method
        public void InitProjectileComponent(int attack, float speed)
        {
            this.attack = attack;
            this.speed = speed;
        }

        void OnEnable()
        {
            Invoke("AutoDestroy", timeToAlive);
        }

        void Update()
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Champion"))
            {
                ChampionComponent championComponent = other.GetComponent<ChampionComponent>();
                championComponent.TakeDamage(attack, DamageType.projectile);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Trap") || other.CompareTag("Obstacle"))
            {
                Destroy(gameObject);
            }
        }

        void AutoDestroy()
        {
            // ObjectPool.Recycle(gameObject);
            Destroy(gameObject);
        }

        #endregion
    }
}