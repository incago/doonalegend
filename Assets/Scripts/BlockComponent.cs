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
        public Renderer blockRenderer;
        public Material greenMaterial, purpleMaterial, redMaterial, orangeMaterial, yellowMaterial;
        public Material brightGreenMaterial, brightPurpleMaterial;
        // public Material brightMaterial, darkMaterial;
        #endregion

        #region Method
        public void InitBlockComponent(SectionComponent sectionComponent, BlockModel blockData)
        {
            this.sectionComponent = sectionComponent;
            this.blockData = blockData;

            // if ((blockData.origin.x % 2 == 0 && blockData.origin.y % 2 == 0) ||
            // (blockData.origin.x % 2 == 1 && blockData.origin.y % 2 == 1))
            // {
            //     blockRenderer.sharedMaterial = brightMaterial;
            // }
            // else { blockRenderer.sharedMaterial = darkMaterial; }


            if (this.blockData.blockType == BlockType.straight)
            {
                if ((Mathf.Abs(blockData.origin.x) % 2 == 0 && Mathf.Abs(blockData.origin.y) % 2 == 0) ||
                (Mathf.Abs(blockData.origin.x) % 2 == 1 && Mathf.Abs(blockData.origin.y) % 2 == 1))
                {
                    blockRenderer.sharedMaterial = greenMaterial;
                }
                else { blockRenderer.sharedMaterial = brightGreenMaterial; }
            }
            else if (this.blockData.blockType == BlockType.corner)
            {
                if ((Mathf.Abs(blockData.origin.x) % 2 == 0 && Mathf.Abs(blockData.origin.y) % 2 == 0) ||
                (Mathf.Abs(blockData.origin.x) % 2 == 1 && Mathf.Abs(blockData.origin.y) % 2 == 1))
                {
                    blockRenderer.sharedMaterial = purpleMaterial;
                }
                else { blockRenderer.sharedMaterial = brightPurpleMaterial; }
            }
            else if (this.blockData.blockType == BlockType.shortcut_start)
            {
                blockRenderer.sharedMaterial = redMaterial;
            }
            else if (this.blockData.blockType == BlockType.shortcut_end)
            {
                blockRenderer.sharedMaterial = yellowMaterial;
            }
            else if (this.blockData.blockType == BlockType.edge)
            {
                blockRenderer.sharedMaterial = orangeMaterial;
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