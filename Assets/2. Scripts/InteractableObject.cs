using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("상호작용 설정")]
    public KeyCode interactionKey = KeyCode.F;

    [Header("연결")]
    public GameObject panelToOpen;
    public GameObject interactionPromptUI;

    private bool isPlayerNear = false;
    
    // ▼▼▼ 1. 퍼즐이 풀렸는지 기억할 변수 추가 ▼▼▼
    private bool isPuzzleSolved = false; 

    void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // F키 입력 확인
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            // ▼▼▼ 2. 퍼즐이 아직 안 풀렸을 때만 패널이 열리도록 수정 ▼▼▼
            if (panelToOpen != null && !isPuzzleSolved) 
            {
                panelToOpen.SetActive(true);
            }
        }

        // "PRESS F" UI 텍스트 관리
        if (interactionPromptUI != null)
        {
            bool isPanelOpen = (panelToOpen != null && panelToOpen.activeSelf);

            // ▼▼▼ 3. "PRESS F"는 [가깝고] [패널이 닫혀있고] [아직 안 풀었을 때]만 켜져야 함 ▼▼▼
            if (isPlayerNear && !isPanelOpen && !isPuzzleSolved)
            {
                interactionPromptUI.SetActive(true);
            }
            else
            {
                interactionPromptUI.SetActive(false);
            }
        }
    }

    // ▼▼▼ 4. (가장 중요) "퍼즐이 풀렸다"는 신호를 받을 공개 '슬롯' 함수 추가 ▼▼▼
    /// <summary>
    /// SwitchPuzzleManager의 OnPuzzleSolved 이벤트에 연결할 함수입니다.
    /// </summary>
    public void OnPuzzleHasBeenSolved()
    {
        this.isPuzzleSolved = true;
        Debug.Log(this.gameObject.name + "의 퍼즐이 풀렸음으로 표시합니다.");
    }
}