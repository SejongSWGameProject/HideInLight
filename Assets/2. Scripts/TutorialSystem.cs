using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject tutorialPanel; // 튜토리얼(설명) 패널 UI

    [Header("퍼즐 UI 제한 설정")]
    public GameObject wirePuzzleAnchor;   // 전선 퍼즐 UI 부모 오브젝트
    public GameObject switchPuzzleAnchor; // 스위치 퍼즐 UI 부모 오브젝트

    [Header("설정")]
    public bool hasTutorial = true; 

    private bool isTutorialOpen = false;

    void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 탭 키를 눌렀을 때
        if (hasTutorial && Input.GetKeyDown(KeyCode.Tab))
        {
            // 1. 퍼즐이 켜져 있는지 확인
            if (IsAnyPuzzleOpen())
            {
                return; // 퍼즐이 켜져 있다면 여기서 중단 (튜토리얼 안 켬)
            }

            // 2. 퍼즐이 없을 때만 튜토리얼 토글
            ToggleTutorial();
        }
    }

    // 퍼즐이 하나라도 켜져 있는지 확인하는 함수
    bool IsAnyPuzzleOpen()
    {
        if (wirePuzzleAnchor != null && wirePuzzleAnchor.activeSelf) return true;
        if (switchPuzzleAnchor != null && switchPuzzleAnchor.activeSelf) return true;
        
        return false;
    }

    void ToggleTutorial()
    {
        isTutorialOpen = !isTutorialOpen; // 상태 반전

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(isTutorialOpen);
        }
    }

    public void GetTutorialItem()
    {
        hasTutorial = true;
    }
}