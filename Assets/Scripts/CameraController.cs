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
        private ChampionComponent player;
        public Transform pivot;
        public Vector3 playerUpAngle = new Vector3(45.0f, 30.0f, 0);
        // public Vector3 playerUpPosition = new Vector3(-5, 10, -5);


        public Vector3 playerRightUpAngle = new Vector3(45.0f, 60.0f, 0);
        // public Vector3 playerRightUpPosition = new Vector3(-5, 10, -5);
        public Vector3 playerRightDownAngle = new Vector3(45.0f, 120.0f, 0);
        // public Vector3 playerRightDownPosition = new Vector3(0, 0, 0);


        public Vector3 playerDownAngle = new Vector3(45.0f, 150.0f, 0);
        // public Vector3 playerDownPosition = new Vector3(-5, 10, -5);
        private Coroutine pivotRotateCoroutine;
        private Coroutine animatePivotRotateCoroutine;
        public float pivotRotateDuration = 0.2f;
        public float smoothing = 5f;	// The speed with which the camera will be following.
        #endregion

        #region Method
        void FixedUpdate()
        {
            if (player != null && !player.isDead)
            {
                Vector3 targetPosition = player.transform.position;
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
            this.player = target;
        }

        public void SetInitialRotation(Node playerOrigin)
        {
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(playerOrigin);
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
            { initialRotation = Quaternion.Euler(pm.cameraController.playerRightUpAngle); }
            else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
            { initialRotation = Quaternion.Euler(pm.cameraController.playerRightDownAngle); }

            if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
            { targetRotation = Quaternion.Euler(pm.cameraController.playerUpAngle); }
            else if (currentBlockComponent.sectionComponent.nextSectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
            { targetRotation = Quaternion.Euler(pm.cameraController.playerDownAngle); }

            pm.cameraController.AnimatePivotAngle(initialRotation, targetRotation, startPercent, endPercent, 0.3f);
        }

        /*
        public void SetPivotAngle(Direction direction)
        {
            if (pivotRotateCoroutine != null) StopCoroutine(pivotRotateCoroutine);
            pivotRotateCoroutine = StartCoroutine(SetPivotAngleHelper(direction));
        }

        IEnumerator SetPivotAngleHelper(Direction direction)
        {
            Quaternion initialRotation = pivot.rotation;
            Quaternion targetRotation = Quaternion.identity;
            if (direction == Direction.up) { targetRotation = Quaternion.Euler(playerUpAngle); }
            else if (direction == Direction.right) { targetRotation = Quaternion.Euler(playerRightUpAngle); }
            else if (direction == Direction.down) { targetRotation = Quaternion.Euler(playerDownAngle); }
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / pivotRotateDuration);
                pivot.rotation = Quaternion.Lerp(initialRotation, targetRotation, percent);
                yield return null;
            }
        }
         */

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