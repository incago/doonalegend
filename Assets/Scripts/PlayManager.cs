/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoonaLegend
{
    public class PlayManager : MonoBehaviour
    {
        #region Variables
        [Header("Managers")]
        public PathManager pathManager;
        public CameraController cameraController;
        public UIManager uiManager;

        [Header("Player")]
        public Transform playerPrefab;
        public PlayerComponent player;
        public bool isHpDecreasing = false;
        private Coroutine hpDecreasingCoroutine;

        [Header("Score")]
        public int score;

        #endregion

        #region Method
        void Awake()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            pathManager.InitPath();
            uiManager.InitUIManager();
            score = 0;
            uiManager.UpdateScore(this.score);

            isHpDecreasing = false;
            if (hpDecreasingCoroutine != null) StopCoroutine(hpDecreasingCoroutine);
            if (this.player != null) Destroy(this.player.gameObject);
            Transform playerTransform = Instantiate(playerPrefab) as Transform;
            PlayerComponent playerComponent = playerTransform.GetComponent<PlayerComponent>();
            Node startNode = new Node(1, 1);
            playerComponent.InitPlayerComponent(startNode, Direction.right);
            this.player = playerComponent;

            cameraController.SetPosition(startNode);
            cameraController.SetTarget(playerComponent);
            cameraController.SetPivotAngle(Direction.right);
        }

        //TODO:특수스킬이나 아이템 사용과 같은 입력이 들어올 수 있으
        public void PlayerAction(PlayerInput input)
        {
            if (!isHpDecreasing)
            {
                isHpDecreasing = true;
                StartCoroutine(DecreasingPlayerHp());
            }
            //플레이어의 방향과 서있는 블럭의 타잎에 따라 다음에 가야할 노드와 회전유무가 정해진다.
            if (input == PlayerInput.left || input == PlayerInput.right || input == PlayerInput.forward || input == PlayerInput.backward)
            {
                if (player.isDead)
                {
                    Debug.Log("player is dead");
                    return;
                }
                if (player.isMoving) return;

                BlockComponent blockComponent = pathManager.GetBlockComponentByOrigin(player.origin);
                Node targetNode = player.origin;
                Direction targetDirection = player.direction;

                if (
                    (blockComponent.blockData.blockType == BlockType.straight) ||
                    (blockComponent.blockData.blockType == BlockType.shortcut_end) ||
                    (blockComponent.blockData.blockType == BlockType.corner && player.direction == blockComponent.blockData.direction) ||
                    (blockComponent.blockData.blockType == BlockType.edge && player.direction == blockComponent.blockData.direction))
                {
                    if (blockComponent.blockData.direction == Direction.right)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(1, 1);
                        else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                        // else if (input == PlayerInput.backward) targetNode += new Node(-1, 0); // TODO : 뒤로 이동하면서 다른 섹션으로 넘어갈경우 예외적인 처리가 필요하다
                    }
                    else if (blockComponent.blockData.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right) targetNode += new Node(1, 1);
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                        // else if (input == PlayerInput.backward) targetNode += new Node(0, -1);
                    }
                }
                else if (blockComponent.blockData.blockType == BlockType.shortcut_start)
                {
                    if (blockComponent.blockData.direction == Direction.right)
                    {
                        if (input == PlayerInput.left)
                        {
                            targetNode += new Node(1, 1);
                            BlockComponent targetBlockComponent = pathManager.GetBlockComponentByOrigin(targetNode);
                            targetDirection = targetBlockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                    }
                    else if (blockComponent.blockData.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(1, 1);
                            BlockComponent targetBlockComponent = pathManager.GetBlockComponentByOrigin(targetNode);
                            targetDirection = targetBlockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                }
                else if (blockComponent.blockData.blockType == BlockType.corner && player.direction != blockComponent.blockData.direction)
                {
                    if (player.direction == Direction.right)
                    {
                        if (input == PlayerInput.left)
                        {
                            targetNode += new Node(0, 1);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                    }
                    else if (player.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(1, 0);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                }
                else if (blockComponent.blockData.blockType == BlockType.edge && player.direction != blockComponent.blockData.direction)
                {
                    if (blockComponent.blockData.direction == Direction.right)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(1, 0);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                    else if (blockComponent.blockData.direction == Direction.up)
                    {
                        if (input == PlayerInput.left)
                        {
                            targetNode += new Node(0, 1);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                    }
                }
                player.MovePlayer(player.origin, targetNode, 0.1f);
                if (player.direction != targetDirection)
                {
                    player.RotatePlayer(player.direction, targetDirection, 0.1f);
                }
            }
            else
            {
                //이동이외의 입력이 들어온다면 여기서 구현해야 한다
            }
        }

        IEnumerator DecreasingPlayerHp()
        {
            while (!player.isDead)
            {
                yield return new WaitForSeconds(0.1f);
                player.TakeDamage(0.1f);
                uiManager.UpdateHp();
            }
        }

        public void AddScore(int add)
        {
            this.score += add;
            uiManager.UpdateScore(this.score);
        }
        #endregion
    }
}