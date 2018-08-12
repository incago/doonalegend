/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class PathManager : MonoBehaviour
    {
        #region Variables
        public Transform sectionPrefab;
        public Transform sectionContainer;
        public SectionComponent lastSectionComponent
        {
            get
            {
                if (sectionList.Count != 0) return sectionList[sectionList.Count - 1];
                else return null;
            }
        }
        public int pathWidth = 3;
        public int maxMaintainSectionCount = 20;

        [Header("Enemy Components")]
        public Dictionary<int, Dictionary<int, EnemyComponent>> enemyDictionaryByOrigin = new Dictionary<int, Dictionary<int, EnemyComponent>>();
        public EnemyComponent GetEnemyComponent(Node node)
        {
            if (enemyDictionaryByOrigin.ContainsKey(node.x))
            {
                if (enemyDictionaryByOrigin[node.x].ContainsKey(node.y))
                {
                    return enemyDictionaryByOrigin[node.x][node.y];
                }
            }
            // Debug.Log("enemyDictionaryByOrigin do not have key: " + node.ToString());
            return null;
        }
        public void PutEnemyComponent(EnemyComponent enemyComponent)
        {
            if (!enemyDictionaryByOrigin.ContainsKey(enemyComponent.origin.x))
            {
                enemyDictionaryByOrigin.Add(enemyComponent.origin.x, new Dictionary<int, EnemyComponent>());
            }
            enemyDictionaryByOrigin[enemyComponent.origin.x].Add(enemyComponent.origin.y, enemyComponent);
        }
        public void RemoveEnemyComponent(EnemyComponent enemyComponent)
        {
            if (enemyDictionaryByOrigin.ContainsKey(enemyComponent.origin.x))
            {
                enemyDictionaryByOrigin[enemyComponent.origin.x].Remove(enemyComponent.origin.y);
            }
        }

        [Header("Item Components")]
        public Dictionary<int, Dictionary<int, ItemComponent>> itemDictionaryByOrigin = new Dictionary<int, Dictionary<int, ItemComponent>>();
        public ItemComponent GetItemComponent(Node node)
        {
            if (itemDictionaryByOrigin.ContainsKey(node.x))
            {
                if (itemDictionaryByOrigin[node.x].ContainsKey(node.y))
                {
                    return itemDictionaryByOrigin[node.x][node.y];
                }
            }
            // Debug.Log("itemDictionaryByOrigin do not have key: " + node.ToString());
            return null;
        }
        public void PutItemComponent(ItemComponent itemComponent)
        {
            if (!itemDictionaryByOrigin.ContainsKey(itemComponent.origin.x))
            {
                itemDictionaryByOrigin.Add(itemComponent.origin.x, new Dictionary<int, ItemComponent>());
            }
            itemDictionaryByOrigin[itemComponent.origin.x].Add(itemComponent.origin.y, itemComponent);
        }
        public void RemoveItemComponent(ItemComponent itemComponent)
        {
            if (itemDictionaryByOrigin.ContainsKey(itemComponent.origin.x))
            {
                itemDictionaryByOrigin[itemComponent.origin.x].Remove(itemComponent.origin.y);
            }
        }

        [Header("Trap Components")]
        public Dictionary<int, Dictionary<int, TrapComponent>> trapDictionaryByOrigin = new Dictionary<int, Dictionary<int, TrapComponent>>();
        public TrapComponent GetTrapComponent(Node node)
        {
            if (trapDictionaryByOrigin.ContainsKey(node.x))
            {
                if (trapDictionaryByOrigin[node.x].ContainsKey(node.y))
                {
                    return trapDictionaryByOrigin[node.x][node.y];
                }
            }
            // Debug.Log("trapDictionaryByOrigin do not have key: " + node.ToString());
            return null;
        }
        public void PutTrapComponent(TrapComponent trapComponent)
        {
            if (!trapDictionaryByOrigin.ContainsKey(trapComponent.origin.x))
            {
                trapDictionaryByOrigin.Add(trapComponent.origin.x, new Dictionary<int, TrapComponent>());
            }
            trapDictionaryByOrigin[trapComponent.origin.x].Add(trapComponent.origin.y, trapComponent);
        }
        public void RemoveTrapComponent(TrapComponent trapComponent)
        {
            if (trapDictionaryByOrigin.ContainsKey(trapComponent.origin.x))
            {
                trapDictionaryByOrigin[trapComponent.origin.x].Remove(trapComponent.origin.y);
            }
        }


        [Header("Block Components")]
        public int lastBlockComponentId = 0;
        public Dictionary<int, BlockComponent> blockDictionary = new Dictionary<int, BlockComponent>();
        public Dictionary<int, Dictionary<int, BlockComponent>> blockDictionaryByOrigin = new Dictionary<int, Dictionary<int, BlockComponent>>();
        public void PutBlockComponent(BlockComponent blockComponent)
        {
            if (blockDictionary.ContainsKey(blockComponent.blockData.blockId))
            {
                Debug.Log("blockDictionary already have key: " + blockComponent.blockData.blockId.ToString());
                return;
            }
            blockDictionary.Add(blockComponent.blockData.blockId, blockComponent);
            if (!blockDictionaryByOrigin.ContainsKey(blockComponent.blockData.origin.x))
            {
                blockDictionaryByOrigin.Add(blockComponent.blockData.origin.x, new Dictionary<int, BlockComponent>());
            }
            if (!blockDictionaryByOrigin[blockComponent.blockData.origin.x].ContainsKey(blockComponent.blockData.origin.y))
            {
                blockDictionaryByOrigin[blockComponent.blockData.origin.x].Add(blockComponent.blockData.origin.y, blockComponent);
            }
            else
            {
                Debug.Log("blockDictionaryByOrigin already have key: " + blockComponent.blockData.origin.ToString());
            }
        }
        public BlockComponent GetBlockComponent(int blockId)
        {
            if (!blockDictionary.ContainsKey(blockId))
            {
                Debug.Log("blockDictionary do not have key: " + blockId.ToString());
                return null;
            }
            return blockDictionary[blockId]; ;
        }
        public BlockComponent GetBlockComponentByOrigin(Node node)
        {
            if (blockDictionaryByOrigin.ContainsKey(node.x))
            {
                if (blockDictionaryByOrigin[node.x].ContainsKey(node.y))
                {
                    return blockDictionaryByOrigin[node.x][node.y];
                }
            }
            Debug.Log("blockDictionaryByOrigin do not have key: " + node.ToString());
            return null;
        }
        public void RemoveBlockComponent(BlockComponent blockComponent)
        {
            blockDictionary.Remove(blockComponent.blockData.blockId);
            if (blockDictionaryByOrigin.ContainsKey(blockComponent.blockData.origin.x))
            {
                blockDictionaryByOrigin[blockComponent.blockData.origin.x].Remove(blockComponent.blockData.origin.y);
            }
        }

        [Header("Section Components")]
        public List<SectionComponent> sectionList = new List<SectionComponent>();
        public Dictionary<int, SectionComponent> sectionDictionary = new Dictionary<int, SectionComponent>();
        public SectionComponent GetSectionComponent(int sectionId)
        {
            if (!sectionDictionary.ContainsKey(sectionId))
            {
                Debug.Log("sectionDictionary do not have key: " + sectionId.ToString());
                return null;
            }
            return sectionDictionary[sectionId];
        }
        public void PutSectionComponent(SectionComponent sectionComponent)
        {
            if (sectionDictionary.ContainsKey(sectionComponent.sectionData.sectionId))
            {
                Debug.Log("sectionDictionary already have key: " + sectionComponent.sectionData.sectionId.ToString());
                return;
            }
            sectionList.Add(sectionComponent);
            sectionDictionary.Add(sectionComponent.sectionData.sectionId, sectionComponent);
        }
        public void RemoveSectionComponent(SectionComponent sectionComponent)
        {
            sectionList.Remove(sectionComponent);
            sectionDictionary.Remove(sectionComponent.sectionData.sectionId);
        }
        #endregion

        #region Method

        SectionComponent MakeSection(SectionModel sectionData, SectionModel nextSectionData = null)
        {
            // Debug.Log("PathManager.MakeSection(" + sectionData.sectionId + ")");
            Transform sectionTransform = Instantiate(sectionPrefab) as Transform;
            sectionTransform.gameObject.name = "section#" + sectionData.sectionId.ToString();
            sectionTransform.SetParent(sectionContainer);
            SectionComponent sectionComponent = sectionTransform.GetComponent<SectionComponent>();
            sectionComponent.InitSectionComponent(sectionData, nextSectionData, lastSectionComponent);

            return sectionComponent;
        }

        void ClearPath()
        {
            itemDictionaryByOrigin.Clear();
            enemyDictionaryByOrigin.Clear();
            trapDictionaryByOrigin.Clear();

            blockDictionary.Clear();
            blockDictionaryByOrigin.Clear();
            lastBlockComponentId = 0;

            sectionList.Clear();
            sectionDictionary.Clear();
            DestroyChildGameObject(sectionContainer);
        }

        public void InitPath()
        {
            int startSectionId = 0;
            ClearPath();
            int unitLength = Random.Range(3, 6);
            unitLength = 3;
            int firstStraightSectionLength = unitLength * 3;
            SectionContent sectionContent = MapManager.Instance.GetSectorContent(unitLength);
            SectionModel firstStraightSection = new SectionModel(
                startSectionId,
                SectionType.straight,
                Direction.right,
                new Node(0, 0),
                firstStraightSectionLength, pathWidth);
            Direction firstCornerSectionDirection = Random.Range(0, 100) < 50 ? Direction.up : Direction.down;
            SectionModel firstCornerSection = new SectionModel(
                firstStraightSection.sectionId + 1,
                SectionType.corner,
                // Direction.up,
                firstCornerSectionDirection,
                new Node(firstStraightSectionLength, 0),
                pathWidth, pathWidth);

            SectionComponent firstStraightSectionComponent = MakeSection(firstStraightSection, firstCornerSection);
            PutSectionComponent(firstStraightSectionComponent);
            SectionComponent firstCornerSectionComponent = MakeSection(firstCornerSection);
            firstStraightSectionComponent.nextSectionComponent = firstCornerSectionComponent;
            firstCornerSectionComponent.beforeSectionComponent = firstStraightSectionComponent;
            PutSectionComponent(firstCornerSectionComponent);

            for (int i = 0; i < 5; i++) { AddSection(); }
        }

        public void AddSection()
        {
            int unitLength = Random.Range(2, 6);
            int straightSectionLength = unitLength * 3; //6, 9, 12, 15

            SectionContent sectionContent = MapManager.Instance.GetSectorContent(unitLength);
            // Debug.Log("unitLength: " + unitLength.ToString());
            // Debug.Log(sectionContent.unitLength.ToString());
            // Debug.Log(sectionContent.fileName);
            // Debug.Log(sectionContent.terrains.GetLength(0)); //always 3
            // Debug.Log(sectionContent.terrains.GetLength(1));
            // Debug.Log("---");

            Node straightSectionOrigin = lastSectionComponent.sectionData.origin;
            int straightSectionWidth = 0;
            int straightSectionHeight = 0;
            if (lastSectionComponent.sectionData.direction == Direction.up)
            {
                straightSectionOrigin += new Node(0, pathWidth);
                straightSectionWidth = pathWidth;
                straightSectionHeight = straightSectionLength;
            }
            else if (lastSectionComponent.sectionData.direction == Direction.right)
            {
                straightSectionOrigin += new Node(pathWidth, 0);
                straightSectionWidth = straightSectionLength;
                straightSectionHeight = pathWidth;
            }
            else if (lastSectionComponent.sectionData.direction == Direction.down)
            {
                straightSectionOrigin += new Node(0, -straightSectionLength);
                straightSectionWidth = pathWidth;
                straightSectionHeight = straightSectionLength;
            }
            int[,] terrains = new int[straightSectionHeight, straightSectionWidth];
            int[,] objects = new int[straightSectionHeight, straightSectionWidth];
            // Debug.Log("straightSectionHeight: " + straightSectionHeight.ToString());
            // Debug.Log("straightSectionWidth: " + straightSectionWidth.ToString());

            if (lastSectionComponent.sectionData.direction == Direction.right)
            {
                terrains = sectionContent.terrains;
                foreach (KeyValuePair<int, Dictionary<int, int>> kv in sectionContent.objects)
                {
                    int x = kv.Key;
                    foreach (KeyValuePair<int, int> kv2 in kv.Value)
                    {
                        int y = kv2.Key;
                        int gid = kv2.Value;
                        // Debug.Log(x.ToString() + "," + y.ToString());
                        objects[x, y] = gid;
                    }
                }
                foreach (KeyValuePair<int, Dictionary<int, int>> kv in sectionContent.objects)
                {
                    int x = kv.Key;
                    foreach (KeyValuePair<int, int> kv2 in kv.Value)
                    {
                        int y = kv2.Key;
                        int gid = kv2.Value;
                        objects[x, y] = gid;
                    }
                }
            }
            else if (lastSectionComponent.sectionData.direction == Direction.up)
            {
                for (int i = 0; i < sectionContent.terrains.GetLength(0); i++)
                {
                    for (int j = 0; j < sectionContent.terrains.GetLength(1); j++)
                    {
                        //TODO: 검증해봐야 함
                        terrains[j, sectionContent.terrains.GetLength(0) - 1 - i] = sectionContent.terrains[i, j];
                    }
                }
                foreach (KeyValuePair<int, Dictionary<int, int>> kv in sectionContent.objects)
                {
                    int x = kv.Key;
                    foreach (KeyValuePair<int, int> kv2 in kv.Value)
                    {
                        int y = kv2.Key;
                        int gid = kv2.Value;
                        objects[y, sectionContent.terrains.GetLength(0) - 1 - x] = gid;
                    }
                }
            }
            else if (lastSectionComponent.sectionData.direction == Direction.down)
            {
                for (int i = 0; i < sectionContent.terrains.GetLength(0); i++)
                {
                    for (int j = 0; j < sectionContent.terrains.GetLength(1); j++)
                    {
                        //TODO: 검증해봐야 함
                        terrains[sectionContent.terrains.GetLength(1) - 1 - j, i] = sectionContent.terrains[i, j];
                    }
                }
                foreach (KeyValuePair<int, Dictionary<int, int>> kv in sectionContent.objects)
                {
                    int x = kv.Key;
                    foreach (KeyValuePair<int, int> kv2 in kv.Value)
                    {
                        int y = kv2.Key;
                        int gid = kv2.Value;
                        objects[sectionContent.terrains.GetLength(1) - 1 - y, x] = gid;
                    }
                }
            }

            SectionModel addStraightSection = new SectionModel(
                lastSectionComponent.sectionData.sectionId + 1,
                SectionType.straight,
                lastSectionComponent.sectionData.direction,
                straightSectionOrigin,
                straightSectionWidth, straightSectionHeight,
                terrains, objects);


            Direction cornerSectionDirection = Direction.none;
            Node cornerSectionOrigin = addStraightSection.origin;
            if (addStraightSection.direction == Direction.up)
            {
                cornerSectionDirection = Direction.right;
                cornerSectionOrigin += new Node(0, addStraightSection.height);
            }
            else if (addStraightSection.direction == Direction.right)
            {
                cornerSectionDirection = Random.Range(0, 100) < 50 ? Direction.up : Direction.down;
                cornerSectionOrigin += new Node(addStraightSection.width, 0);
            }
            else if (addStraightSection.direction == Direction.down)
            {
                cornerSectionDirection = Direction.right;
                cornerSectionOrigin += new Node(0, -pathWidth);
            }
            SectionModel addCornerSection = new SectionModel(
                addStraightSection.sectionId + 1,
                SectionType.corner,
                cornerSectionDirection,
                cornerSectionOrigin,
                pathWidth, pathWidth);

            SectionComponent straightSectionComponent = MakeSection(addStraightSection, addCornerSection);
            lastSectionComponent.nextSectionComponent = straightSectionComponent;
            straightSectionComponent.beforeSectionComponent = lastSectionComponent;
            PutSectionComponent(straightSectionComponent);
            SectionComponent cornerSectionComponent = MakeSection(addCornerSection);
            straightSectionComponent.nextSectionComponent = cornerSectionComponent;
            cornerSectionComponent.beforeSectionComponent = straightSectionComponent;
            PutSectionComponent(cornerSectionComponent);

            if (sectionList.Count > maxMaintainSectionCount)
            {
                SectionComponent firstStraightSection = sectionList[0];
                SectionComponent firstCornerSection = sectionList[1];
                RemoveSectionComponent(firstStraightSection);
                RemoveSectionComponent(firstCornerSection);
                firstStraightSection.RemoveSection();
                firstCornerSection.RemoveSection();
            }
        }


        public void DestroyChildGameObject(Transform parent)
        {
            var childItems = new List<GameObject>();
            foreach (Transform child in parent) childItems.Add(child.gameObject);
            childItems.ForEach(child => Destroy(child));
        }
        #endregion
    }
}