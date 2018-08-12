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
        public InputManager inputManager;

        [Header("Player")]
        public Transform playerPrefab;
        public ChampionComponent player;
        public bool isHpDecreasing = false;
        public bool isSpIncreasing = false;
        private Coroutine hpDecreasingCoroutine;
        private Coroutine spIncreasingCoroutine;
        private Coroutine autoMoveCoroutine;
        private Coroutine gameOverCoroutine;

        [Header("Score")]
        public int distance;
        public int kill;
        public int totalCoin;
        public int addCoin;

        [Header("Pause")]
        public PausePanel pausePanel;

        [Header("GameOver")]
        public GameOverPanel gameOverPanel;

        #endregion

        #region Method


        void Awake()
        {
            pausePanel.gameObject.SetActive(true);
            gameOverPanel.gameObject.SetActive(true);

            totalCoin = GameManager.Instance.GetPlayerCoinFromPref();
            ResetGame();
        }

        public void RestartGame()
        {
            ResetGame();
            uiManager.animator_control.SetTrigger("slidein");
            uiManager.sceneTransition.FadeOut();
        }

        public void ResetGame()
        {
            pathManager.InitPath();
            distance = 0;
            kill = 0;
            addCoin = 0;
            uiManager.UpdateKill(this.kill);
            uiManager.UpdateDistance(this.distance);
            uiManager.UpdateCoin(false);

            isHpDecreasing = false;
            isSpIncreasing = false;
            if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
            if (hpDecreasingCoroutine != null) StopCoroutine(hpDecreasingCoroutine);
            if (spIncreasingCoroutine != null) StopCoroutine(spIncreasingCoroutine);
            if (autoMoveCoroutine != null) StopCoroutine(autoMoveCoroutine);
            if (this.player != null) Destroy(this.player.gameObject);
            Transform playerTransform = Instantiate(playerPrefab) as Transform;
            ChampionComponent playerComponent = playerTransform.GetComponent<ChampionComponent>();
            Node startNode = new Node(0, 1);
            playerComponent.InitPlayerComponent(startNode, Direction.right);
            this.player = playerComponent;
            // autoMoveCoroutine = StartCoroutine(AutoMove());

            cameraController.SetPosition(startNode);
            cameraController.SetTarget(playerComponent);
            cameraController.SetInitialRotation(startNode);
            // cameraController.SetPivotAngle(Direction.right);

            uiManager.InitUIManager();
        }

        //TODO:특수스킬이나 아이템 사용과 같은 입력이 들어올 수 있으
        public void PlayerAction(PlayerInput input)
        {
            // if (!isHpDecreasing)
            // {
            //     isHpDecreasing = true;
            //     hpDecreasingCoroutine = StartCoroutine(DecreasingPlayerHp());
            // }
            if (!isSpIncreasing)
            {
                isSpIncreasing = true;
                spIncreasingCoroutine = StartCoroutine(IncreasingPlayerSp());
            }

            //플레이어의 방향과 서있는 블럭의 타잎에 따라 다음에 가야할 노드와 회전유무가 정해진다.
            if (input == PlayerInput.left || input == PlayerInput.right || input == PlayerInput.forward)
            {
                if (player.isDead)
                {
                    Debug.Log("player is dead");
                    return;
                }
                if (player.isMoving) return;
                if (player.isAttacking) return;

                BlockComponent blockComponent = pathManager.GetBlockComponentByOrigin(player.origin);
                Node targetNode = player.origin;
                Direction targetDirection = player.direction;

                if (player.isWatered)
                {
                    player.isWatered = false;
                    player.MovePlayer(player.origin, player.origin, 0.2f, false);
                    return;
                }

                //스트레이트 블럭 위에 있고 입력받은곳에 적이 있는지 확인해야한다
                //공격후 적이 죽는다면 적이 있던 자리로 이동하고 적이 죽지 않으면 원래 있던 자리로 돌아와야 한다
                // Debug.Log("player.origin: " + player.origin.ToString());
                Node possibleEnemyNode = GetNextNode(player, input);
                // Debug.Log("possibleEnemyNode: " + possibleEnemyNode.ToString());
                if (blockComponent.blockData.blockCategory == BlockCategory.straight && IsEnemyInDirection(possibleEnemyNode))
                {
                    player.Attack(player.origin, possibleEnemyNode, 0.2f);
                    return;
                }
                else if (
                    (blockComponent.blockData.blockCategory == BlockCategory.straight) ||
                    (blockComponent.blockData.blockCategory == BlockCategory.shortcut_end) ||
                    (blockComponent.blockData.blockCategory == BlockCategory.corner && player.direction == blockComponent.blockData.direction) ||
                    (blockComponent.blockData.blockCategory == BlockCategory.corner_edge && player.direction == blockComponent.blockData.direction))
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
                    else if (blockComponent.blockData.direction == Direction.down)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                        // else if (input == PlayerInput.backward) targetNode += new Node(0, -1);
                    }
                }
                else if (blockComponent.blockData.blockCategory == BlockCategory.shortcut_start)
                {
                    if (player.direction != blockComponent.blockData.direction)
                    {
                        if (player.direction == Direction.right)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(1, 1);
                            else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                        }
                        else if (player.direction == Direction.up)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                            else if (input == PlayerInput.right) targetNode += new Node(1, 1);
                            else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                        }
                        else if (player.direction == Direction.down)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(1, -1);
                            else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                        }
                    }
                    else
                    {
                        //이게 문제다. 오른쪽으로 향하고 있는 숏컷블럭인데 다음 섹션(코너)의 방향을 알아야 우상단숏컷인지 우하단 숏컷인지 알 수 있다.
                        if (blockComponent.blockData.direction == Direction.right)
                        {
                            if (blockComponent.sectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
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
                            else if (blockComponent.sectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                            {
                                if (input == PlayerInput.left)
                                {
                                    targetNode += new Node(1, 1);
                                }
                                else if (input == PlayerInput.right)
                                {
                                    targetNode += new Node(1, -1);
                                    BlockComponent targetBlockComponent = pathManager.GetBlockComponentByOrigin(targetNode);
                                    targetDirection = targetBlockComponent.blockData.direction;
                                }
                                else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                            }
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
                        else if (blockComponent.blockData.direction == Direction.down)
                        {
                            if (input == PlayerInput.left)
                            {
                                targetNode += new Node(1, -1);
                                BlockComponent targetBlockComponent = pathManager.GetBlockComponentByOrigin(targetNode);
                                targetDirection = targetBlockComponent.blockData.direction;
                            }
                            else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                        }
                    }
                }
                else if (blockComponent.blockData.blockCategory == BlockCategory.corner && player.direction != blockComponent.blockData.direction)
                {
                    if (player.direction == Direction.right)
                    {
                        if (blockComponent.blockData.direction == Direction.up)
                        {
                            if (input == PlayerInput.left)
                            {
                                targetNode += new Node(1, 1);
                                targetDirection = blockComponent.blockData.direction;
                            }
                            else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(1, 0);

                        }
                        else if (blockComponent.blockData.direction == Direction.down)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(1, 1);
                            else if (input == PlayerInput.right)
                            {
                                targetNode += new Node(1, -1);
                                targetDirection = blockComponent.blockData.direction;
                            }
                            else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                        }
                    }
                    else if (player.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(1, 1);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                    else if (player.direction == Direction.down)
                    {
                        if (input == PlayerInput.left)
                        {
                            targetNode += new Node(1, -1);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                    }
                }
                else if (blockComponent.blockData.blockCategory == BlockCategory.corner_edge && player.direction != blockComponent.blockData.direction)
                {
                    if (blockComponent.blockData.direction == Direction.right)
                    {
                        if (player.direction == Direction.up)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                            else if (input == PlayerInput.right)
                            {
                                targetNode += new Node(1, 0);
                                targetDirection = blockComponent.blockData.direction;
                            }
                            else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                        }
                        else if (player.direction == Direction.down)
                        {
                            if (input == PlayerInput.left)
                            {
                                targetNode += new Node(1, 0);
                                targetDirection = blockComponent.blockData.direction;
                            }
                            else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                        }
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
                    else if (blockComponent.blockData.direction == Direction.down)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(0, -1);
                            targetDirection = blockComponent.blockData.direction;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                    }
                }
                else if (blockComponent.blockData.blockCategory == BlockCategory.straight_edge)
                {
                    if (blockComponent.blockData.direction == Direction.right)
                    {
                        if (blockComponent.sectionComponent.nextSectionComponent.sectionData.direction == Direction.up)
                        {
                            if (input == PlayerInput.left)
                            {
                                targetNode += new Node(1, 1);
                                targetDirection = Direction.up;
                            }
                            else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                            else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                        }
                        else if (blockComponent.sectionComponent.nextSectionComponent.sectionData.direction == Direction.down)
                        {
                            if (input == PlayerInput.left) targetNode += new Node(1, 1);
                            else if (input == PlayerInput.right)
                            {
                                targetNode += new Node(1, -1);
                                targetDirection = Direction.down;
                            }
                            else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                        }
                    }
                    else if (blockComponent.blockData.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(1, 1);
                            targetDirection = Direction.right;
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                    else if (blockComponent.blockData.direction == Direction.down)
                    {
                        if (input == PlayerInput.left)
                        {
                            targetNode += new Node(1, -1);
                            targetDirection = Direction.right;
                        }
                        else if (input == PlayerInput.right)
                        {
                            targetNode += new Node(-1, -1);
                        }
                        else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                    }

                }
                bool isRotate = player.direction != targetDirection;
                player.MovePlayer(player.origin, targetNode, 0.2f, isRotate);
                if (isRotate)
                {
                    player.RotatePlayer(player.direction, targetDirection, 0.2f);
                }
            }
            else
            {
                //이동이외의 입력이 들어온다면 여기서 구현해야 한다
            }
        }

        Node GetNextNode(ChampionComponent player, PlayerInput input)
        {
            Node targetNode = player.origin;
            if (player.direction == Direction.right)
            {
                if (input == PlayerInput.left) { targetNode += new Node(1, 1); }
                else if (input == PlayerInput.forward) { targetNode += new Node(1, 0); }
                else if (input == PlayerInput.right) { targetNode += new Node(1, -1); }
            }
            else if (player.direction == Direction.up)
            {
                if (input == PlayerInput.left) { targetNode += new Node(-1, 1); }
                else if (input == PlayerInput.forward) { targetNode += new Node(0, 1); }
                else if (input == PlayerInput.right) { targetNode += new Node(1, 1); }
            }
            else if (player.direction == Direction.down)
            {
                if (input == PlayerInput.left) { targetNode += new Node(1, -1); }
                else if (input == PlayerInput.forward) { targetNode += new Node(0, -1); }
                else if (input == PlayerInput.right) { targetNode += new Node(-1, -1); }
            }
            return targetNode;
        }
        bool IsEnemyInDirection(Node targetNode)
        {
            return pathManager.GetEnemyComponent(targetNode) != null;
        }

        private float step = 0.3f;
        IEnumerator AutoMove()
        {
            while (!player.isDead)
            {
                yield return new WaitForSeconds(step);
                if (inputManager.lastInput == PlayerInput.none)
                {
                    PlayerAction(PlayerInput.forward);
                }
                else
                {
                    PlayerAction(inputManager.lastInput);
                    inputManager.lastInput = PlayerInput.none;
                }
            }
        }

        IEnumerator DecreasingPlayerHp()
        {
            while (!player.isDead)
            {
                yield return new WaitForSeconds(0.1f);
                player.TakeDamage(1, DamageType.time);
            }
        }

        IEnumerator IncreasingPlayerSp()
        {
            while (!player.isDead)
            {
                yield return new WaitForSeconds(0.1f);
                player.AddSp(0.2f);
                uiManager.UpdateSp();
            }
        }

        public void AddDistance(int add)
        {
            this.distance += add;
            uiManager.UpdateDistance(this.distance, true);
        }

        public void AddKill(int add)
        {
            this.kill += add;
            uiManager.UpdateKill(this.kill, true);
        }

        public void GameOver()
        {
            if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = StartCoroutine(GameOverHelper());
        }

        IEnumerator GameOverHelper()
        {
            yield return new WaitForSeconds(1.5f);
            int bestScore = GameManager.Instance.GetBestScoreFromPref();
            if (distance > bestScore)
            {
                GameManager.Instance.SetBestScoreToPref(distance);
            }
            uiManager.animator_control.SetTrigger("slideout");
            uiManager.animator_menu.SetTrigger("slidein");
            gameOverPanel.ShowGameOverPanel();
        }
        #endregion
    }
}