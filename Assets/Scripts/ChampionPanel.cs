/*
* Copyright (c) Incago Studio
* http://www.incagostudio.com/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoonaLegend
{
    public class ChampionPanel : MonoBehaviour
    {
        #region Variables
        private PlayManager _pm;
        public PlayManager pm
        {
            get { if (_pm == null) _pm = GameObject.FindGameObjectWithTag("PlayManager").GetComponent<PlayManager>(); return _pm; }
        }
        public Button button_back;
        public Button button_select;
        public Button button_buy;
        public Button button_gotcha;
        public Button button_share;
        public Animator animator;
        public PlaySceneState from;
        public Text text_championcounter;
        public Text text_championname;
        public Transform container;
        public bool isChampionPanelOpened = false;

        [Header("HP")]
        public Transform container_heart;
        public Transform heartSlotPrefab;
        public List<HeartSlot> hearts;

        [Header("Touch")]
        public ChampionPreview[] champions;
        public Transform championCameraTransform;
        public ChampionPreview focusedChampionPreview;
        public Transform container_focus;
        public Transform container_champion;
        public int currentChampionIndex;
        public float cameraPosition = 0;
        private Vector3 mouseDownPosition;
        public bool isMouseDown;
        public Vector3 beforeMousePosition;
        public float screenWidth;
        public float unitCountPerWidthOnPhone, unitCountPerWidthOnTablet;
        #endregion

        #region Method
        void Awake()
        {
            screenWidth = Screen.width;
            button_select.onClick.AddListener(() => { OnSelectButtonClick(); });
            button_back.onClick.AddListener(() => { OnBackButtonClick(); });
            MakeChampions();
        }

        public void MakeChampions()
        {

        }

        void Update()
        {
            if (isChampionPanelOpened)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnTouchStart();

                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnTouchEnd();
                }

                if (isMouseDown)
                {
                    OnTouchPhase();

                }

                //터치한상태로 좌우로 움직이면 카메라가 움직임
                //터치를 놓았을때 카메라가 클램프될 영역 밖이라면 클램프 영역까지 애니메이션해서 이동함
                //터치가 시작될때 클램프애니메이션은 멈추게되고 다시 터치한 상태의 동작이 이어짐
                //포커스된 동물은 가운데 고정되어 있고 좌우로 움직이지 않음
                //카메라하부의 포커스 영역에 포커스된 챔피언이 들어오는것
                //포커스가 바뀔때 원래의 자리로 돌아가는 애니메이션이 있어야함
                //포크스가 되는 챔피언은 우선 포커스컨테이너 안으로 이동시킨 후 원점으로 이동하는 애니메이션을 준다
            }
        }

        void OnTouchStart()
        {
            isMouseDown = true;
            mouseDownPosition = Input.mousePosition;
            beforeMousePosition = Input.mousePosition;
        }
        void OnTouchPhase()
        {
            Vector2 mouseDeltaPosition = Input.mousePosition - beforeMousePosition;
            championCameraTransform.position += new Vector3(-(mouseDeltaPosition.x / screenWidth) * (pm.settingPanel.uiSizeType == UISizeType.phone ? unitCountPerWidthOnPhone : unitCountPerWidthOnTablet), 0, 0);
            cameraPosition = championCameraTransform.position.x;
            int targetChampionIndex = Mathf.RoundToInt(Mathf.Clamp(cameraPosition, 0, champions.Length - 1));
            if (currentChampionIndex != targetChampionIndex)
            {
                currentChampionIndex = targetChampionIndex;
                OnChampionIndexChange(currentChampionIndex);
            }
            beforeMousePosition = Input.mousePosition;
        }

        void OnChampionIndexChange(int championIndex)
        {
            // Debug.Log("ChampionPanel.OnChampionIndexChange(" + championIndex.ToString() + ")");
            text_championname.text = champions[championIndex].championId;

            int maxHp = champions[championIndex].maxHp;
            int startHp = champions[championIndex].startHp;
            if (maxHp > 14) maxHp = 14;
            int heartCount = maxHp % 2 == 1 ? (maxHp + 1) / 2 : maxHp / 2;
            DestroyChildGameObject(container_heart);
            for (int i = 0; i < heartCount; i++)
            {
                Transform heartTransform = Instantiate(heartSlotPrefab) as Transform;
                heartTransform.SetParent(container_heart);
                heartTransform.localScale = Vector3.one;
                heartTransform.position = Vector2.zero;
                HeartSlot heartSlot = heartTransform.GetComponent<HeartSlot>();
                hearts.Add(heartSlot);
                int bgColorIndex = (maxHp - 1) / 8;
                int heartColorIndex = bgColorIndex + 1;
                HeartStatus heartStatus = HeartStatus.empty;

                if (i < startHp / 2) heartStatus = HeartStatus.full;
                else if ((i == startHp / 2) && (startHp % 2 == 1)) heartStatus = HeartStatus.half;
                else heartStatus = HeartStatus.empty;

                heartSlot.SetHeartSlot(heartStatus);
            }


            // for (int i = 0; i < champions.Length; i++)
            // {
            //     if (i == championIndex) continue;
            //     champions[i].transform.SetParent(container_champion);
            //     //위치 원래있던곳으로 애니메이션
            //     champions[i].transform.localPosition = new Vector3(champions[i].championIndex, 0, 0);//애니메이션
            //     champions[i].ScaleDownChampionPreview();
            // }
            if (focusedChampionPreview != null)
            {
                focusedChampionPreview.transform.SetParent(container_champion);
                //위치 원래있던곳으로 애니메이션
                focusedChampionPreview.transform.localPosition = new Vector3(focusedChampionPreview.championIndex, 0, 0);//애니메이션
                focusedChampionPreview.ScaleDownChampionPreview();
            }
            focusedChampionPreview = champions[championIndex];
            focusedChampionPreview.transform.SetParent(container_focus);
            focusedChampionPreview.transform.localPosition = Vector3.zero;//애니메이션
            focusedChampionPreview.SclaeUpChampionPreview();
        }

        void OnTouchEnd()
        {
            // Debug.Log("ChampionPanel.OnTouchEnd()");
            isMouseDown = false;
            float targetCameraPosition = Mathf.Clamp(cameraPosition, 0, champions.Length - 1);
            targetCameraPosition = Mathf.RoundToInt(targetCameraPosition);
            // Debug.Log("targetCameraPosition: " + targetCameraPosition);
            cameraPosition = targetCameraPosition;
            // championCameraTransform.position = new Vector3(targetCameraPosition, 0, 1000);
            AnimateChampionCamera(new Vector3(targetCameraPosition, 0, 1000), 0.05f);
            OnChampionIndexChange((int)targetCameraPosition);
        }

        private Coroutine animateChampionCamera;
        void AnimateChampionCamera(Vector3 targetCameraPosition, float duration)
        {
            if (animateChampionCamera != null) StopCoroutine(animateChampionCamera);
            animateChampionCamera = StartCoroutine(AnimateChampionCameraHelper(targetCameraPosition, duration));
        }
        IEnumerator AnimateChampionCameraHelper(Vector3 targetCameraPosition, float duration)
        {
            Vector3 initialCameraPosition = championCameraTransform.position;
            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * (1.0f / duration);
                championCameraTransform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, percent);
                yield return null;
            }

        }
        void OnSelectButtonClick()
        {
            pm.championId = champions[currentChampionIndex].championId;
            GameManager.Instance.SetLastPlayChampion(champions[currentChampionIndex].championId);
            if (from == PlaySceneState.main)
            {
                pm.ResetGame();
                OnBackButtonClick();
            }
            else if (from == PlaySceneState.gameover)
            {
                CloseChampionPanel();
                pm.mainPanel.FadeIn(pm.RestartGame);
            }
        }

        void OnBackButtonClick()
        {
            if (from == PlaySceneState.main)
            {
                CloseChampionPanel();
                pm.mainPanel.FastIn();
            }
            else if (from == PlaySceneState.gameover)
            {
                CloseChampionPanel();
                pm.gameOverPanel.ShowGameOverPanel();
                pm.gameOverPanel.animator_menu.SetTrigger("slidein");
                pm.gameOverPanel.animator_labels.SetTrigger("slidein");
                pm.uiManager.animator_toppanel.SetTrigger("slidein");
            }
        }

        public void OpenChampionPanel(PlaySceneState from)
        {
            string lastPlayChampionId = GameManager.Instance.GetLastPlayChampion();
            // Debug.Log("lastPlayChampionId: " + lastPlayChampionId);

            for (int i = 0; i < champions.Length; i++)
            {
                if (champions[i].championId.Equals(lastPlayChampionId))
                {
                    currentChampionIndex = i;
                    break;
                }
            }
            championCameraTransform.position = new Vector3(currentChampionIndex, 0, 1000);
            OnChampionIndexChange(currentChampionIndex);
            this.from = from;
            animator.SetTrigger("fadein");
            StartCoroutine(OpenChampionPanelHelper());
        }

        IEnumerator OpenChampionPanelHelper()
        {
            yield return new WaitForSeconds(0.1f);
            isChampionPanelOpened = true;
            isMouseDown = false;
        }

        public void CloseChampionPanel()
        {
            isMouseDown = false;
            isChampionPanelOpened = false;
            animator.SetTrigger("fadeout");
        }


        public void DestroyChildGameObject(Transform parent)
        {
            var childItems = new List<GameObject>();
            foreach (Transform child in parent) childItems.Add(child.gameObject);
            childItems.ForEach(child => Destroy(child));
        }
        #endregion
    }
}