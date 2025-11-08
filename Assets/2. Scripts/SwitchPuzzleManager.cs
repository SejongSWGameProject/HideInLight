using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 스위치 퍼즐의 정답을 관리하고,
/// '보이지 않는' UI 토글 상태를 확인합니다.
/// 3D 팝업 모델을 끄는(ESC, X버튼) 역할도 합니다.
/// </summary>
public class SwitchPuzzleManager : MonoBehaviour 
{
    [Header("연결")]
    [Tooltip("퍼즐에 사용할 4개의 '보이지 않는' UI 토글")]
    public Toggle[] switches; 

    [Tooltip("활성화/비활성화할 3D 팝업 모델 (예: PopupAnchor)")]
    public GameObject puzzlePopupModel; // 3D 팝업 모델을 연결할 변수

    [Header("정답")]
    private bool[] answerKey = new bool[4]; 
    private bool isPuzzleSolved = false; // 퍼즐이 풀렸는지 여부

    [Header("성공 이벤트")]
    [Tooltip("정답을 맞췄을 때 InteractableObject에 신호를 보낼 이벤트")]
    public UnityEvent OnPuzzleSolved; 

    void Start()
    {
        if (switches == null || switches.Length != 4)
        {
            Debug.LogError("SwitchPuzzleManager: UI 토글 4개를 모두 연결해야 합니다!");
            return;
        }
        GenerateRandomAnswer();
    }

    void Update()
    {
        // 3D 팝업이 켜져있고, 퍼즐이 아직 안 풀렸을 때만
        if (puzzlePopupModel != null && puzzlePopupModel.activeSelf && !isPuzzleSolved)
        {
            // ESC 키를 누르면 3D 팝업을 닫습니다.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePuzzlePopup();
            }
        }
    }

    // 랜덤 정답 생성
    void GenerateRandomAnswer()
    {
        isPuzzleSolved = false; // 퍼즐 새로 시작
        string answerLog = "스위치 퍼즐 랜덤 정답: ";
        for (int i = 0; i < answerKey.Length; i++)
        {
            answerKey[i] = (Random.Range(0, 2) == 1);
            answerLog += (answerKey[i] ? "ON " : "OFF ");
        }
        Debug.Log(answerLog); // ✅ 콘솔에 정답 출력
    }

    // '보이지 않는' 토글(Toggle)을 클릭할 때마다 호출될 함수
    public void CheckPuzzleSolution()
    {
        // 이미 푼 퍼즐은 다시 체크하지 않음
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
        isPuzzleSolved = true; // 퍼즐 풀림 상태로 변경
        
        if (OnPuzzleSolved != null)
        {
            OnPuzzleSolved.Invoke(); // InteractableObject에 "다 풀었다!" 신호 전송
        }

        // '보이지 않는' 토글 비활성화 (필수는 아님)
        foreach (Toggle sw in switches)
        {
            sw.interactable = false; 
        }

        // ▼▼▼ (2번 문제 해결) 주석을 제거하여, 정답을 맞추면 1초 후에 팝업이 자동으로 닫히게 합니다. ▼▼▼
        Invoke("ClosePuzzlePopup", 1.0f);
    }

    // '보이지 않는' X 닫기 버튼 또는 ESC 키가 호출할 함수
    public void ClosePuzzlePopup()
    {
        if (puzzlePopupModel != null)
        {
            puzzlePopupModel.SetActive(false);
            Debug.Log("3D 퍼즐 팝업을 닫았습니다.");
        }
    }
}