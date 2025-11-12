using UnityEngine;
using UnityEngine.UI; // UI 텍스트를 사용하기 위해 필요

public class PlayerInteractor : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("플레이어의 메인 카메라 Transform")]
    public Transform playerCameraTransform;

    [Tooltip("상호작용이 가능한 최대 거리")]
    public float interactionDistance = 1000.0f;

    [Tooltip("상호작용할 레이어 (예: Interactable)")]
    public LayerMask interactionLayer; // [중요!]

    [Tooltip("상호작용 키")]
    public KeyCode interactionKey = KeyCode.F;

    [Header("UI (선택 사항)")]
    [Tooltip("상호작용 안내 텍스트 (예: 'Press [F] to interact')")]
    public GameObject interactionPromptUI;

    // 현재 바라보고 있는 상호작용 가능한 객체
    private InteractableObject currentInteractable;

    void Start()
    {
        // 카메라가 연결 안 됐으면 자신(카메라)의 Transform을 사용
        if (playerCameraTransform == null)
        {
            playerCameraTransform = transform;
        }

        // 시작할 때 UI 숨기기
        if (interactionPromptUI != null)
        {
            interactionPromptUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 1. 레이(Ray) 생성: 카메라 위치에서 카메라 정면으로
        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        RaycastHit hit; // 레이에 맞은 물체의 정보

        bool foundInteractable = false;
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        // 2. 레이 발사: (레이, 맞은 정보, 거리, 감지할 레이어)
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // 3. 맞은 물체에서 'InteractableObject' 스크립트 가져오기
            // (참고: 이 스크립트는 'Interact()' 함수를 가진다고 가정합니다)
            //Debug.Log("ray hit!: "+hit.transform.name);
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                // 4. 상호작용 가능한 물체를 찾음
                foundInteractable = true;
                currentInteractable = interactable; // 현재 객체로 저장

                //Debug.Log(currentInteractable.transform.name);
                // 5. 상호작용 키(E)를 눌렀는지 확인
                if (Input.GetKeyDown(interactionKey))
                {
                    // 6. 상호작용 실행!
                    // (이 객체의 UnityEvent가 SwitchPuzzleManager의 OpenPuzzlePopup()을 호출)

                    Debug.Log("playerinter getkey");
                    interactable.Interact();
                }

                interactable.UpdatePromptUI(true);
            }
        }

        // 7. 상호작용 UI 관리
        //if (interactionPromptUI != null)
        //{
        //    // 바라보고 있으면 UI 켜기
        //    if (foundInteractable)
        //    {

        //        // (필요시 interactable.promptMessage 같은 변수를 받아와 텍스트 변경
        //        interactionPromptUI.gameObject.SetActive(true);

        //    }
        //    else // 바라보고 있지 않으면 UI 끄기
        //    {
        //        interactionPromptUI.gameObject.SetActive(false);
        //        currentInteractable = null;
        //    }
        //}

        
    }
}