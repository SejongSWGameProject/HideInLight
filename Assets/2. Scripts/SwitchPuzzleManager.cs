using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 스위치 퍼즐의 정답을 관리하고,
/// '보이지 않는' UI 토글 상태를 확인합니다.
/// 3D 팝업 모델과 2D UI 패널을 켜고 끄는(ESC, X버튼, 거리) 역할과
/// 플레이어/커서/라이트 제어를 담당합니다.
/// </summary>
public class SwitchPuzzleManager : MonoBehaviour
{
    [Header("연결")]
    [Tooltip("퍼즐에 사용할 4개의 '보이지 않는' UI 토글")]
    public Toggle[] switches;

    [Tooltip("활성화/비활성화할 3D 팝업 모델 (예: PopupAnchor)")]
    public GameObject puzzlePopupModel;

    [Tooltip("활성화/비활성화할 2D UI 패널 (SwitchPuzzle_Panel)")]
    public GameObject puzzleUIPanel;

    [Tooltip("숨겨야 할, 월드에 있는 상호작용 오브젝트 (generator)")]
    public GameObject inWorldInteractObject; // <-- 원본 변압기(generator) 연결 변수

    [Header("플레이어 제어")]
    [Tooltip("플레이어 게임 오브젝트")]
    public GameObject playerObject;

    [Tooltip("플레이어의 '움직임' 스크립트 (예: Player Move)")]
    public MonoBehaviour playerMovementScript;

    [Tooltip("플레이어의 '카메라 조작' 스크립트 (예: MouseLook)")]
    public MonoBehaviour cameraLookScript;

    [Tooltip("플레이어의 '라이트' 스크립트 (예: FlashlightController)")]
    public MonoBehaviour flashlightScript;

    [Tooltip("퍼즐을 자동으로 끌 최대 거리")]
    public float maxDistance = 5f;

    [Header("정답")]
    private bool[] answerKey = new bool[4];
    private bool isPuzzleSolved = false; // 퍼즐이 풀렸는지 여부

    [Header("성공 이벤트")]
    [Tooltip("정답을 맞췄을 때 InteractableObject에 신호를 보낼 이벤트")]
    public UnityEvent OnPuzzleSolved;

    private Transform playerTransform;

    void Start()
    {
        if (switches == null || switches.Length != 4)
        {
            Debug.LogError("SwitchPuzzleManager: UI 토글 4개를 모두 연결해야 합니다!");
            return;
        }
        GenerateRandomAnswer();

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("PlayerObject가 연결되지 않았습니다!");
        }

         // 시작 시 퍼즐 UI와 3D 모델을 모두 끈 상태로 초기화
        if (puzzlePopupModel != null) puzzlePopupModel.SetActive(false);
        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);
         // (inWorldInteractObject는 켜져 있어야 하므로 여기엔 없음)

         // 시작 시 커서 상태를 정상으로 (필요시)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
         // 3D 팝업이나 2D UI가 켜져있고, 퍼즐이 아직 안 풀렸을 때만
        if ((puzzlePopupModel != null && puzzlePopupModel.activeSelf) || (puzzleUIPanel != null && puzzleUIPanel.activeSelf))
        {
            if (!isPuzzleSolved)
            {
                 // ESC 키를 누르면 3D 팝업을 닫습니다.
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ClosePuzzlePopup();
                }

                /* // <-- 거리 체크 시작 (주석 처리)
                 if (playerTransform != null && puzzlePopupModel != null)
                 {
                     // 변압기(퍼즐 팝업)와 플레이어 사이의 거리 계산
                     float distance = Vector3.Distance(puzzlePopupModel.transform.position, playerTransform.position);

                     // 거리가 maxDistance보다 멀어지면 퍼즐 강제 종료
                     if (distance > maxDistance)
                     {
                         Debug.Log("플레이어가 너무 멀어져서 퍼즐을 닫습니다.");
                         ClosePuzzlePopup();
                     }
                 }
                 */ // <-- 거리 체크 끝 (주석 처리)
            }
        }
    }

     // 랜덤 정답 생성
    void GenerateRandomAnswer()
    {
        isPuzzleSolved = false; // 퍼즐 새로 시작
        string answerLog = "스위치 퍼즐 정답: ";
        for (int i = 0; i < answerKey.Length; i++)
        {
            // ▼▼▼ [수정됨] 1. 정답은 '랜덤'으로 생성 ▼▼▼
            answerKey[i] = (Random.Range(0, 2) == 1); // <-- 랜덤 정답 복원
            // answerKey[i] = false;  // <-- 이전 코드
            
            // ▼▼▼ [수정됨] 2. 시작 시 '모양'(UI 토글)은 OFF로 강제 설정 ▼▼▼
            if (switches[i] != null)
            {
                switches[i].isOn = false; // <-- 이 코드는 '모양'을 OFF로 강제합니다 (유지)
            }
            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            answerLog += (answerKey[i] ? "ON " : "OFF ");
        }
        Debug.Log(answerLog); // ✅ 콘솔에 정답 출력
    }

     // '보이지 않는' 토글(Toggle)을 클릭할 때마다 호출될 함수
    public void CheckPuzzleSolution()
    {
        if (isPuzzleSolved) return;

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i].isOn != answerKey[i])
            {
                Debug.Log("스위치 퍼즐: 오답입니다.");
                return;
            }
        }

        Debug.Log("스위치 퍼즐: 정답! 퍼즐 통과!");
        isPuzzleSolved = true;

        if (OnPuzzleSolved != null)
        {
            OnPuzzleSolved.Invoke();
        }

        foreach (Toggle sw in switches)
        {
            sw.interactable = false;
        }

        Invoke("ClosePuzzlePopup", 1.0f);
    }

     /// <summary>
     /// 퍼즐 팝업과 UI를 엽니다. (플레이어/카메라/라이트 정지, 커서 보이기)
     /// </summary>
    public void OpenPuzzlePopup()
    {
        // ▼▼▼ [수정됨] 2. 이미 퍼즐을 풀었다면 열리지 않음 ▼▼▼
        if (isPuzzleSolved)
        {
            Debug.Log("스위치 퍼즐: 이미 해결되었습니다.");
            return; // 함수 종료
        }
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        if (puzzlePopupModel != null) puzzlePopupModel.SetActive(true);
        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(true);
        if (inWorldInteractObject != null) inWorldInteractObject.SetActive(false); // <-- 원본 숨기기

        Debug.Log("3D 퍼즐 팝업과 2D UI를 엽니다. (원본 숨김)");

         // 1번, 3번 문제 해결: 플레이어 제어 비활성화
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraLookScript != null) cameraLookScript.enabled = false;
        if (flashlightScript != null) flashlightScript.enabled = false;

         // 1번, 3번 문제 해결: 마우스 커서 보이기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

     /// <summary>
     /// 퍼즐 팝업과 UI를 닫습니다. (플레이어/카메라/라이트 재개, 커서 숨기기)
     /// </summary>
    public void ClosePuzzlePopup()
    {
        if (puzzlePopupModel != null) puzzlePopupModel.SetActive(false);
        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);
        if (inWorldInteractObject != null) inWorldInteractObject.SetActive(true); // <-- 원본 다시 보이기

        Debug.Log("3D 퍼즐 팝업과 2D UI를 닫았습니다. (원본 표시)");

         // 1번, 3번 문제 해결: 플레이어 제어 활성화
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraLookScript != null) cameraLookScript.enabled = true;
        if (flashlightScript != null) flashlightScript.enabled = true;

         // 1번, 3번 문제 해결: 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}