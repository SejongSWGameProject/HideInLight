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
    private Color originalColor; // Start에서 초기값 저장

    public LayerMask obstacleLayerMask;
    public AudioClip toggleLight;
    private AudioSource audioSource;

    [Header("Debug")]
    public bool showDebugRays = true;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;

    // 색상 목록을 배열로 관리 (인스펙터에서 설정 가능)
    // 0: Original, 1: Red, 2: Green
    public Color[] lightColors;

    public int colorIndex = 0;

    public LayerMask defaultLayer; // 기본적으로 비출 레이어 (Default, Player 등)
    public LayerMask redHiddenLayer; // 빨간색 숨겨진 오브젝트 레이어
    public LayerMask greenHiddenLayer; // 초록색 숨겨진 오브젝트 레이어

    void Start()
    {
        if (flashlight == null) return;

        if (lightColors == null || lightColors.Length == 0)
        {
            lightColors = new Color[] { flashlight.color, Color.red, Color.green };
        }
        else
        {
            // 배열의 첫 번째 색상을 현재 라이트 색상으로 저장 (OriginalColor)
            lightColors[0] = flashlight.color;
        }

        audioSource = GetComponent<AudioSource>();

        range = initialRange;  // Inspector 값 그대로 초기값으로 사용
        angle = initialAngle;

        flashlight.range = range;
        flashlight.spotAngle = angle;

        flashlight.enabled = false;
    }


    void Update()
    {

        if (flashlight.enabled && Input.GetKeyDown(KeyCode.E))
        {
            if(flashlight.color == originalColor)
                flashlight.color = Color.red; // 빨간색으로 변경
            else if (flashlight.color == Color.red)
                flashlight.color = Color.green; // 초록색으로 변경
            else
                flashlight.color = originalColor; // 기존색으로 변경
        }

        // 마우스 왼쪽 클릭 시 토글
        if (Input.GetMouseButtonDown(0) && uiObjectA.sizeDelta.x > 0) // 0 = 왼쪽 버튼
        {
            flashlight.enabled = !flashlight.enabled; // 켜져있으면 끄고, 꺼져있으면 켬
            if (!flashlight.enabled)
            {
                mainCamera.cullingMask = defaultLayer;
            }
            audioSource.PlayOneShot(toggleLight);
            if (flashlight.enabled)
            {
                if (isMonsterInSight())
                {
                    monster.setMonsterState(2);
                    Debug.Log("pL103");
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
                    Debug.Log("pl133");
                }
            }
            else
            {
                Debug.Log(monster.name + "이(가) 안 보입니다. (화면 밖이거나 벽에 가려짐)");

            }
            playerMove.StartCoroutine(playerMove.FlashScreen());
        }

        if (flashlight.enabled)
        {
            ScrollableLight();
            UpdateCullingMask();

        }

        // **손전등이 켜져 있는 동안 UI 감소**
        if (flashlight.enabled && uiObjectA != null)
        {
            Vector2 size = uiObjectA.sizeDelta;
            Vector3 pos = uiObjectA.localPosition;

            float a = size.x;

            float decreaseAmount = decreaseSpeed * Time.deltaTime; // 이번 프레임에 줄일 값
            size.x -= decreaseAmount;

            if (size.x < 0f)
            {
                size.x = 0f;
                flashlight.enabled = false;
            }

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

    public void UpdateCullingMask()
    {
        // 2. 보이는 레이어 변경 (비트 연산 | 을 사용하여 레이어 합치기)
        if (colorIndex == 0) // 기본 (하얀 빛)
        {
            // 기본 레이어만 비춤
            mainCamera.cullingMask = defaultLayer;
        }
        else if (colorIndex == 1) // 빨간 빛
        {
            // 기본 레이어 + 빨간 숨겨진 레이어를 같이 비춤
            mainCamera.cullingMask = defaultLayer | redHiddenLayer;
        }
        else if (colorIndex == 2) // 초록 빛
        {
            // 기본 레이어 + 초록 숨겨진 레이어를 같이 비춤
            mainCamera.cullingMask = defaultLayer | greenHiddenLayer;
        }
    }
    public void ScrollableLight()
    {
        // 마우스 휠 입력 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // 스크롤 방향에 따라 인덱스 증감
            if (scroll > 0) colorIndex++;
            else if (scroll < 0) colorIndex--;

            // 배열 길이(Length)에 맞춰 안전하게 순환 계산
            int length = lightColors.Length;
            colorIndex = (colorIndex % length + length) % length;

            // 색상 적용 (단 한 줄로 처리 가능)
            flashlight.color = lightColors[colorIndex];
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
