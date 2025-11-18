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

        originalScale = transform.localScale; 

        // 시작할 때 감도 적용
        UpdateSensitivity();
    }

    // =========================================================
    // [핵심] 감도 업데이트 함수 (외부에서 호출 가능)
    // =========================================================
    public void UpdateSensitivity()
    {
        float sens = PlayerPrefs.GetFloat("MouseSens", 5.0f);
        
        // [공식 변경] 제곱(Pow)을 사용하여 차이를 극대화함
        // 감도 1 -> 1*1 * 5 = 5 (아주 느림)
        // 감도 5 -> 5*5 * 5 = 125 (보통)
        // 감도 10 -> 10*10 * 5 = 500 (아주 빠름)
        mouseSpeed = Mathf.Pow(sens, 2) * 5.0f; 
    }

    // --- 플래시 코루틴 ---
    public IEnumerator FlashScreen()
    {
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

        if (Input.GetKeyDown(KeyCode.Space) && flashImage != null)
        {
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
                Vector3 pos = transform.position;
                pos.y = originalY * 0.5f;
                transform.position = pos;

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
                Vector3 pos = transform.position;
                pos.y = originalY;
                transform.position = pos;

                Vector3 scale = transform.localScale;
                scale.y = originalScale.y;
                transform.localScale = scale;
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 moveVec = transform.forward * v + transform.right * h;
        moveVec = moveVec.normalized * moveSpeed;
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