/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class PrefabManager : MonoBehaviour
    {
        #region Variables
        private static PrefabManager _instance;
        public static PrefabManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<PrefabManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        [Header("Enemy")]
        public EnemyComponent[] enemies;
        public Dictionary<string, GameObject> enemyDictionry = new Dictionary<string, GameObject>();

        #endregion

        #region Method

        void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
                //Init();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void InitPrefabManager()
        {
            enemyDictionry.Clear();
            foreach (EnemyComponent enemy in enemies)
            {
                if (enemyDictionry.ContainsKey(enemy.enemyId))
                {
                    Debug.Log("enemyDictionry already have key: " + enemy.enemyId);
                }
                enemyDictionry.Add(enemy.enemyId, enemy.gameObject);
            }
        }

        public GameObject GetEnemyPrefab(string enemyId)
        {
            if (!enemyDictionry.ContainsKey(enemyId))
            {
                Debug.Log("enemyDictionry do have key: " + enemyId);
                return null;
            }
            return enemyDictionry[enemyId];
        }
        #endregion
    }
}