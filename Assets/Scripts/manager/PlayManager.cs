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
        public ObjectPool objectPool;

        [Header("Champion")]
        public Transform playerPrefab;
        public ChampionComponent champion;
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
        public int combo;
        public int totalCombo;
        public int score;

        [Header("Play State")]
        public PlaySceneState playSceneState;
        // private bool isWaitExit = false;

        [Header("Main")]
        public MainPanel mainPanel;

        [Header("Setting")]
        public SettingPanel settingPanel;

        [Header("Pause")]
        public PausePanel pausePanel;

        [Header("GameOver")]
        public GameOverPanel gameOverPanel;
        public bool isContinued;

        #endregion

        #region Method


        void Awake()
        {
            objectPool.InitStartupPools();
            mainPanel.gameObject.SetActive(true);
            settingPanel.gameObject.SetActive(true);
            pausePanel.gameObject.SetActive(true);
            gameOverPanel.gameObject.SetActive(true);

            totalCoin = GameManager.Instance.GetPlayerCoinFromPref();
            ResetGame();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("BACK!");
                if (Application.platform == RuntimePlatform.Android)
                {
                    OnPressBackButton();
                }
                else
                {
                    OnPressBackButton();
                }
            }
        }

        void OnPressBackButton()
        {
            Debug.Log("PlayManager.OnPressBackButton()");
            if (playSceneState == PlaySceneState.ready ||
            playSceneState == PlaySceneState.gameover ||
            playSceneState == PlaySceneState.pause ||
            playSceneState == PlaySceneState.main)
            {
                Application.Quit();
            }
            else if (playSceneState == PlaySceneState.play)
            {
                pausePanel.Pause();
            }
        }

        public void RestartGame()
        {
            ResetGame();
            // uiManager.animator_control.SetTrigger("slidein");
            // uiManager.sceneTransition.FadeOut();
        }

        public void ResetGame()
        {
            playSceneState = PlaySceneState.main;
            pathManager.InitPath();
            distance = 0;
            kill = 0;
            addCoin = 0;
            combo = 0;
            totalCombo = 0;
            score = 0;
            uiManager.UpdateKill(this.kill);
            uiManager.UpdateDistance(this.distance);
            uiManager.UpdateCoin(false);
            uiManager.UpdateScoreUI(this.score);

            isContinued = false;
            pausePanel.isPaused = false;
            pausePanel.button_pause.gameObject.SetActive(true);

            isHpDecreasing = false;
            isSpIncreasing = false;
            if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
            if (hpDecreasingCoroutine != null) StopCoroutine(hpDecreasingCoroutine);
            if (spIncreasingCoroutine != null) StopCoroutine(spIncreasingCoroutine);
            if (autoMoveCoroutine != null) StopCoroutine(autoMoveCoroutine);
            if (this.champion != null) Destroy(this.champion.gameObject);
            Transform playerTransform = Instantiate(playerPrefab) as Transform;
            ChampionComponent playerComponent = playerTransform.GetComponent<ChampionComponent>();
            Node startNode = new Node(1, 1);
            playerComponent.InitChampionComponent(startNode, Direction.right);
            this.champion = playerComponent;
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
            if (playSceneState == PlaySceneState.main ||
            playSceneState == PlaySceneState.gameover ||
            playSceneState == PlaySceneState.pause)
            {
                return;
            }
            if (playSceneState == PlaySceneState.ready)
            {
                playSceneState = PlaySceneState.play;
            }
            if (!isSpIncreasing)
            {
                isSpIncreasing = true;
                spIncreasingCoroutine = StartCoroutine(IncreasingPlayerSp());
            }

            //플레이어의 방향과 서있는 블럭의 타잎에 따라 다음에 가야할 노드와 회전유무가 정해진다.
            if (input == PlayerInput.left || input == PlayerInput.right || input == PlayerInput.forward)
            {
                if (champion.isDead)
                {
                    // Debug.Log("player is dead");
                    return;
                }
                if (champion.isMoving) return;
                if (champion.isAttacking) return;

                Node targetNode = champion.origin;
                Node nextNode = GetNextNode(champion, input);
                BlockComponent blockComponent = pathManager.GetBlockComponentByOrigin(champion.origin);
                BlockComponent nextBlockComponent = pathManager.GetBlockComponentByOrigin(nextNode); //null 일 수도있다
                Direction targetDirection = champion.direction;

                if (champion.isWatered)
                {
                    champion.isWatered = false;
                    champion.MoveChampion(champion.origin, champion.origin, 0.2f, MoveType.walk, false);
                    return;
                }
                else if (champion.isBitten)
                {
                    champion.isBitten = false;
                    champion.MoveChampion(champion.origin, champion.origin, 0.2f, MoveType.walk, false);
                    return;
                }


                //스트레이트 블럭 위에 있고 입력받은곳에 적이 있는지 확인해야한다
                //공격후 적이 죽는다면 적이 있던 자리로 이동하고 적이 죽지 않으면 원래 있던 자리로 돌아와야 한다
                // Debug.Log("player.origin: " + player.origin.ToString());
                TrapComponent trapComponent = pathManager.GetTrapComponent(nextNode);
                if (trapComponent != null && trapComponent.isObstacle)
                {
                    champion.MoveChampion(champion.origin, champion.origin, 0.2f, MoveType.walk, false);
                    return;
                }

                if (IsEnemyInDirection(nextNode))
                {
                    champion.Attack(champion.origin, nextNode, 0.2f);
                    return;
                }
                else
                {
                    targetDirection = champion.direction;
                    if (champion.direction == Direction.right)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(1, 1);
                        else if (input == PlayerInput.right) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(1, 0);
                    }
                    else if (champion.direction == Direction.up)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(-1, 1);
                        else if (input == PlayerInput.right) targetNode += new Node(1, 1);
                        else if (input == PlayerInput.forward) targetNode += new Node(0, 1);
                    }
                    else if (champion.direction == Direction.down)
                    {
                        if (input == PlayerInput.left) targetNode += new Node(1, -1);
                        else if (input == PlayerInput.right) targetNode += new Node(-1, -1);
                        else if (input == PlayerInput.forward) targetNode += new Node(0, -1);
                    }
                    if (nextBlockComponent != null && (nextBlockComponent.blockData.blockCategory == BlockCategory.turn || nextBlockComponent.blockData.blockCategory == BlockCategory.shortcut_end))
                    {
                        targetDirection = nextBlockComponent.blockData.direction;
                    }
                }

                bool isRotate = champion.direction != targetDirection;
                champion.MoveChampion(champion.origin, targetNode, 0.2f, MoveType.walk
                , isRotate);
                if (isRotate)
                {
                    champion.RotateChampion(champion.direction, targetDirection, 0.2f);
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
            while (!champion.isDead)
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
            while (!champion.isDead)
            {
                yield return new WaitForSeconds(0.1f);
                champion.TakeDamage(1, DamageType.time);
            }
        }

        IEnumerator IncreasingPlayerSp()
        {
            while (!champion.isDead)
            {
                yield return new WaitForSeconds(0.1f);
                champion.AddSp(0.2f);
                uiManager.UpdateSp();
            }
        }

        public void AddDistance(int add, bool updateScoreUI = false)
        {
            // Debug.Log("ChampionComponent.AddDistance(" + add.ToString() + ")");
            this.distance += add;
            this.score += add;
            uiManager.UpdateDistance(this.distance, true);
            if (updateScoreUI)
                uiManager.UpdateScoreUI(this.score);
        }

        public void AddKill(int add, bool updateScoreUI = false)
        {
            this.kill += add;
            this.score += (add * 10);
            uiManager.UpdateKill(this.kill, true);
            if (updateScoreUI)
                uiManager.UpdateScoreUI(this.score);
        }

        public void ContinueGame()
        {
            playSceneState = PlaySceneState.ready;
            isContinued = true;
            champion.isDead = false;
            SectionComponent sectionComponent = pathManager.currentSectionComponent.nextSectionComponent;
            Node continueNode = new Node(0, 0);
            if (sectionComponent.sectionData.direction == Direction.right)
            {
                continueNode = sectionComponent.sectionData.origin + new Node(0, 1);
            }
            else if (sectionComponent.sectionData.direction == Direction.up)
            {
                continueNode = sectionComponent.sectionData.origin + new Node(1, 0);
            }
            else if (sectionComponent.sectionData.direction == Direction.down)
            {
                continueNode = sectionComponent.sectionData.origin + new Node(1, sectionComponent.sectionData.height - 1);
            }
            champion.InitChampionComponent(continueNode, sectionComponent.sectionData.direction);
            champion.animator.SetTrigger("continue");
            gameOverPanel.HideGameOverPanel();
            pausePanel.button_pause.gameObject.SetActive(true);
            gameOverPanel.animator_continue.SetTrigger("slideout");
            uiManager.animator_control.SetTrigger("slidein");
            uiManager.InitHpUI(champion.maxHp, champion.hp, false);
            //챔피언 되살리고
            //어느 위치에 되살려야 하는가?
            //각종 변수 초기화하고
            //카운트 넣어준다음 게임시작시킴
        }

        public void GameOver()
        {
            playSceneState = PlaySceneState.gameover;
            if (gameOverCoroutine != null) StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = StartCoroutine(GameOverHelper());
            pausePanel.button_pause.gameObject.SetActive(false);
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
            if (!isContinued) { gameOverPanel.animator_continue.SetTrigger("slidein"); }
            else
            {
                uiManager.animator_score.SetTrigger("reset");
                gameOverPanel.animator_menu.SetTrigger("slidein");
                gameOverPanel.animator_labels.SetTrigger("slidein");
            }
            gameOverPanel.ShowGameOverPanel();
        }

        public void AddCombo()
        {
            this.combo++;
            if (this.combo >= 2)
            {
                this.totalCombo++;
                // uiManager.UpdateComboUI(this.combo);
                uiManager.MakeCanvasMessageHud(champion.transform, (this.combo).ToString() + " Combo!", champion.canvasHudOffset, Color.green, Color.black);
            }
        }

        public void ResetCombo()
        {
            if (this.combo >= 2)
            {
                // uiManager.UpdateComboUI(this.combo, Color.red, Color.white);
                // uiManager.animator_combo.SetTrigger("update");
                uiManager.MakeCanvasMessageHud(champion.transform, (this.combo).ToString() + " Combo!", champion.canvasHudOffset, Color.red, Color.white);
            }
            this.combo = 0;
        }

        void OnApplicationFocus(bool hasFocus)
        {
            // Debug.Log("PlayManage.OnApplicationFocus(" + hasFocus + ")");
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (hasFocus)
                {

                }
                else
                {
                    if (playSceneState == PlaySceneState.play)
                    {
                        pausePanel.Pause();
                    }
                }
            }
        }
        #endregion
    }
    public enum PlaySceneState
    {
        main, ready, play, pause, gameover
    }
    /*
     */
}