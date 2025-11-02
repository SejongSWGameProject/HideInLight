using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Ctrl : MonoBehaviour
{
    Rigidbody rb;

    [Header("Rotate")]
    public float mouseSpeed;
    float yRotation;
    float xRotation;
    Camera cam;

    [Header("Move")]
    public float moveSpeed;
    float h;
    float v;

    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightRotateSpeed = 10f; // 적절히 조절
    Vector3 flashlightRotation; // 누적 회전을 위해

    bool canLook = false;  // 마우스 입력 가능 여부

    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.Z; // 앉기 키
    private float originalY; // 초기 y 위치 저장

    private bool isCrouched = false;

    void Start()
    {
        hiddenMouseCursor();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = Camera.main;

        if (cam != null)
        {
            Vector3 camAngles = cam.transform.eulerAngles;
            xRotation = cam.transform.eulerAngles.x;
            yRotation = cam.transform.eulerAngles.y;
        }

        if (flashlight != null)
            flashlightRotation = flashlight.transform.localEulerAngles;

        // 초기 y 위치 저장
        originalY = transform.position.y;

        // 게임 시작 후 0.5초 뒤부터 마우스 입력 허용
        Invoke(nameof(EnableMouseLook), 0.5f);
    }

    void EnableMouseLook()
    {
        canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Getinput();

        // 앉기 처리
        HandleCrouch();

        // 평소 마우스 → Player 회전
        if (!Input.GetMouseButton(1) && canLook)
            Rotate();

        // 우클릭 시 → 손전등 회전
        if (Input.GetMouseButton(1))
            RotateFlashlight();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("클릭됨!");
                }
            }
        }
    }

    void HandleCrouch()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(crouchKey))
        {
            // 키를 누르고 있으면 절반 높이
            pos.y = originalY * 0.5f;
        }
        else
        {
            // 키를 떼면 원래 높이
            pos.y = originalY;
        }

        transform.position = pos;
    }

    void FixedUpdate()
    {
        Vector3 moveVec = transform.forward * v + transform.right * h;
        rb.linearVelocity = moveVec.normalized * moveSpeed;
    }

    void Getinput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    void Rotate()
    {
        if (!canLook) return; // 로딩 중에는 마우스 입력 무시

        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void Move()
    {

        Vector3 moveVec = transform.forward * v + transform.right * h;


        rb.MovePosition(rb.position + moveVec.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void RotateFlashlight()
    {
        if (flashlight == null) return;

        float mouseX = Input.GetAxis("Mouse X") * flashlightRotateSpeed * 0.05f;
        float mouseY = Input.GetAxis("Mouse Y") * flashlightRotateSpeed * 0.05f;

        // 누적 회전
        flashlightRotation.x -= mouseY;
        flashlightRotation.y += mouseX;
        flashlightRotation.x = Mathf.Clamp(flashlightRotation.x, -90f, 90f);

        flashlight.transform.localRotation = Quaternion.Euler(flashlightRotation);
    }

    void hiddenMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}