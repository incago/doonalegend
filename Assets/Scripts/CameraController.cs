/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class CameraController : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        private ChampionComponent champion;
        public Transform pivot;
        public Vector3 championUpAngle = new Vector3(45.0f, 30.0f, 0);
        public Vector3 championRightUpAngle = new Vector3(45.0f, 60.0f, 0);
        public Vector3 championRightDownAngle = new Vector3(45.0f, 120.0f, 0);
        public Vector3 championDownAngle = new Vector3(45.0f, 150.0f, 0);
        private Coroutine pivotRotateCoroutine;
        private Coroutine animatePivotRotateCoroutine;
        public float pivotRotateDuration = 0.2f;
        public float smoothing = 5f;	// The speed with which the camera will be following.
        #endregion

        #region Method
        void FixedUpdate()
        {
            if (champion != null && !champion.isDead)
            {
                Vector3 targetPosition = champion.transform.position;
                Vector3 targetCameraPosition = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
                transform.position = new Vector3(targetCameraPosition.x, targetCameraPosition.y, targetCameraPosition.z);
            }
        }

        public void SetPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }

        public void SetTarget(ChampionComponent target)
        {
            this.champion = target;
        }

        public void SetInitialRotation(Node championOrigin)
        {
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(championOrigin);
            // SectionComponent firstSectionComponent = pm.pathManager.GetSectionComponent(1);
            // SectionComponent secondSectionComponent = pm.pathManager.GetSectionComponent(2);
            int currentProgress = currentBlockComponent.blockData.progress;
            int minProgressInSection = currentBlockComponent.sectionComponent.minProgress;
            int maxProgressInSection = currentBlockComponent.sectionComponent.maxProgress;
            float startPercent = (float)(currentProgress - minProgressInSection - 1) / (float)(maxProgressInSection - minProgressInSection);
            float endPercent = (float)(currentProgress - minProgressInSection) / (float)(maxProgressInSection - minProgressInSection);
            Quaternion initialRotation = Quaternion.identity;
            Quaternion targetRotation = Quaternion.identity;

            if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
            { initialRotation = Quaternion.Euler(pm.cameraController.championRightUpAngle); }
            else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
            { initialRotation = Quaternion.Euler(pm.cameraController.championRightDownAngle); }

            if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
            { targetRotation = Quaternion.Euler(pm.cameraController.championUpAngle); }
            else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
            { targetRotation = Quaternion.Euler(pm.cameraController.championDownAngle); }

            pm.cameraController.AnimatePivotAngle(initialRotation, targetRotation, startPercent, endPercent, 0.3f);
        }

        public void AnimatePivotAngle(Quaternion initialRotation, Quaternion targetRotation, float startPercent, float endPercent, float duration)
        {
            // Debug.Log("initialRotation: " + initialRotation.ToString());
            // Debug.Log("targetRotation: " + targetRotation.ToString());
            if (animatePivotRotateCoroutine != null) StopCoroutine(animatePivotRotateCoroutine);
            animatePivotRotateCoroutine = StartCoroutine(AnimatePivotAngleHelper(initialRotation, targetRotation, startPercent, endPercent, duration));
        }

        IEnumerator AnimatePivotAngleHelper(Quaternion initialRotation, Quaternion targetRotation, float startPercent, float endPercent, float duration)
        {
            float deltaPercent = endPercent - startPercent;
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / duration);
                pivot.rotation = Quaternion.Lerp(initialRotation, targetRotation, startPercent + percent * deltaPercent);
                yield return null;
            }
        }
        #endregion
    }
}