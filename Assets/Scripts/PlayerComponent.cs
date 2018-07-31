/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class PlayerComponent : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Node origin;
        public Direction direction;
        public int progress;
        private Coroutine moveCoroutine;
        private Coroutine rotateCoroutine;
        public bool isMoving = false;
        public bool isRotating = false;
        public bool isDead = false;
        public Transform body;
        #endregion

        #region Method
        public void InitPlayerComponent(Node node, Direction direction)
        {
            this.origin = node;
            this.direction = direction;

            SetPlayerPosition(this.origin);
        }

        void SetPlayerPosition(Node node)
        {
            gameObject.transform.position = new Vector3(node.x + 0.5f, 0, node.y + 0.5f);
        }

        public void MovePlayer(Node beforeNode, Node afterNode, float moveDuration)
        {
            moveCoroutine = StartCoroutine(MovePlayerHelper(beforeNode, afterNode, moveDuration));
        }

        IEnumerator MovePlayerHelper(Node beforeNode, Node afterNode, float moveDuration)
        {
            isMoving = true;
            Vector3 initialPosition = new Vector3(beforeNode.x + 0.5f, 0, beforeNode.y + 0.5f);
            Vector3 targetPosition = new Vector3(afterNode.x + 0.5f, 0, afterNode.y + 0.5f);
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / moveDuration);
                transform.position = Vector3.Lerp(initialPosition, targetPosition, percent);
                yield return null;
            }

            origin = afterNode;
            isMoving = false;
            OnMoveComplete(beforeNode);
            //if need callback here

        }
        void OnMoveComplete(Node beforeNode)
        {
            BlockComponent beforeBlockComponent = pm.pathManager.GetBlockComponentByOrigin(beforeNode);
            BlockComponent currentBlockComponent = pm.pathManager.GetBlockComponentByOrigin(this.origin);
            if (currentBlockComponent == null)
            {
                //there is no block blow player
                isDead = true;
            }
            else
            {
                if (beforeBlockComponent.sectionComponent != currentBlockComponent.sectionComponent && currentBlockComponent.sectionComponent.sectionData.sectionType == SectionType.straight)
                {
                    //time to generate more section
                    pm.pathManager.AddSection();
                }
            }
        }

        public void RotatePlayer(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            // Debug.Log("PlayerComponent.RotatePlayer(" + afterDirection.ToString() + ")");
            rotateCoroutine = StartCoroutine(RotatePlayerHelper(beforeDirection, afterDirection, rotateDuration));
            pm.cameraController.SetPivotAngle(afterDirection);
        }

        IEnumerator RotatePlayerHelper(Direction beforeDirection, Direction afterDirection, float rotateDuration)
        {
            isRotating = true;
            Quaternion initialRotation = body.rotation;
            Quaternion targetRotation = Quaternion.identity;
            if (afterDirection == Direction.up) { targetRotation = Quaternion.Euler(0, 270.0f, 0); }
            else if (afterDirection == Direction.right) { targetRotation = Quaternion.Euler(0, 0.0f, 0); }
            else if (afterDirection == Direction.down) { targetRotation = Quaternion.Euler(0, 90.0f, 0); }
            else if (afterDirection == Direction.left) { targetRotation = Quaternion.Euler(0, 180.0f, 0); }
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / rotateDuration);
                body.rotation = Quaternion.Lerp(initialRotation, targetRotation, percent);
                yield return null;
            }

            direction = afterDirection;
            isRotating = false;
            //if need callback here
        }

        #endregion
    }
}