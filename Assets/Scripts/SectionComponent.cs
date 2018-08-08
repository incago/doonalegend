/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //TODO : prefab manager
        [Header("Block")]
        public Transform blockContainer;
        public GameObject grassBlockPrefab, dirtBlockPrefab;
        public List<BlockComponent> blockComponents = new List<BlockComponent>();

        [Header("Enemy")]
        public List<EnemyComponent> enemyComponents = new List<EnemyComponent>();

        [Header("Item")]
        public Transform heartPrefab;
        public Transform coinPrefab;
        public Transform hpPotionPrefab;
        public Transform spPotionPrefab;
        // public Transform itemContainer;
        public List<ItemComponent> itemComponents = new List<ItemComponent>();

        [Header("Trap")]
        public Transform thornFloorPrefab;
        // public Transform trapContainer;
        public List<TrapComponent> trapComponents = new List<TrapComponent>();
        public int minProgress;
        public int maxProgress;
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
            for (int i = 0; i < this.sectionData.height; i++)
            {
                for (int j = 0; j < this.sectionData.width; j++)
                {
                    int randomNumber = Random.Range(0, 100);
                    if (sectionData.sectionType == SectionType.straight && randomNumber < 2 && sectionData.sectionId != 0) //2%확률로 구멍
                    {
                        // continue;
                    }

                    GameObject blockPrefab = null;
                    if (sectionData.sectionType == SectionType.straight)
                    {
                        blockPrefab = grassBlockPrefab;
                    }
                    else if (sectionData.sectionType == SectionType.corner)
                    {
                        blockPrefab = dirtBlockPrefab;
                    }
                    GameObject blockGameObject = Instantiate(blockPrefab) as GameObject;
                    blockGameObject.name = "block#" + pm.pathManager.lastBlockComponentId.ToString();
                    blockGameObject.transform.SetParent(blockContainer);
                    BlockComponent blockComponent = blockGameObject.GetComponent<BlockComponent>();
                    BlockType blockType = this.sectionData.sectionType == SectionType.corner ? BlockType.corner : BlockType.straight;
                    if (this.sectionData.sectionType == SectionType.corner)
                    {
                        if (
                        (lastSectionComponent.sectionData.direction == Direction.up && this.sectionData.direction == Direction.right && i == this.sectionData.height - 1) ||
                        (lastSectionComponent.sectionData.direction == Direction.down && this.sectionData.direction == Direction.right && i == 0) ||
                        (this.sectionData.direction == Direction.up && j == this.sectionData.width - 1) ||
                        (this.sectionData.direction == Direction.down && j == this.sectionData.width - 1))
                        {
                            blockType = BlockType.corner_edge;
                        }
                        else
                        {
                            blockType = BlockType.corner;
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
                                blockType = BlockType.shortcut_start;
                            }
                            else if (this.sectionData.direction == Direction.right && j == this.sectionData.width - 1)
                            {
                                blockType = BlockType.straight_edge;
                            }
                            else if (this.sectionData.direction == Direction.up && i == this.sectionData.height - 1)
                            {
                                blockType = BlockType.straight_edge;
                            }
                            else if (j == 0 && i == 0)
                            {
                                blockType = BlockType.shortcut_end;
                            }
                            else
                            {
                                blockType = BlockType.straight;
                            }
                        }
                        else
                        {
                            if (j == this.sectionData.width - 1 && i == 0)
                            {
                                blockType = BlockType.shortcut_start;
                            }
                            else if (this.sectionData.direction == Direction.right && j == this.sectionData.width - 1)
                            {
                                blockType = BlockType.straight_edge;
                            }
                            else if (this.sectionData.direction == Direction.down && i == 0)
                            {
                                blockType = BlockType.straight_edge;
                            }
                            else if (j == 0 && i == this.sectionData.height - 1)
                            {
                                blockType = BlockType.shortcut_end;
                            }
                            else
                            {
                                blockType = BlockType.straight;
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
                        blockType, this.sectionData.direction,
                        this.sectionData.origin + new Node(j, i),
                        progress);

                    blockComponent.InitBlockComponent(this, blockData);
                    blockComponents.Add(blockComponent);
                    pm.pathManager.PutBlockComponent(blockComponent);

                    if (sectionData.sectionId == 0) continue; //첫번째 섹션에는 아무것도 만들지 않는다

                    if (sectionData.sectionType == SectionType.straight)
                    {
                        //for test
                        if (j == 2 && i == 1)
                        {
                            Node enemyOrigin = this.sectionData.origin + new Node(j, i);
                            GameObject enemyInstance = Instantiate(PrefabManager.Instance.GetEnemyPrefab("slime")) as GameObject;
                            enemyInstance.transform.SetParent(blockComponent.itemContainer);
                            EnemyComponent enemyComponent = enemyInstance.GetComponent<EnemyComponent>();
                            enemyComponent.InitEnemyComponent(enemyOrigin, blockData.direction);
                            enemyComponents.Add(enemyComponent);
                            pm.pathManager.PutEnemyComponent(enemyComponent);
                            continue;
                        }

                        if (randomNumber < 2)
                        {
                            Node itemOrigin = this.sectionData.origin + new Node(j, i);
                            Transform hpPotionTransform = Instantiate(heartPrefab) as Transform;
                            hpPotionTransform.SetParent(blockComponent.itemContainer);
                            ItemComponent itemComponent = hpPotionTransform.GetComponent<ItemComponent>();
                            itemComponent.InitItemComponent(this, itemOrigin);
                            itemComponents.Add(itemComponent);
                            pm.pathManager.PutItemComponent(itemComponent);
                        }
                        else if (randomNumber < 5)
                        {
                            Node itemOrigin = this.sectionData.origin + new Node(j, i);
                            Transform hpPotionTransform = Instantiate(hpPotionPrefab) as Transform;
                            hpPotionTransform.SetParent(blockComponent.itemContainer);
                            ItemComponent itemComponent = hpPotionTransform.GetComponent<ItemComponent>();
                            itemComponent.InitItemComponent(this, itemOrigin);
                            itemComponents.Add(itemComponent);
                            pm.pathManager.PutItemComponent(itemComponent);
                        }
                        else if (randomNumber < 10)
                        {
                            Node itemOrigin = this.sectionData.origin + new Node(j, i);
                            Transform spPotionTransform = Instantiate(spPotionPrefab) as Transform;
                            spPotionTransform.SetParent(blockComponent.itemContainer);
                            ItemComponent itemComponent = spPotionTransform.GetComponent<ItemComponent>();
                            itemComponent.InitItemComponent(this, itemOrigin);
                            itemComponents.Add(itemComponent);
                            pm.pathManager.PutItemComponent(itemComponent);
                        }
                        else if (randomNumber < 20)
                        {
                            Node itemOrigin = this.sectionData.origin + new Node(j, i);
                            Transform coinTransform = Instantiate(coinPrefab) as Transform;
                            coinTransform.SetParent(blockComponent.itemContainer);
                            ItemComponent itemComponent = coinTransform.GetComponent<ItemComponent>();
                            itemComponent.InitItemComponent(this, itemOrigin);
                            itemComponents.Add(itemComponent);
                            pm.pathManager.PutItemComponent(itemComponent);
                        }
                        else if (randomNumber < 22 && sectionData.sectionId != 0)
                        {
                            Node trapOrigin = this.sectionData.origin + new Node(j, i);
                            Transform trapTransform = Instantiate(thornFloorPrefab) as Transform;
                            trapTransform.SetParent(blockComponent.trapContainer);
                            TrapComponent trapComponent = trapTransform.GetComponent<TrapComponent>();
                            trapComponent.InitItemComponent(this, trapOrigin);
                            trapComponents.Add(trapComponent);
                            pm.pathManager.PutTrapComponent(trapComponent);
                        }
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
                float _collapseDelay = collapseDelay + (blockComponent.blockData.progress - minProgress) * 1.0f + Random.Range(0, 0.5f);
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
                    float collapseDelay = 1.5f + (blockComponent.blockData.progress - minProgress) * 1.0f + Random.Range(0, 0.5f);
                    float shiveringDuration = 1.5f;
                    blockComponent.StartCollapse(collapseDelay, shiveringDuration);
                }
            }
        }

        void SetSectionPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }

        public void RemoveSection()
        {
            //TODO : recycle block gameObject and destroy section gameObject
            foreach (BlockComponent blockComponent in blockComponents)
            {
                pm.pathManager.RemoveBlockComponent(blockComponent);
                //TODO : recycle block gameObject
            }

            foreach (ItemComponent itemComponent in itemComponents)
            {
                pm.pathManager.RemoveItemComponent(itemComponent);
                //TODO : recycle item gameObject
            }

            foreach (TrapComponent trapComponent in trapComponents)
            {
                pm.pathManager.RemoveTrapComponent(trapComponent);
                //TODO : recycle trap gameObject
            }

            foreach (EnemyComponent enemyComponent in enemyComponents)
            {
                pm.pathManager.RemoveEnemyComponent(enemyComponent);
                //TODO : recycle enemy gameObject
            }

            Destroy(gameObject);
        }
        #endregion
    }
}
