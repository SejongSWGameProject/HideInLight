using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject tutorialPanel; // 튜토리얼(설명) 패널 UI를 여기에 드래그

    [Header("설정")]
    // 만약 튜토리얼을 처음부터 가지고 있는 게 아니라 나중에 얻어야 한다면 false로 바꾸세요.
    public bool hasTutorial = true; 

    private bool isTutorialOpen = false; // 현재 켜져 있는지 확인용

    void Start()
    {
        // 게임 시작 시 튜토리얼 창이 켜져있다면 강제로 끕니다.
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 튜토리얼 기능이 활성화되어 있고, Tab 키를 눌렀을 때
        if (hasTutorial && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleTutorial();
        }
    }

    void ToggleTutorial()
    {
        isTutorialOpen = !isTutorialOpen; // 상태 반전 (켜짐 <-> 꺼짐)

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(isTutorialOpen); // UI 껐다 켜기
        }
    }

    // (옵션) 나중에 아이템처럼 습득하게 만들고 싶다면 사용
    public void GetTutorialItem()
    {
        hasTutorial = true;
    }
}