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
        [Header("Champion")]
        public ChampionComponent[] champions;
        public Dictionary<string, GameObject> championDictionary = new Dictionary<string, GameObject>();

        [Header("Item")]
        public ItemComponent[] items;
        public Dictionary<ItemGid, GameObject> itemDictionary = new Dictionary<ItemGid, GameObject>();

        [Header("Enemy")]
        public EnemyComponent[] enemies;
        public Dictionary<EnemyGid, GameObject> enemyDictionary = new Dictionary<EnemyGid, GameObject>();

        [Header("Trap")]
        public TrapComponent[] traps;
        public Dictionary<TrapGid, GameObject> trapDictionary = new Dictionary<TrapGid, GameObject>();

        [Header("Block")]
        public BlockComponent[] blocks;
        public Dictionary<TerrainGid, GameObject> blockDictionary = new Dictionary<TerrainGid, GameObject>();

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
            championDictionary.Clear();
            enemyDictionary.Clear();
            blockDictionary.Clear();
            itemDictionary.Clear();
            foreach (ChampionComponent champion in champions)
            {
                if (championDictionary.ContainsKey(champion.championId))
                {
                    Debug.Log("championDictionary already have key: " + champion.championId);
                }
                championDictionary.Add(champion.championId, champion.gameObject);
            }

            foreach (EnemyComponent enemy in enemies)
            {
                if (enemyDictionary.ContainsKey(enemy.enemyGid))
                {
                    Debug.Log("enemyDictionry already have key: " + enemy.enemyGid);
                }
                enemyDictionary.Add(enemy.enemyGid, enemy.gameObject);
            }

            foreach (BlockComponent block in blocks)
            {
                if (blockDictionary.ContainsKey(block.terrainGid))
                {
                    Debug.Log("blockDictionary already have key: " + block.terrainGid.ToString());
                }
                blockDictionary.Add(block.terrainGid, block.gameObject);
            }

            foreach (ItemComponent item in items)
            {
                if (itemDictionary.ContainsKey(item.itemGid))
                {
                    Debug.Log("itemDictionary already have key: " + item.itemGid.ToString());
                }
                itemDictionary.Add(item.itemGid, item.gameObject);
            }

            foreach (TrapComponent trap in traps)
            {
                if (trapDictionary.ContainsKey(trap.trapGid))
                {
                    Debug.Log("trapDictionary already have key: " + trap.trapGid.ToString());
                }
                trapDictionary.Add(trap.trapGid, trap.gameObject);
            }
        }



        public GameObject GetChampionPrefab(string championId)
        {
            if (!championDictionary.ContainsKey(championId))
            {
                Debug.Log("championDictionary do have key: " + championId.ToString());
                return null;
            }
            return championDictionary[championId];
        }

        public GameObject GetEnemyPrefab(EnemyGid enemyGid)
        {
            if (!enemyDictionary.ContainsKey(enemyGid))
            {
                Debug.Log("enemyDictionary do have key: " + enemyGid.ToString());
                return null;
            }
            return enemyDictionary[enemyGid];
        }

        public GameObject GetBlockPrefab(TerrainGid terrainGid)
        {
            if (!blockDictionary.ContainsKey(terrainGid))
            {
                Debug.Log("blockDictionary do not have key: " + terrainGid.ToString());
                return null;
            }
            return blockDictionary[terrainGid];
        }

        public GameObject GetItemPrefab(ItemGid itemGid)
        {
            if (!itemDictionary.ContainsKey(itemGid))
            {
                Debug.Log("itemDictionary do not have key: " + itemGid.ToString());
                return null;
            }
            return itemDictionary[itemGid];
        }

        public GameObject GetTrapPrefab(TrapGid trapGid)
        {
            if (!trapDictionary.ContainsKey(trapGid))
            {
                Debug.Log("trapDictionry do not have key: " + trapGid.ToString());
                return null;
            }
            return trapDictionary[trapGid];
        }
        #endregion
    }
}