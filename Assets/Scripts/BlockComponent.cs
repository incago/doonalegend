﻿/*
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
        public TerrainGid terrainGid;
        public SectionComponent sectionComponent;
        public Transform body;
        public Renderer[] renderers;
        // public Material greenMaterial, purpleMaterial, redMaterial, orangeMaterial, yellowMaterial;
        // public Material brightGreenMaterial, brightPurpleMaterial;
        public Material brightMaterial, darkMaterial;
        public Animator animator;
        public Transform objectContainer;
        public bool isCollapsing = false;
        public bool isCollapsed = false;
        public bool isGrid = false;

        [Header("Effect")]
        public bool hasStepEffect;
        public ParticleSystem effect_step;
        public Transform effectTransform;

        #endregion

        #region Method
        public void InitBlockComponent(SectionComponent sectionComponent, BlockModel blockData)
        {
            animator.SetTrigger("reset");
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
            SetBlockPosition(this.blockData.origin);
            SetBlockDirection(this.blockData.direction);
        }

        void SetBlockPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x, 0, node.y);
        }

        void SetBlockDirection(Direction direction)
        {
            if (direction == Direction.up) { body.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
            else if (direction == Direction.right) { body.rotation = Quaternion.Euler(new Vector3(0, 90.0f, 0)); }
            else if (direction == Direction.down) { body.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0)); }
            else if (direction == Direction.left) { body.rotation = Quaternion.Euler(new Vector3(0, 270.0f, 0)); }
        }

        //delay초 이후에 떨리는 애니메이션이 실행된다.
        //그후 초후 붕괴된다
        public void StartCollapse(float delay, float shiveringDuration, bool force = false)
        {
            if (isCollapsing && !force) return;
            CancelInvoke();
            isCollapsing = true;
            if (isCollapsed) return;
            Invoke("StartCollapseHelper", delay);
            Invoke("Collapse", delay + shiveringDuration);
        }

        void StartCollapseHelper()
        {
            animator.SetTrigger("shivering");
        }

        public void StopCollapse()
        {
            if (!isCollapsed)
            {
                animator.SetTrigger("reset");
                CancelInvoke("StartCollapseHelper");
                CancelInvoke("Collapse");
            }
        }

        void Collapse()
        {
            isCollapsed = true;
            animator.SetTrigger("collapse");
            if (pm.champion.origin == blockData.origin)
            {
                pm.champion.TakeDamage(pm.champion.currentHp, DamageType.drop);
            }
            //자신(블럭)의 위에 플레이어가 있는지 확인후 있다면 낙사판정
        }

        public void Finish()
        {
            animator.SetTrigger("finish");
        }

        public void MakeStepEffect()
        {
            // Transform _effectTransform = Instantiate(effect_step);
            // _effectTransform.position = effectTransform.position;
            if (GameManager.Instance.GetEffectActive())
            {
                Destroy(Instantiate(effect_step, effectTransform.position, Quaternion.identity).gameObject, effect_step.main.startLifetimeMultiplier);

            }
        }
        #endregion
    }
}