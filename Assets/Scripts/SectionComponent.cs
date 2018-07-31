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
        //TODO : prefab manager
        public Transform blockPrefab;
        public Transform blockContainer;
        public List<BlockComponent> blockComponents = new List<BlockComponent>();
        public int maxProgress;
        #endregion

        #region Method
        public void InitSectionComponent(SectionModel sectionData, SectionComponent lastSectionComponent)
        {
            blockComponents.Clear();
            this.sectionData = sectionData;
            SetSectionPosition(this.sectionData.origin);

            int _maxProgress = -1;
            for (int i = 0; i < this.sectionData.height; i++)
            {
                for (int j = 0; j < this.sectionData.width; j++)
                {
                    Transform blockTransform = Instantiate(blockPrefab) as Transform;
                    blockTransform.gameObject.name = "block#" + pm.pathManager.lastBlockComponentId.ToString();
                    blockTransform.SetParent(blockContainer);
                    BlockComponent blockComponent = blockTransform.GetComponent<BlockComponent>();
                    BlockType blockType = this.sectionData.sectionType == SectionType.corner ? BlockType.corner : BlockType.straight;
                    if (this.sectionData.sectionType == SectionType.corner)
                    {
                        if ((this.sectionData.direction == Direction.right && i == this.sectionData.height - 1) ||
                        (this.sectionData.direction == Direction.up && j == this.sectionData.width - 1))
                        {
                            blockType = BlockType.edge;
                        }
                        else
                        {
                            blockType = BlockType.corner;
                        }
                    }
                    else if (j == this.sectionData.width - 1 && i == this.sectionData.height - 1)
                    {
                        blockType = BlockType.shortcut_start;
                    }
                    else if (j == 0 && i == 0)
                    {
                        blockType = BlockType.shortcut_end;
                    }
                    else
                    {
                        blockType = BlockType.straight;
                    }
                    int progress = 0;
                    int baseProgress = lastSectionComponent != null ? lastSectionComponent.maxProgress : 0;
                    if (sectionData.sectionType == SectionType.straight)
                    {
                        if (sectionData.direction == Direction.right)
                        { progress = baseProgress + j + 1; }
                        else if (sectionData.direction == Direction.up)
                        { progress = baseProgress + i + 1; }
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
                }
            }

            this.maxProgress = _maxProgress;
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
            }
            Destroy(gameObject);
        }
        #endregion
    }
}
