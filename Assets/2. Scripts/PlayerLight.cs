using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Light flashlight;          // Spot Light 연결
    public Transform cameraTransform; // 카메라 Transform
    public Camera mainCamera;

    [Header("Battery UI Canvas")]
    public GameObject BatteryUI;

    [Header("UI Settings")]
    public RectTransform uiObjectA;
    public float decreaseSpeed = 20f;
    public float decreasesize = 30f;

    [Header("Initial Settings")]
    public float initialRange = 80f;
    public float initialAngle = 70f;

    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;
    [Range(0f, 1f)] public float angleRangeFactor = 0.5f;

    public PlayerMove playerMove;

    // [삭제됨] 이제 리스트를 쓰므로 단일 변수는 필요 없습니다.
    // public MonsterAI monster; 

    private float range;
    private float angle;
    private Color originalColor;

    public LayerMask obstacleLayerMask;
    public AudioClip toggleLight;
    private AudioSource audioSource;

    [Header("Debug")]
    public bool showDebugRays = true;
    public Color hitColor = Color.green;
    public Color missColor = Color.red;

    public Color[] lightColors;
    public int colorIndex = 0;

    public LayerMask defaultLayer;
    public LayerMask redHiddenLayer;
    public LayerMask greenHiddenLayer;

    void Start()
    {
        if (flashlight == null) return;

        if (lightColors == null || lightColors.Length == 0)
        {
            lightColors = new Color[] { flashlight.color, Color.red, Color.green };
        }
        else
        {
            lightColors[0] = flashlight.color;
        }

        audioSource = GetComponent<AudioSource>();

        range = initialRange;
        angle = initialAngle;

        flashlight.range = range;
        flashlight.spotAngle = angle;

        flashlight.enabled = false;
    }


    void Update()
    {
        if (flashlight.enabled && Input.GetKeyDown(KeyCode.E))
        {
            if (flashlight.color == originalColor)
                flashlight.color = Color.red;
            else if (flashlight.color == Color.red)
                flashlight.color = Color.green;
            else
                flashlight.color = originalColor;
        }

        // 마우스 왼쪽 클릭 시 토글
        if (Input.GetMouseButtonDown(0) && uiObjectA.sizeDelta.x > 0)
        {
            flashlight.enabled = !flashlight.enabled;

            if (!flashlight.enabled)
            {
                mainCamera.cullingMask = defaultLayer;
            }

            audioSource.PlayOneShot(toggleLight);

            // [수정된 부분] 켜는 순간 화면에 있는 '모든' 괴물을 자극함
            if (flashlight.enabled)
            {
                foreach (MonsterAI m in MonsterAI.allMonsters)
                {
                    if (IsSpecificMonsterInSight(m))
                    {
                        m.setMonsterState(MonsterState.CHASE);
                        Debug.Log("빛으로 몬스터 발견: " + m.name);
                    }
                }
            }
        }

        // 마우스 오른쪽 클릭 (섬광/스턴 기능)
        if (Input.GetMouseButtonDown(1) && uiObjectA != null && uiObjectA.sizeDelta.x >= decreasesize)
        {
            Vector2 size = uiObjectA.sizeDelta;
            Vector3 pos = uiObjectA.localPosition;

            size.x -= decreasesize;
            if (size.x < 0f) size.x = 0f;

            pos.x -= decreasesize / 2f;

            uiObjectA.sizeDelta = size;
            uiObjectA.localPosition = pos;

            playerMove.StartCoroutine(playerMove.FlashScreen());

            // [수정된 부분] 모든 괴물에 대해 판정
            bool anyMonsterStunned = false;

            foreach (MonsterAI m in MonsterAI.allMonsters)
            {
                // 1. 내 화면(카메라)에 이 괴물이 보이는가?
                if (IsSpecificMonsterInSight(m))
                {
                    // 2. 괴물도 나를 볼 수 있는가? (벽 뒤가 아닌지 등, MonsterAI의 CheckSight 활용)
                    // 주의: MonsterAI의 CheckSight()가 현재 플레이어를 보고 있는지 내부적으로 판단함
                    if (m.CheckSight())
                    {
                        m.RequestPause(3.0f); // 스턴
                        anyMonsterStunned = true;
                    }
                    else
                    {
                        m.setMonsterState(MonsterState.CHASE); // 못 봤는데 빛 쐈으면 추격
                    }
                }
            }

            if (!anyMonsterStunned)
            {
                // 디버깅용 (필요 없으면 삭제)
                // Debug.Log("화면 내에 스턴 걸린 몬스터가 없습니다.");
            }
        }

        if (flashlight.enabled)
        {
            ScrollableLight();
            UpdateCullingMask();
        }

        if (flashlight.enabled && uiObjectA != null)
        {
            Vector2 size = uiObjectA.sizeDelta;
            Vector3 pos = uiObjectA.localPosition;

            float a = size.x;
            float decreaseAmount = decreaseSpeed * Time.deltaTime;
            size.x -= decreaseAmount;

            if (size.x < 0f)
            {
                size.x = 0f;
                flashlight.enabled = false;
            }

            float b = a - size.x;
            pos.x -= b / 2f;

            uiObjectA.sizeDelta = size;
            uiObjectA.localPosition = pos;
        }
    }

    // [함수 수정] 특정 괴물을 매개변수로 받아서 그 놈이 보이는지 체크
    public bool IsSpecificMonsterInSight(MonsterAI targetMonster)
    {
        if (targetMonster == null) return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(targetMonster.transform.position);

        // 1. 화면(Viewport) 좌표계 안에 있는지 검사
        bool isInView = viewportPos.z > 0 &&
                        viewportPos.x >= 0 && viewportPos.x <= 1 &&
                        viewportPos.y >= 0 && viewportPos.y <= 1;

        if (isInView)
        {
            // 2. 레이캐스트로 장애물 검사
            Vector3 directionToMonster = (targetMonster.eyePosition.position - mainCamera.transform.position);
            float distanceToMonster = directionToMonster.magnitude;

            // 괴물까지 거리가 너무 멀면(예: 안개 속) 못 본 것으로 처리하려면 여기서 거리 체크 추가 가능

            if (Physics.Raycast(mainCamera.transform.position,
                                directionToMonster.normalized,
                                distanceToMonster,
                                obstacleLayerMask))
            {
                return false; // 벽에 막힘
            }
            else
            {
                return true; // 벽 없음 = 보임
            }
        }
        return false; // 화면 밖
    }

    public void UpdateCullingMask()
    {
        if (colorIndex == 0) mainCamera.cullingMask = defaultLayer;
        else if (colorIndex == 1) mainCamera.cullingMask = defaultLayer | redHiddenLayer;
        else if (colorIndex == 2) mainCamera.cullingMask = defaultLayer | greenHiddenLayer;
    }

    public void ScrollableLight()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0) colorIndex++;
            else if (scroll < 0) colorIndex--;

            int length = lightColors.Length;
            colorIndex = (colorIndex % length + length) % length;

            flashlight.color = lightColors[colorIndex];
        }
    }

    public bool IsObjectIlluminated(GameObject target)
    {
        if (target == null || cameraTransform == null) return false;
        if (!flashlight.enabled) return false;

        Vector3 flashlightPosition = cameraTransform.position;
        Vector3 flashlightForward = cameraTransform.forward;
        Vector3 targetPosition = target.transform.position;

        Vector3 directionToTarget = targetPosition - flashlightPosition;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > initialRange)
        {
            if (showDebugRays) Debug.DrawRay(flashlightPosition, directionToTarget.normalized * initialRange, Color.gray);
            return false;
        }

        float angleToTarget = Vector3.Angle(flashlightForward, directionToTarget);

        if (angleToTarget > initialAngle / 3f) // 스포트라이트 각도 체크 (조절 가능)
        {
            if (showDebugRays) Debug.DrawRay(flashlightPosition, directionToTarget.normalized * distanceToTarget, Color.yellow);
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(flashlightPosition, directionToTarget.normalized, out hit, distanceToTarget, obstacleLayerMask))
        {
            if (showDebugRays) Debug.DrawRay(flashlightPosition, directionToTarget.normalized * hit.distance, missColor);
            return false;
        }

        if (showDebugRays) Debug.DrawRay(flashlightPosition, directionToTarget.normalized * distanceToTarget, hitColor);

        return true;
    }
}