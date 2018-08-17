/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INCAGO_TMX;

namespace DoonaLegend
{
    public class SectionComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public SectionModel sectionData;
        public SectionComponent beforeSectionComponent;
        public SectionComponent nextSectionComponent;
        private bool isCollapsing = false;
        public int minProgress;
        public int maxProgress;

        [Header("Block")]
        public Transform blockContainer;
        public List<BlockComponent> blockComponents = new List<BlockComponent>();

        [Header("Enemy")]
        public List<EnemyComponent> enemyComponents = new List<EnemyComponent>();

        [Header("Item")]
        public List<ItemComponent> itemComponents = new List<ItemComponent>();

        [Header("Trap")]
        public List<TrapComponent> trapComponents = new List<TrapComponent>();
        #endregion

        #region Method
        public void InitSectionComponent(SectionModel sectionData, SectionModel nextSectionData, SectionComponent lastSectionComponent)
        {
            isCollapsing = false;
            blockComponents.Clear();
            this.sectionData = sectionData;
            SetSectionPosition(this.sectionData.origin);

            int baseProgress = lastSectionComponent != null ? lastSectionComponent.maxProgress : 0;
            minProgress = baseProgress;
            int _maxProgress = -1;
            // Debug.Log("sectionData.height: " + sectionData.height.ToString());
            // Debug.Log("sectionData.width: " + sectionData.width.ToString());
            for (int i = 0; i < this.sectionData.height; i++)
            {
                for (int j = 0; j < this.sectionData.width; j++)
                {
                    TerrainGid terrainGid = TerrainGid.empty;
                    if (sectionData.terrains != null)
                    {
                        terrainGid = (TerrainGid)sectionData.terrains[i, j];
                    }
                    else
                    {
                        //terrain정보가 들어오지 않는 경우는 코너섹션일 때
                        terrainGid = TerrainGid.basic_dirt;
                    }
                    if (terrainGid == TerrainGid.empty) continue;

                    GameObject blockPrefab = PrefabManager.Instance.GetBlockPrefab(terrainGid);
                    GameObject blockGameObject = Instantiate(blockPrefab) as GameObject;
                    // GameObject blockGameObject = ObjectPool.Spawn(blockPrefab);

                    blockGameObject.name = "block#" + pm.pathManager.lastBlockComponentId.ToString();
                    blockGameObject.transform.SetParent(blockContainer);
                    BlockComponent blockComponent = blockGameObject.GetComponent<BlockComponent>();
                    BlockCategory blockCategory = this.sectionData.sectionType == SectionType.corner ? BlockCategory.corner : BlockCategory.straight;
                    if (this.sectionData.sectionType == SectionType.corner)
                    {
                        if (
                        (lastSectionComponent.sectionData.direction == Direction.up && this.sectionData.direction == Direction.right && i == this.sectionData.height - 1) ||
                        (lastSectionComponent.sectionData.direction == Direction.down && this.sectionData.direction == Direction.right && i == 0) ||
                        (this.sectionData.direction == Direction.up && j == this.sectionData.width - 1) ||
                        (this.sectionData.direction == Direction.down && j == this.sectionData.width - 1))
                        {
                            blockCategory = BlockCategory.corner_edge;
                        }
                        else
                        {
                            blockCategory = BlockCategory.corner;
                        }
                    }
                    else if (this.sectionData.sectionType == SectionType.straight)
                    {
                        //다음 코너에서 방향이 어떨게 되냐에따라 현재 스트레이트 섹션의 숏컷이 결정된다
                        if ((this.sectionData.direction == Direction.right && nextSectionData.direction == Direction.up) ||
                        (this.sectionData.direction == Direction.up && nextSectionData.direction == Direction.right))
                        {
                            if (j == this.sectionData.width - 1 && i == this.sectionData.height - 1)
                            {
                                blockCategory = BlockCategory.shortcut_start;
                            }
                            else if (this.sectionData.direction == Direction.right && j == this.sectionData.width - 1)
                            {
                                blockCategory = BlockCategory.straight_edge;
                            }
                            else if (this.sectionData.direction == Direction.up && i == this.sectionData.height - 1)
                            {
                                blockCategory = BlockCategory.straight_edge;
                            }
                            else if (j == 0 && i == 0)
                            {
                                blockCategory = BlockCategory.shortcut_end;
                            }
                            else
                            {
                                blockCategory = BlockCategory.straight;
                            }
                        }
                        else
                        {
                            if (j == this.sectionData.width - 1 && i == 0)
                            {
                                blockCategory = BlockCategory.shortcut_start;
                            }
                            else if (this.sectionData.direction == Direction.right && j == this.sectionData.width - 1)
                            {
                                blockCategory = BlockCategory.straight_edge;
                            }
                            else if (this.sectionData.direction == Direction.down && i == 0)
                            {
                                blockCategory = BlockCategory.straight_edge;
                            }
                            else if (j == 0 && i == this.sectionData.height - 1)
                            {
                                blockCategory = BlockCategory.shortcut_end;
                            }
                            else
                            {
                                blockCategory = BlockCategory.straight;
                            }
                        }
                    }
                    int progress = 0;

                    if (sectionData.sectionType == SectionType.straight)
                    {
                        if (sectionData.direction == Direction.right)
                        { progress = baseProgress + j + 1; }
                        else if (sectionData.direction == Direction.up)
                        { progress = baseProgress + i + 1; }
                        else if (sectionData.direction == Direction.down)
                        { progress = baseProgress + (this.sectionData.height - i); }
                    }
                    else if (sectionData.sectionType == SectionType.corner)
                    { progress = baseProgress + 1; }

                    if (progress > _maxProgress) _maxProgress = progress;

                    BlockModel blockData = new BlockModel(
                        pm.pathManager.lastBlockComponentId++,
                        blockCategory,
                        this.sectionData.direction,
                        this.sectionData.origin + new Node(j, i),
                        progress);

                    blockComponent.InitBlockComponent(this, blockData);
                    blockComponents.Add(blockComponent);
                    pm.pathManager.PutBlockComponent(blockComponent);
                }
            }

            if (sectionData.objects != null)
            {
                // Debug.Log("---------");
                // Debug.Log(sectionData.objects.Count);
                // Debug.Log("sectionData.direction: " + sectionData.direction);
                // Debug.Log("sectionData.width: " + sectionData.width.ToString());
                // Debug.Log("sectionData.height: " + sectionData.height.ToString());
                foreach (KeyValuePair<Node, TiledObject> kv in sectionData.objects)
                {
                    Node node = kv.Key;
                    // Debug.Log("node: " + node.ToString());
                    TiledObject tiledObject = kv.Value;
                    int objectGid = tiledObject.gid;

                    BlockComponent blockComponent = pm.pathManager.GetBlockComponentByOrigin(this.sectionData.origin + node);
                    if (blockComponent == null)
                    {
                        Debug.Log("blockComponent is null");
                        continue;
                    }

                    if (17 <= objectGid && objectGid < 33) //item
                    {
                        Node itemOrigin = this.sectionData.origin + node;
                        GameObject itemPrefab = PrefabManager.Instance.GetItemPrefab((ItemGid)objectGid);
                        // GameObject itemInstance = Instantiate(itemPrefab) as GameObject;
                        GameObject itemInstance = ObjectPool.Spawn(itemPrefab);
                        itemInstance.transform.SetParent(blockComponent.objectContainer);
                        ItemComponent itemComponent = itemInstance.GetComponent<ItemComponent>();
                        itemComponent.InitItemComponent(this, itemOrigin, blockComponent.blockData.direction);
                        itemComponents.Add(itemComponent);
                        pm.pathManager.PutItemComponent(itemComponent);
                    }
                    else if (33 <= objectGid && objectGid < 49) //trap
                    {
                        Node trapOrigin = this.sectionData.origin + node;
                        GameObject trapPrefab = PrefabManager.Instance.GetTrapPrefab((TrapGid)objectGid);
                        // GameObject trapInstance = Instantiate(trapPrefab) as GameObject;
                        GameObject trapInstance = ObjectPool.Spawn(trapPrefab);
                        trapInstance.transform.SetParent(blockComponent.objectContainer);
                        TrapComponent trapComponent = trapInstance.GetComponent<TrapComponent>();
                        Direction trapDirection = blockComponent.blockData.direction;
                        if (tiledObject.properties != null)
                        {
                            //TODO TileProperty의 direction을 그대로 사용하는것이 아니라
                            //섹션의 방향에 따라 수정된 direction을 사용해야 한다
                            if (sectionData.direction == Direction.right)
                            {
                                trapDirection = tiledObject.properties.direction;
                            }
                            else if (sectionData.direction == Direction.up)
                            {
                                if (tiledObject.properties.direction == Direction.up) { trapDirection = Direction.left; }
                                else if (tiledObject.properties.direction == Direction.right) { trapDirection = Direction.up; }
                                else if (tiledObject.properties.direction == Direction.down) { trapDirection = Direction.right; }
                                else if (tiledObject.properties.direction == Direction.left) { trapDirection = Direction.down; }
                            }
                            else if (sectionData.direction == Direction.down)
                            {
                                if (tiledObject.properties.direction == Direction.up) { trapDirection = Direction.right; }
                                else if (tiledObject.properties.direction == Direction.right) { trapDirection = Direction.down; }
                                else if (tiledObject.properties.direction == Direction.down) { trapDirection = Direction.left; }
                                else if (tiledObject.properties.direction == Direction.left) { trapDirection = Direction.up; }
                            }
                        }
                        trapComponent.InitTrapComponent(this, trapOrigin, trapDirection, tiledObject);
                        trapComponents.Add(trapComponent);
                        pm.pathManager.PutTrapComponent(trapComponent);
                    }
                    else if (49 <= objectGid && objectGid < 65) //enemy
                    {
                        Node itemOrigin = this.sectionData.origin + node;
                        GameObject enemyPrefab = PrefabManager.Instance.GetEnemyPrefab((EnemyGid)objectGid);
                        // GameObject enemyInstance = Instantiate(enemyPrefab) as GameObject;
                        GameObject enemyInstance = ObjectPool.Spawn(enemyPrefab);
                        enemyInstance.transform.SetParent(blockComponent.objectContainer);
                        EnemyComponent enemyComponent = enemyInstance.GetComponent<EnemyComponent>();
                        enemyComponent.InitEnemyComponent(itemOrigin, blockComponent.blockData.direction);
                        enemyComponents.Add(enemyComponent);
                        pm.pathManager.PutEnemyComponent(enemyComponent);
                    }
                }
            }

            this.maxProgress = _maxProgress;
        }

