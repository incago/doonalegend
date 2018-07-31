/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class BlockComponent : MonoBehaviour
    {
        #region Variables
        public BlockModel blockData;
        public SectionComponent sectionComponent;
        public Renderer renderer;
        public Material greenMaterial, purpleMaterial, redMaterial, orangeMaterial, yellowMaterial;
        public Material brightGreenMaterial, brightPurpleMaterial;
        #endregion

        #region Method
        public void InitBlockComponent(SectionComponent sectionComponent, BlockModel blockData)
        {
            this.sectionComponent = sectionComponent;
            this.blockData = blockData;

            if (this.blockData.blockType == BlockType.straight)
            {
                if ((blockData.origin.x % 2 == 0 && blockData.origin.y % 2 == 0) ||
                (blockData.origin.x % 2 == 1 && blockData.origin.y % 2 == 1))
                {
                    renderer.sharedMaterial = greenMaterial;
                }
                else { renderer.sharedMaterial = brightGreenMaterial; }
            }
            else if (this.blockData.blockType == BlockType.corner)
            {
                if ((blockData.origin.x % 2 == 0 && blockData.origin.y % 2 == 0) ||
                (blockData.origin.x % 2 == 1 && blockData.origin.y % 2 == 1))
                {
                    renderer.sharedMaterial = purpleMaterial;
                }
                else { renderer.sharedMaterial = brightPurpleMaterial; }
            }
            else if (this.blockData.blockType == BlockType.shortcut_start)
            {
                renderer.sharedMaterial = redMaterial;
            }
            else if (this.blockData.blockType == BlockType.shortcut_end)
            {
                renderer.sharedMaterial = yellowMaterial;
            }
            else if (this.blockData.blockType == BlockType.edge)
            {
                renderer.sharedMaterial = orangeMaterial;
            }

            SetBlockPosition(this.blockData.origin);
        }

        void SetBlockPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }
        #endregion
    }
}