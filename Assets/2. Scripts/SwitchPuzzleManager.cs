using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // ▼▼▼ 1. UI (Toggle)을 사용하기 위해 추가 ▼▼▼
using System.Linq;

public class SwitchPuzzleManager : MonoBehaviour 
{
    [Header("연결")]
    [Tooltip("퍼즐에 사용할 4개의 UI 토글을 순서대로 연결하세요.")]
    // ▼▼▼ 2. ClickableSwitch[] -> Toggle[]로 변경 ▼▼▼
    public Toggle[] switches; // 4개의 UI 토글을 담을 배열

    [Tooltip("활성화/비활성화할 스위치 퍼즐 UI 패널")]
    // ▼▼▼ 3. UI 패널을 연결할 변수 추가 ▼▼▼
    public GameObject switchPanelUI; 

    [Header("정답")]
    private bool[] answerKey = new bool[4]; 

    [Header("성공 이벤트")]
    [Tooltip("정답을 맞췄을 때 실행할 이벤트를 연결하세요.")]
    public UnityEvent OnPuzzleSolved; 

    void Start()
    {
        if (switches.Length != 4)
        {
            Debug.LogError("SwitchPuzzleManager: UI 토글 4개를 모두 연결해야 합니다!");
            return;
        }
        GenerateRandomAnswer();
    }

    // 랜덤 정답 생성 (이 함수는 수정할 필요 없음)
    void GenerateRandomAnswer()
    {
        string answerLog = "스위치 퍼즐 랜덤 정답: ";
        for (int i = 0; i < answerKey.Length; i++)
        {
            answerKey[i] = (Random.Range(0, 2) == 1);
            answerLog += (answerKey[i] ? "ON " : "OFF ");
        }
        Debug.Log(answerLog); 
    }

    // 토글(Toggle)을 클릭할 때마다 호출될 함수
    public void CheckPuzzleSolution()
    {
        for (int i = 0; i < switches.Length; i++)
        {
            // ▼▼▼ 4. switches[i].isOn은 Toggle에도 있는 속성이라 수정 불필요 ▼▼▼
            if (switches[i].isOn != answerKey[i])
            {
                Debug.Log("스위치 퍼즐: 오답입니다.");
                return; 
            }
        }

        Debug.Log("스위치 퍼즐: 정답! 퍼즐 통과!");
        
        if (OnPuzzleSolved != null)
        {
            OnPuzzleSolved.Invoke();
        }

        // ▼▼▼ 5. 퍼즐을 풀면 토글을 비활성화(interactable = false)하도록 변경 ▼▼▼
        foreach (Toggle sw in switches)
        {
            sw.interactable = false; // enabled = false 대신 interactable = false 사용
        }
    }

    // ▼▼▼ 6. 'X' 닫기 버튼이 호출할 함수 추가 ▼▼▼
    /// <summary>
    /// 스위치 퍼즐 UI 패널을 닫습니다.
    /// </summary>
    public void ClosePuzzlePanel()
    {
        if (switchPanelUI != null)
        {
            switchPanelUI.SetActive(false);
            Debug.Log("스위치 퍼즐 UI가 닫혔습니다.");
        }
    }
}