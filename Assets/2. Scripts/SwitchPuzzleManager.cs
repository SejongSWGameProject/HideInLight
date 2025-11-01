using UnityEngine;
using UnityEngine.Events;
using System.Linq;

// ▼▼▼ 클래스 이름이 바뀌었습니다! ▼▼▼
public class SwitchPuzzleManager : MonoBehaviour 
{
    [Header("연결")]
    [Tooltip("퍼즐에 사용할 4개의 스위치를 순서대로 연결하세요.")]
    public ClickableSwitch[] switches; // 4개의 스위치를 담을 배열

    [Header("정답")]
    private bool[] answerKey = new bool[4]; 

    [Header("성공 이벤트")]
    [Tooltip("정답을 맞췄을 때 실행할 이벤트를 연결하세요.")]
    public UnityEvent OnPuzzleSolved; // 정답 맞췄을 때 실행할 이벤트

    void Start()
    {
        if (switches.Length != 4)
        {
            Debug.LogError("SwitchPuzzleManager: 스위치 4개를 모두 연결해야 합니다!");
            return;
        }
        GenerateRandomAnswer();
    }

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

    // 스위치가 클릭될 때마다 호출될 함수
    public void CheckPuzzleSolution()
    {
        for (int i = 0; i < switches.Length; i++)
        {
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

        foreach (ClickableSwitch sw in switches)
        {
            sw.enabled = false; 
        }
    }
}