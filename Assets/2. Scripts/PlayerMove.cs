using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Ctrl_Raycast : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 50f; // 아주 빠른 속도 가능

    [Header("Mouse Settings")]
    public float mouseSpeed = 200f;
    public Camera cam;

    Rigidbody rb;
    float yRotation;
    float xRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (cam == null)
            cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 초기 회전값 가져오기
        yRotation = transform.eulerAngles.y;
        xRotation = cam.transform.localEulerAngles.x;

        // Unity EulerAngles는 0~360 범위 → -180~180 범위로 변환
        if (xRotation > 180) xRotation -= 360f;
    }

    void Update()
    {
        Rotate();

        // 스페이스바를 누르면 플레이어와 카메라 180도 뒤로 회전
        if (Input.GetKeyDown(KeyCode.Space))
        {
            yRotation += 180f;
            // 범위를 0~360으로 맞춤
            if (yRotation > 360f) yRotation -= 360f;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        if (cam != null)
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;
        Vector3 targetPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;

        // Raycast로 이동 경로에 벽이 있는지 체크
        if (!Physics.Raycast(rb.position, moveDir, moveDir.magnitude))
        {
            rb.MovePosition(targetPos);
        }
        else
        {
            // 벽이 있으면 충돌 위치까지 이동
            RaycastHit hit;
            if (Physics.Raycast(rb.position, moveDir, out hit, moveDir.magnitude))
            {
                rb.MovePosition(hit.point - moveDir * 0.01f); // 0.01 유격 확보
            }
        }
    }
}