        public void StartCollapse(float collapseDelay, float shiveringDuration)
        {
            if (isCollapsing) return;
            isCollapsing = true;
            //자신이 가진 블럭들에게 붕괴 명령을 내려야 한다
            foreach (BlockComponent blockComponent in blockComponents)
            {
                float _collapseDelay = collapseDelay + (blockComponent.blockData.progress - minProgress) * 1.0f + UnityEngine.Random.Range(0, 0.5f);
                blockComponent.StartCollapse(_collapseDelay, shiveringDuration);
            }
        }

        public void StartCollapse(int progress)
        {
            foreach (BlockComponent blockComponent in blockComponents)
            {
                if (blockComponent.isCollapsing || blockComponent.isCollapsed) continue;
                if (blockComponent.blockData.progress <= progress)
                {
                    float collapseDelay = 1.5f + (blockComponent.blockData.progress - minProgress) * 1.0f + UnityEngine.Random.Range(0, 0.5f);
                    float shiveringDuration = 1.5f;
                    blockComponent.StartCollapse(collapseDelay, shiveringDuration);
                }
            }
        }

        public void StopCollapse()
        {
            foreach (BlockComponent blockComponent in blockComponents)
            {
                blockComponent.StopCollapse();
            }
        }

        void SetSectionPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }

        public void RemoveSection()
        {
            foreach (ItemComponent itemComponent in itemComponents)
            {
                pm.pathManager.RemoveItemComponent(itemComponent);
                ObjectPool.Recycle(itemComponent);
                // Destroy(itemComponent.gameObject);
            }

            foreach (TrapComponent trapComponent in trapComponents)
            {
                pm.pathManager.RemoveTrapComponent(trapComponent);
                ObjectPool.Recycle(trapComponent);
                // Destroy(trapComponent.gameObject);
            }

            foreach (EnemyComponent enemyComponent in enemyComponents)
            {
                pm.pathManager.RemoveEnemyComponent(enemyComponent);
                ObjectPool.Recycle(enemyComponent);
                // Destroy(enemyComponent.gameObject);
            }

            foreach (BlockComponent blockComponent in blockComponents)
            {
                pm.pathManager.RemoveBlockComponent(blockComponent);
                ObjectPool.Recycle(blockComponent);
                // Destroy(blockComponent.gameObject);
            }
            Destroy(gameObject);
        }
        #endregion
    }
}
