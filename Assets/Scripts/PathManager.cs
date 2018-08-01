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
            blockDictionaryByOrigin[blockComponent.blockData.origin.x].Add(blockComponent.blockData.origin.y, blockComponent);
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

        SectionComponent MakeSection(SectionModel sectionData)
        {
            Transform sectionTransform = Instantiate(sectionPrefab) as Transform;
            sectionTransform.gameObject.name = "section#" + sectionData.sectionId.ToString();
            sectionTransform.SetParent(sectionContainer);
            SectionComponent sectionComponent = sectionTransform.GetComponent<SectionComponent>();
            sectionComponent.InitSectionComponent(sectionData, lastSectionComponent);

            return sectionComponent;
        }

        void ClearPath()
        {
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
            int firstStraightSectionLength = Random.Range(3, 6) * 3;
            SectionModel firstStraightSection = new SectionModel(
                startSectionId,
                SectionType.straight,
                Direction.right,
                new Node(0, 0),
                firstStraightSectionLength, pathWidth);
            PutSectionComponent(MakeSection(firstStraightSection));
            SectionModel firstCornerSection = new SectionModel(
                lastSectionComponent.sectionData.sectionId + 1,
                SectionType.corner,
                Direction.up,
                new Node(firstStraightSectionLength, 0),
                pathWidth, pathWidth);
            PutSectionComponent(MakeSection(firstCornerSection));

            for (int i = 0; i < 5; i++) { AddSection(); }
        }

        public void AddSection()
        {
            int straightSectionLength = Random.Range(1, 10) * 3; //3,6,9,12,15
            SectionModel addStraightSection = new SectionModel(
                lastSectionComponent.sectionData.sectionId + 1,
                SectionType.straight,
                lastSectionComponent.sectionData.direction,
                lastSectionComponent.sectionData.origin + (lastSectionComponent.sectionData.direction == Direction.up ? new Node(0, pathWidth) : new Node(pathWidth, 0)),
                lastSectionComponent.sectionData.direction == Direction.up ? pathWidth : straightSectionLength,
                lastSectionComponent.sectionData.direction == Direction.up ? straightSectionLength : pathWidth);
            PutSectionComponent(MakeSection(addStraightSection));
            SectionModel addCornerSection = new SectionModel(
                lastSectionComponent.sectionData.sectionId + 1,
                SectionType.corner,
                lastSectionComponent.sectionData.direction == Direction.up ? Direction.right : Direction.up,
                lastSectionComponent.sectionData.origin + (lastSectionComponent.sectionData.direction == Direction.up ? new Node(0, lastSectionComponent.sectionData.height) : new Node(lastSectionComponent.sectionData.width, 0)),
                pathWidth, pathWidth);
            PutSectionComponent(MakeSection(addCornerSection));

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