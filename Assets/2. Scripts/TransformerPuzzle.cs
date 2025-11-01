using UnityEngine;
using UnityEngine.UI;
using System.Linq; // 배열을 쉽게 비교하기 위해 필요합니다.

public class TransformerPuzzle : MonoBehaviour
{
    [Header("UI 연결")]
    // 인스펙터 창에서 여기에 스위치 4개를 끌어다 넣습니다.
    public Button[] switches; 
    
    // ▼▼▼ 이미지 대신 색깔 설정 변수 추가 ▼▼▼
    public Color switchOffColor = Color.gray;    // 꺼진 상태 색깔 (회색)
    public Color switchOnColor = Color.green;   // 켜진 상태 색깔 (초록색)
    // ▲▲▲ 여기까지 ▲▲▲

    [Header("퍼즐 정답 설정")]
    // [true, false, true, true] = (On, Off, On, On)이 정답이라는 뜻
    // 인스펙터 창에서 원하는 대로 정답을 바꿀 수 있습니다.
    [SerializeField]
    private bool[] solution = new bool[4] { true, false, true, true };

    // 현재 스위치 4개의 상태를 저장하는 배열 (false는 Off, true는 On)
    private bool[] currentStates = new bool[4];
    
    // 게임이 시작되면 (또는 이 스크립트가 활성화되면) 1번 실행됩니다.
    void Start()
    {
        // 1. 각 버튼에 클릭 기능을 코드로 직접 연결합니다.
        // "0번째 버튼(Switch1)을 누르면 -> ToggleSwitch(0) 함수를 실행해라"
        switches[0].onClick.AddListener(() => ToggleSwitch(0));
        switches[1].onClick.AddListener(() => ToggleSwitch(1));
        switches[2].onClick.AddListener(() => ToggleSwitch(2));
        switches[3].onClick.AddListener(() => ToggleSwitch(3));

        // 2. 시작할 때 모든 스위치를 'Off' 상태로 초기화합니다.
        for (int i = 0; i < 4; i++)
        {
            currentStates[i] = false; // 'Off' 상태로 설정
            UpdateSwitchVisual(i); // 'Off' 색깔로 변경
        }
    }

    /// <summary>
    /// 스위치 버튼을 클릭할 때마다 호출되는 함수
    /// </summary>
    /// <param name="switchID">몇 번째 스위치인지 (0, 1, 2, 3)</param>
    public void ToggleSwitch(int switchID)
    {
        // 1. 현재 상태를 반대로 뒤집습니다. (Off -> On, On -> Off)
        currentStates[switchID] = !currentStates[switchID];
        
        Debug.Log($"스위치 {switchID}번 상태: {currentStates[switchID]}");

        // 2. 바뀐 상태에 맞게 색깔도 바꿔줍니다.
        UpdateSwitchVisual(switchID);

        // 3. 스위치를 누를 때마다 정답인지 체크합니다.
        CheckSolution();
    }

    /// <summary>
    /// 스위치 ID를 받아서 색깔을 On/Off에 맞게 바꿔주는 함수
    /// </summary>
    private void UpdateSwitchVisual(int switchID)
    {
        // 버튼의 Image 컴포넌트를 가져옵니다.
        Image buttonImage = switches[switchID].GetComponent<Image>();
        
        // ▼▼▼ 이미지 대신 색깔로 변경하는 코드 ▼▼▼
        if (currentStates[switchID] == true) // On 상태라면
        {
            buttonImage.color = switchOnColor; // '켜진' 색깔로 변경
        }
        else // Off 상태라면
        {
            buttonImage.color = switchOffColor; // '꺼진' 색깔로 변경
        }
        // ▲▲▲ 여기까지 ▲▲▲
    }

    /// <summary>
    /// 현재 스위치 상태가 정답과 일치하는지 확인하는 함수
    /// </summary>
    private void CheckSolution()
    {
        // currentStates 배열과 solution 배열이 1:1로 똑같은지 비교
        bool isSolved = currentStates.SequenceEqual(solution);

        if (isSolved)
        {
            Debug.Log("<color=cyan>===== 퍼즐 성공! =====</color>");
            // 여기에 퍼즐 성공 시 할 일을 적으세요.
            // 예: gameObject.SetActive(false); // 퍼즐 패널 끄기
        }
    }
}