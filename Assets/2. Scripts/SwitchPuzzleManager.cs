using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class SwitchPuzzleManager : MonoBehaviour
{
    [Header("연결")]
    [Tooltip("퍼즐에 사용할 4개의 UI 토글을 순서대로 연결하세요.")]
    public Toggle[] switches; // 4개의 UI 토글을 담을 배열

    [Tooltip("활성화/비활성화할 스위치 퍼즐 UI 패널")]
    public GameObject switchPanelUI;

    [Header("정답")]
    private bool[] answerKey = new bool[4];

    [Header("성공 이벤트")]
    [Tooltip("정답을 맞췄을 때 실행할 이벤트를 연결하세요.")]
    public UnityEvent OnPuzzleSolved;

    [Header("퍼즐 상태")]
    [Tooltip("퍼즐이 풀렸는지 여부 (true가 되면 더 이상 상호작용 안 함)")]
    public bool isPuzzleSolved = false;

    void Start()
    {
        if (switches.Length != 4)
        {
            Debug.LogError("SwitchPuzzleManager: UI 토글 4개를 모두 연결해야 합니다!");
            return;
        }
        GenerateRandomAnswer();
    }

    // ▼▼▼ 'ESC' 키 감지를 위한 Update 함수 추가 ▼▼▼
    /// <summary>
    /// 매 프레임마다 키 입력을 확인합니다.
    /// </summary>
    void Update()
    {
        // 1. 패널이 null이 아니고, 
        // 2. 패널이 현재 켜져있으며(activeSelf),
        // 3. 'Escape' 키를 눌렀는지 확인
        if (switchPanelUI != null && switchPanelUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            // 'X' 버튼을 누른 것과 똑같이 ClosePuzzlePanel 함수를 호출
            ClosePuzzlePanel();
        }
    }
    // ▲▲▲ 여기까지 추가 ▲▲▲

    // 랜덤 정답 생성
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
        // 이미 퍼즐을 풀었다면, 더 이상 체크하지 않음
        if (isPuzzleSolved)
        {
            return;
        }

        for (int i = 0; i < switches.Length; i++)
        {
            // switches[i].isOn은 Toggle에도 있는 속성이라 수정 불필요
            if (switches[i].isOn != answerKey[i])
            {
                Debug.Log("스위치 퍼즐: 오답입니다.");
                return;
            }
        }

        Debug.Log("스위치 퍼즐: 정답! 퍼즐 통과!");

        // 퍼즐을 풀었으므로 상태를 true로 변경
        isPuzzleSolved = true;

        if (OnPuzzleSolved != null)
        {
            OnPuzzleSolved.Invoke();
        }

        // 퍼즐을 풀면 토글을 비활성화(interactable = false)하도록 변경
        foreach (Toggle sw in switches)
        {
            sw.interactable = false; // enabled = false 대신 interactable = false 사용
        }
    }

    // 'X' 닫기 버튼 또는 'ESC' 키가 호출할 함수
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