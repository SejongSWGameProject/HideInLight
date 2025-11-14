using UnityEngine;

public class InteractableDevice : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private GameObject puzzleUI; // 퍼즐 UI 패널
    [SerializeField] private WireConnectionPuzzle wireConnectionPuzzle; // 전선 퍼즐
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("UI Elements")]
    [SerializeField] private GameObject interactionPrompt; // "E를 눌러 상호작용" UI

    private Camera playerCamera;
    private bool isPlayerInRange = false;
    private bool isPuzzleActive = false;
    

    void Start()
    {
        playerCamera = Camera.main;

        

        if (puzzleUI != null)
            puzzleUI.SetActive(false);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        CheckPlayerDistance();

        if (isPlayerInRange && Input.GetKeyDown(interactionKey) && !isPuzzleActive)
        {
            OpenPuzzle();
        }
    }

    void CheckPlayerDistance()
    {
        if (playerCamera == null) return;

        // 플레이어와의 거리 체크
        float distance = Vector3.Distance(transform.position, playerCamera.transform.position);

        // Raycast로 플레이어가 장치를 보고 있는지 확인
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isPlayerInRange = true;
                if (interactionPrompt != null && !isPuzzleActive)
                    interactionPrompt.SetActive(true);
                return;
            }
        }

        isPlayerInRange = false;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    public void OpenPuzzle()
    {
        isPuzzleActive = true;

        if (puzzleUI != null)
            puzzleUI.SetActive(true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // 전선 퍼즐 열기
        if (wireConnectionPuzzle != null)
            wireConnectionPuzzle.OpenPuzzle();

        

        // 마우스 커서 보이기
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("InteractableObject OpenPuzzle()");
    }

    public void ClosePuzzle()
    {
        isPuzzleActive = false;

        if (puzzleUI != null)
            puzzleUI.SetActive(false);
        

        // 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Time.timeScale = 1f; // 게임 재개 (선택사항)
    }

    public void OnPuzzleSolved()
    {
        // 퍼즐이 해결되었을 때 호출
        Debug.Log("퍼즐 해결!");
        ClosePuzzle();

        // 여기에 문 열기, 아이템 획득 등의 로직 추가
        // 예: GetComponent<Door>()?.Open();
    }

    // Gizmo로 상호작용 범위 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}