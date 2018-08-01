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
        private PlayerComponent player;
        public Transform pivot;
        public Vector3 playerRightAngle = new Vector3(45.0f, 60.0f, 0);
        public Vector3 playerUpAngle = new Vector3(45.0f, 30.0f, 0);
        private Coroutine pivotRotateCoroutine;
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

        public void SetTarget(PlayerComponent target)
        {
            this.player = target;
        }

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
            else if (direction == Direction.right) { targetRotation = Quaternion.Euler(playerRightAngle); }
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / pivotRotateDuration);
                pivot.rotation = Quaternion.Lerp(initialRotation, targetRotation, percent);
                yield return null;
            }
        }
        #endregion
    }
}