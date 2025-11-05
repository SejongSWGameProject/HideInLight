using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Light flashlight;          // Spot Light 연결
    public Transform cameraTransform; // 카메라 Transform

    [Header("Initial Settings")]
    public float initialRange = 150f;  // Inspector에서 초기값 설정
    public float initialAngle = 45f;   // Inspector에서 초기값 설정

    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;         // 마우스 휠 감도
    [Range(0f, 1f)] public float angleRangeFactor = 0.5f; // range 증가 시 angle 감소 비율

    private float range;
    private float angle;

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

        RaycastHit hit;

        // 전등 앞쪽 방향으로 Raycast
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            if (flashlight.enabled)
            {
                // 특정 태그 예시
                if (hit.collider.CompareTag("Monster"))
                {
                    Debug.Log("적이 빛을 받았다!");
                }
            }
        }
    }
}
