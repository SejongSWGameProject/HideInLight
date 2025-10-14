using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Ctrl_Raycast : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 50f; // ���� ���� �ӵ� ����

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

        // �ʱ� ȸ���� ��������
        yRotation = transform.eulerAngles.y;
        xRotation = cam.transform.localEulerAngles.x;

        // Unity EulerAngles�� 0~360 ���� �� -180~180 ������ ��ȯ
        if (xRotation > 180) xRotation -= 360f;
    }

    void Update()
    {
        Rotate();

        // �����̽��ٸ� ������ �÷��̾�� ī�޶� 180�� �ڷ� ȸ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            yRotation += 180f;
            // ������ 0~360���� ����
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

        // Raycast�� �̵� ��ο� ���� �ִ��� üũ
        if (!Physics.Raycast(rb.position, moveDir, moveDir.magnitude))
        {
            rb.MovePosition(targetPos);
        }
        else
        {
            // ���� ������ �浹 ��ġ���� �̵�
            RaycastHit hit;
            if (Physics.Raycast(rb.position, moveDir, out hit, moveDir.magnitude))
            {
                rb.MovePosition(hit.point - moveDir * 0.01f); // 0.01 ���� Ȯ��
            }
        }
    }
}