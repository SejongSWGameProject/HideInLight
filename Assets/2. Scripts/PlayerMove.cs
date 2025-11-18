using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    Rigidbody rb;

    [Header("Rotate")]
    public float mouseSpeed; // 여기에 저장된 감도 값이 들어갑니다
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

    float currentX;
    float currentY;
    float xVelocity;
    float yVelocity;
    public float smoothTime = 0.1f; // 원하는 부드러움

    [Header("Flash Effect")]
    public Image flashImage;           // Canvas의 흰색 Image 연결
    public float flashDuration = 0.5f; // 플래시 지속 시간

    Vector3 originalScale;

    void Start()
    {
        hiddenMouseCursor();
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = Camera.main;
        if (cam != null)
        {
            xRotation = cam.transform.eulerAngles.x;
            yRotation = cam.transform.eulerAngles.y;
        }

        if (flashlight != null)
            flashlightRotation = flashlight.transform.localEulerAngles;

        originalY = transform.position.y;

        Invoke(nameof(EnableMouseLook), 0.5f);

        originalScale = transform.localScale; // 원래 스케일 저장

        // =========================================================
        // [추가된 부분] 메인 화면에서 설정한 마우스 감도 불러오기
        // =========================================================
        // "MouseSens"라는 이름으로 저장된 값이 있으면 가져옵니다.
        // 저장된 게 없으면(처음 켰을 때) 기본값 5.0f를 씁니다.
        // * 50f를 곱해서 슬라이더 값(1~10)을 실제 게임 감도(50~500)로 변환합니다.
        mouseSpeed = PlayerPrefs.GetFloat("MouseSens", 5.0f) * 50.0f;

        // (선택사항) 만약 손전등 속도도 마우스 감도에 비례하게 하고 싶다면 아래 주석을 푸세요.
        // flashlightRotateSpeed = mouseSpeed * 2.0f; 
    }

    // --- 플래시 코루틴 ---
    public IEnumerator FlashScreen()
    {
        // Image를 흰색으로 불투명하게
        Color c = flashImage.color;
        c.a = 1f;
        flashImage.color = c;

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / flashDuration);
            flashImage.color = c;
            yield return null;
        }

        // 마지막으로 완전히 투명
        c.a = 0f;
        flashImage.color = c;
    }

    void EnableMouseLook()
    {
        canLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Getinput();
        HandleCrouch();

        if (!Input.GetMouseButton(1) && canLook)
            Rotate();

        if (Input.GetMouseButton(1))
        {
            //RotateFlashlight();
            
        }

        // --- 추가: 스페이스바 입력 시 플래시 효과 ---
        if (Input.GetKeyDown(KeyCode.Space) && flashImage != null)
        {
            //StopAllCoroutines();  // 혹시 중복 실행 방지
            //StartCoroutine(FlashScreen());
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKey(crouchKey))
        {
            if (!isCrouched)
            {
                isCrouched = true;

                // 위치 y값 절반
                Vector3 pos = transform.position;
                pos.y = originalY * 0.5f;
                transform.position = pos;

                // y축 스케일 절반, x/z는 그대로
                Vector3 scale = transform.localScale;
                scale.y = originalScale.y * 0.5f;
                transform.localScale = scale;
            }
        }
        else
        {
            if (isCrouched)
            {
                isCrouched = false;

                // 위치 원래대로
                Vector3 pos = transform.position;
                pos.y = originalY;
                transform.position = pos;

                // y축 스케일 원래대로
                Vector3 scale = transform.localScale;
                scale.y = originalScale.y;
                transform.localScale = scale;
            }
        }
    }

    void FixedUpdate()
    {
        // 이동 입력
        Vector3 moveVec = transform.forward * v + transform.right * h;
        moveVec = moveVec.normalized * moveSpeed;

        // Rigidbody에 이동 적용, y축 속도 유지
        rb.linearVelocity = new Vector3(moveVec.x, rb.linearVelocity.y, moveVec.z);
    }

    void Getinput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    void Rotate()
    {
        if (!canLook) return;

        // mouseSpeed 변수가 Start()에서 설정된 값으로 적용됩니다.
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        currentX = Mathf.SmoothDampAngle(currentX, xRotation, ref xVelocity, smoothTime);
        currentY = Mathf.SmoothDampAngle(currentY, yRotation, ref yVelocity, smoothTime);

        cam.transform.rotation = Quaternion.Euler(currentX, currentY, 0);
        transform.rotation = Quaternion.Euler(0, currentY, 0);
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

    // --- 반발력 무시 코드 ---
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.y > 0f) vel.y = 0f;
        rb.linearVelocity = vel;
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.y > 0f) vel.y = 0f;
        rb.linearVelocity = vel;
    }
}