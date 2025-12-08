using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject tutorialPanel; // 튜토리얼(설명) 패널 UI

    [Header("설정")]
    public bool hasTutorial = true;

    void Start()
    {
        // 게임 시작 시 튜토리얼 창 끄기
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 튜토리얼 권한이 없거나 패널이 연결 안 되어 있으면 실행 X
        if (!hasTutorial || tutorialPanel == null) return;

        // 1. Tab 키를 '누르고 있는가?' (true/false)
        bool isTabPressed = Input.GetKey(KeyCode.Tab);

        // 2. 현재 패널의 상태가 키 입력 상태와 다를 때만 변경 (최적화)
        // (예: 키를 누르고 있는데 패널이 꺼져있다면 -> 켠다)
        if (tutorialPanel.activeSelf != isTabPressed)
        {
            tutorialPanel.SetActive(isTabPressed);
        }
    }

    // 아이템 습득 시 호출
    public void GetTutorialItem()
    {
        hasTutorial = true;
    }
}