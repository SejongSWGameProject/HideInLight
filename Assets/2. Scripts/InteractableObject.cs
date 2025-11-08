using UnityEngine;
using UnityEngine.Events; // UnityEvent를 사용하기 위해 필요합니다.

/// <summary>
/// 3D 오브젝트와 F키로 상호작용하여,
/// 1. "PRESS F" UI를 띄우고
/// 2. 지정된 패널(3D 또는 2D)을 엽니다.
/// 3. 퍼즐이 풀리면 상호작용을 영구적으로 비활성화합니다.
/// </summary>
public class InteractableObject : MonoBehaviour
{
    [Header("상호작용 설정")]
    [Tooltip("상호작용에 사용할 키")]
    public KeyCode interactionKey = KeyCode.F; // F 키로 설정

    [Header("연결")]
    [Tooltip("상호작용 시 켤 UI 패널 (SwitchPuzzle_Panel 또는 PopupAnchor)")]
    public GameObject panelToOpen;

    [Tooltip("가까이 갔을 때 켤 상호작용 UI (예: 'F키' 텍스트)")]
    public GameObject interactionPromptUI;

    [Header("퍼즐 상태")]
    [Tooltip("퍼즐이 이미 해결되었는지 여부")]
    private bool isPuzzleSolved = false; // 퍼즐이 풀리면 true가 됨

    // 플레이어가 범위 안에 있는지 여부
    private bool isPlayerNear = false;

    // 1. 시작할 때 UI 숨기기
    void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    // 2. 플레이어가 범위(Trigger) 안에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 오브젝트가 "Player" 태그를 가졌는지 확인
        // 그리고 퍼즐이 아직 풀리지 않았는지 확인
        if (other.CompareTag("Player") && !isPuzzleSolved)
        {
            isPlayerNear = true;
            
            // 패널이 닫혀 있을 때만 "PRESS F" 텍스트를 켭니다.
            if (panelToOpen != null && !panelToOpen.activeSelf)
            {
                UpdatePromptUI(true); // "PRESS F" UI 켜기
            }
        }
    }

    // 3. 플레이어가 범위(Trigger) 밖으로 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            UpdatePromptUI(false); // "F키" UI 끄기
        }
    }

    // 4. 매 프레임마다 키 입력 및 상태 확인
    private void Update()
    {
        // 플레이어가 범위 안에 있고 퍼즐이 풀리지 않았을 때
        if (isPlayerNear && !isPuzzleSolved)
        {
            // F키를 눌렀다면
            if (Input.GetKeyDown(interactionKey))
            {
                // 패널을 켠다!
                if (panelToOpen != null)
                {
                    panelToOpen.SetActive(true);
                }
            }

            // (1번 문제 해결) 패널이 열려있는지 매번 확인
            bool isPanelOpen = (panelToOpen != null && panelToOpen.activeSelf);
            
            // "PRESS F" 텍스트 갱신
            // (패널이 닫혀있고, 퍼즐이 안 풀렸을 때만 true)
            UpdatePromptUI(!isPanelOpen);
        }
        else
        {
            // 플레이어가 범위 밖이거나 퍼즐이 풀렸으면 무조건 끔
            UpdatePromptUI(false);
        }
    }

    /// <summary>
    /// "PRESS F" 텍스트 UI의 활성화 상태를 안전하게 변경합니다.
    /// </summary>
    void UpdatePromptUI(bool show)
    {
        if (interactionPromptUI != null && interactionPromptUI.activeSelf != show)
        {
            interactionPromptUI.SetActive(show);
        }
    }
    
    // 5. (가장 중요) 퍼즐 매니저가 "퍼즐 풀렸다!"고 호출해줄 함수
    /// <summary>
    /// (UnityEvent용) 퍼즐이 풀렸다고 표시하고, 이 상호작용을 영구적으로 비활성화합니다.
    /// </summary>
    public void OnPuzzleHasBeenSolved()
    {
        isPuzzleSolved = true;
        
        // F키 텍스트가 켜져있다면 즉시 끈다.
        UpdatePromptUI(false);
    }
}