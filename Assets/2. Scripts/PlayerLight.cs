using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Light flashlight;          // Spot Light 연결
    public Transform cameraTransform; // 카메라 Transform
    public Camera mainCamera;
    [Header("Initial Settings")]
    public float initialRange = 100f;  // Inspector에서 초기값 설정
    public float initialAngle = 45f;   // Inspector에서 초기값 설정

    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;         // 마우스 휠 감도
    [Range(0f, 1f)] public float angleRangeFactor = 0.5f; // range 증가 시 angle 감소 비율

    public PlayerMove playerMove;
    public MonsterAI monster;

    private float range;
    private float angle;

    public LayerMask obstacleLayerMask;

    void Start()
    {
        if (flashlight == null) return;

        range = initialRange;  // Inspector 값 그대로 초기값으로 사용
        angle = initialAngle;

        flashlight.range = range;
        flashlight.spotAngle = angle;

        flashlight.enabled = false;
    }


    void Update()
    {
        // 마우스 왼쪽 클릭 시 토글
        if (Input.GetMouseButtonDown(0)) // 0 = 왼쪽 버튼
        {
            flashlight.enabled = !flashlight.enabled; // 켜져있으면 끄고, 꺼져있으면 켬
        }
        if (Input.GetMouseButtonDown(1))
        {
            //RotateFlashlight();
            //StopAllCoroutines();  // 혹시 중복 실행 방지
                                  // 전등 앞쪽 방향으로 Raycast

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
            // (else: 1단계(화면 밖)에서 탈락했으므로 isVisible은 어차피 false)


            // --- 최종 결과 ---
            if (isVisible)
            {
                Debug.Log(monster.name + "이(가) 화면에 보입니다. (벽 없음)");
                monster.RequestPause(3.0f);
            }
            else
            {
                Debug.Log(monster.name + "이(가) 안 보입니다. (화면 밖이거나 벽에 가려짐)");
            }

            playerMove.StartCoroutine(playerMove.FlashScreen());
        }

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
}
