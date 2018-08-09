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
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public BlockModel blockData;
        public SectionComponent sectionComponent;
        public Renderer[] renderers;
        public Material greenMaterial, purpleMaterial, redMaterial, orangeMaterial, yellowMaterial;
        public Material brightGreenMaterial, brightPurpleMaterial;
        public Material brightMaterial, darkMaterial;
        public Animator animator;
        public Transform itemContainer;
        public Transform trapContainer;
        public bool isCollapsing = false;
        public bool isCollapsed = false;
        public bool isGrid = false;

        #endregion

        #region Method
        public void InitBlockComponent(SectionComponent sectionComponent, BlockModel blockData)
        {
            this.sectionComponent = sectionComponent;
            this.blockData = blockData;
            isCollapsing = isCollapsed = false;

            if (isGrid)
            {
                if ((Mathf.Abs(blockData.origin.x) % 2 == 0 && Mathf.Abs(blockData.origin.y) % 2 == 0) ||
                (Mathf.Abs(blockData.origin.x) % 2 == 1 && Mathf.Abs(blockData.origin.y) % 2 == 1))
                {
                    foreach (Renderer renderer in renderers) { renderer.sharedMaterial = brightMaterial; }
                }
                else
                {
                    foreach (Renderer renderer in renderers) { renderer.sharedMaterial = darkMaterial; }
                }
            }
            else
            {
                foreach (Renderer renderer in renderers) { renderer.sharedMaterial = brightMaterial; }
            }


            // if (this.blockData.blockType == BlockType.straight)
            // {
            //     if ((Mathf.Abs(blockData.origin.x) % 2 == 0 && Mathf.Abs(blockData.origin.y) % 2 == 0) ||
            //     (Mathf.Abs(blockData.origin.x) % 2 == 1 && Mathf.Abs(blockData.origin.y) % 2 == 1))
            //     {
            //         blockRenderer.sharedMaterial = greenMaterial;
            //     }
            //     else { blockRenderer.sharedMaterial = brightGreenMaterial; }
            // }
            // else if (this.blockData.blockType == BlockType.corner)
            // {
            //     if ((Mathf.Abs(blockData.origin.x) % 2 == 0 && Mathf.Abs(blockData.origin.y) % 2 == 0) ||
            //     (Mathf.Abs(blockData.origin.x) % 2 == 1 && Mathf.Abs(blockData.origin.y) % 2 == 1))
            //     {
            //         blockRenderer.sharedMaterial = purpleMaterial;
            //     }
            //     else { blockRenderer.sharedMaterial = brightPurpleMaterial; }
            // }
            // else if (this.blockData.blockType == BlockType.shortcut_start)
            // {
            //     blockRenderer.sharedMaterial = redMaterial;
            // }
            // else if (this.blockData.blockType == BlockType.shortcut_end)
            // {
            //     blockRenderer.sharedMaterial = yellowMaterial;
            // }
            // else if (this.blockData.blockType == BlockType.edge)
            // {
            //     blockRenderer.sharedMaterial = orangeMaterial;
            // }


            SetBlockPosition(this.blockData.origin);
        }

        void SetBlockPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }

        //delay초 이후에 떨리는 애니메이션이 실행된다.
        //그후 초후 붕괴된다
        public void StartCollapse(float delay, float shiveringDuration)
        {
            if (isCollapsing) return;
            isCollapsing = true;
            if (isCollapsed) return;
            Invoke("StartCollapseHelper", delay);
            Invoke("Collapse", delay + shiveringDuration);
        }

        void StartCollapseHelper()
        {
            animator.SetTrigger("shivering");
        }

        void Collapse()
        {
            isCollapsed = true;
            animator.SetTrigger("collapse");
            if (pm.player.origin == blockData.origin)
            {
                pm.player.TakeDamage(pm.player.hp, DamageType.drop);
            }
            //자신(블럭)의 위에 플레이어가 있는지 확인후 있다면 낙사판정
        }
        #endregion
    }
}