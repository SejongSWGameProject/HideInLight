using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Light flashlight;          // Spot Light 연결
    public Transform cameraTransform; // 카메라 Transform
    public Camera mainCamera;

    [Header("Battery UI Canvas")]
    public GameObject BatteryUI;

    [Header("UI Settings")]
    public RectTransform uiObjectA;   // Inspector에서 연결
    public float decreaseSpeed = 20f; // 1초에 얼마나 감소할지
    public float decreasesize = 30f;

    [Header("Initial Settings")]
    public float initialRange = 80f;  // Inspector에서 초기값 설정
    public float initialAngle = 70f;   // Inspector에서 초기값 설정

    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;         // 마우스 휠 감도
    [Range(0f, 1f)] public float angleRangeFactor = 0.5f; // range 증가 시 angle 감소 비율

    public PlayerMove playerMove;
    public MonsterAI monster;

    private float range;
    private float angle;

    public LayerMask obstacleLayerMask;
    public AudioClip toggleLight;
    private AudioSource audioSource;

    [Header("Debug")]
    public bool showDebugRays = true;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;

    void Start()
    {
        if (flashlight == null) return;

        audioSource = GetComponent<AudioSource>();

        range = initialRange;  // Inspector 값 그대로 초기값으로 사용
        angle = initialAngle;

        flashlight.range = range;
        flashlight.spotAngle = angle;

        flashlight.enabled = false;
    }


    void Update()
    {
        // --- 배터리 UI 오브젝트 활성화 (한 번만) ---
        if (BatteryUI != null)
            BatteryUI.SetActive(true);

        // 마우스 왼쪽 클릭 시 토글
        if (Input.GetMouseButtonDown(0)) // 0 = 왼쪽 버튼
        {
            flashlight.enabled = !flashlight.enabled; // 켜져있으면 끄고, 꺼져있으면 켬
            audioSource.PlayOneShot(toggleLight);
            if (flashlight.enabled)
            {
                if (isMonsterInSight())
                {
                    monster.setMonsterState(2);
                }
            }
        }
        if (Input.GetMouseButtonDown(1) && uiObjectA != null && uiObjectA.sizeDelta.x >= decreasesize)
        {

            Vector2 size = uiObjectA.sizeDelta;
            Vector3 pos = uiObjectA.localPosition;

            size.x -= decreasesize;

            if (size.x < 0f) size.x = 0f; // 최소값 0 제한

            // 왼쪽으로 이동: 줄어든 값의 절반만큼
            pos.x -= decreasesize / 2f;

            uiObjectA.sizeDelta = size;
            uiObjectA.localPosition = pos;
            

            if (isMonsterInSight())
            {
                Debug.Log(monster.name + "이(가) 화면에 보입니다. (벽 없음)");
                if (monster.CheckSight())
                {
                    monster.RequestPause(3.0f);
                }
                else
                {
                    monster.setMonsterState(2);
                }
            }
            else
            {
                Debug.Log(monster.name + "이(가) 안 보입니다. (화면 밖이거나 벽에 가려짐)");

            }
            playerMove.StartCoroutine(playerMove.FlashScreen());
        }

        ScrollableLight();

        // **손전등이 켜져 있는 동안 UI 감소**
        if (flashlight.enabled && uiObjectA != null)
        {
            Vector2 size = uiObjectA.sizeDelta;
            Vector3 pos = uiObjectA.localPosition;

            float a = size.x;

            float decreaseAmount = decreaseSpeed * Time.deltaTime; // 이번 프레임에 줄일 값
            size.x -= decreaseAmount;

            if (size.x < 0f) size.x = 0f; // 최소값 0 제한

            float b = a - size.x;
            // 왼쪽으로 이동: 줄어든 값의 절반만큼
            pos.x -= b / 2f;

            uiObjectA.sizeDelta = size;
            uiObjectA.localPosition = pos;
        }
    }

    public bool isMonsterInSight()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(monster.transform.position);
        // 2. 화면 안에 있는지 3가지 조건을 모두 검사
        bool isInView = viewportPos.z > 0 &&
                        viewportPos.x >= 0 && viewportPos.x <= 1 &&
                        viewportPos.y >= 0 && viewportPos.y <= 1;

        bool isVisible = false;

        if (isInView)
        {
            // 카메라 위치에서 몬스터 위치로 향하는 방향과 거리 계산
            Vector3 directionToMonster = (monster.eyePosition.position - mainCamera.transform.position);
            float distanceToMonster = directionToMonster.magnitude;

            // Raycast 실행
            // Physics.Raycast()가 'true'를 반환하면 "장애물을 맞췄다"는 의미
            if (Physics.Raycast(mainCamera.transform.position,
                                directionToMonster.normalized,
                                distanceToMonster, // 몬스터까지만의 거리
                                obstacleLayerMask)) // "Obstacle" 레이어만 감지
            {
                // 장애물이 감지됨 (벽에 가려짐)
                isVisible = false;
            }
            else
            {
                // 장애물이 감지되지 않음 (보임)
                isVisible = true;

            }
        }
        return isVisible;
    }

    public void ScrollableLight()
    {
        // 마우스 휠 입력 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // 위로 스크롤: +, 아래로: -
        if (scroll != 0)
        {
            // range 증가/감소 (최소값 50)
            if (angle != 15f)
                range = Mathf.Max(50f, range + scroll * scrollSpeed);

            // spotAngle 증가/감소 (최소값 15)
            if (range != 50f)
                angle = Mathf.Max(15f, angle - scroll * scrollSpeed * angleRangeFactor);

            flashlight.range = range;
            flashlight.spotAngle = angle;
        }
    }

    public bool IsObjectIlluminated(GameObject target)
    {

        if (target == null || cameraTransform == null)
            return false;

        if (!flashlight.enabled)
        {
            return false;
        }

        Vector3 flashlightPosition = cameraTransform.position;
        Vector3 flashlightForward = cameraTransform.forward;
        Vector3 targetPosition = target.transform.position;

        // 1. 거리 체크
        Vector3 directionToTarget = targetPosition - flashlightPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > initialRange)
        {
            if (showDebugRays)
                Debug.DrawRay(flashlightPosition, directionToTarget.normalized * initialRange, Color.gray);
            return false;
        }

        // 2. 각도 체크 (원뿔 범위 내에 있는지)
        float angleToTarget = Vector3.Angle(flashlightForward, directionToTarget);

        if (angleToTarget > initialAngle / 3f)
        {
            if (showDebugRays)
                Debug.DrawRay(flashlightPosition, directionToTarget.normalized * distanceToTarget, Color.yellow);
            return false;
        }

        // 3. 장애물 체크 (Raycast)
        RaycastHit hit;
        if (Physics.Raycast(flashlightPosition, directionToTarget.normalized, out hit, distanceToTarget, obstacleLayerMask))
        {
            // 중간에 장애물이 있음
            if (showDebugRays)
                Debug.DrawRay(flashlightPosition, directionToTarget.normalized * hit.distance, missColor);
            return false;
        }

        // 모든 조건 통과 - 객체가 빛을 받고 있음
        if (showDebugRays)
            Debug.DrawRay(flashlightPosition, directionToTarget.normalized * distanceToTarget, hitColor);

        return true;
    }
}
