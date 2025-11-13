using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
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

    float currentX;
    float currentY;
    float xVelocity;
    float yVelocity;
    public float smoothTime = 0.1f; // 원하는 부드러움

    [Header("Flash Effect")]
    public Image flashImage;          // Canvas의 흰색 Image 연결
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

                // 스케일 절반으로 줄임
                Vector3 newScale = transform.localScale;
                newScale.y = originalScale.y * 0.5f;

                // Pivot이 중앙이므로 위쪽만 줄이려면
                // 줄어든 높이의 절반만큼 아래로 내려줌
                float heightBefore = originalScale.y;
                float heightAfter = newScale.y;
                float deltaHeight = heightBefore - heightAfter;

                transform.position -= new Vector3(0, deltaHeight / 2f, 0);
                transform.localScale = newScale;

                // --- 이동속도 절반으로 감소 ---
                moveSpeed *= 0.5f;
            }
        }
        else
        {
            if (isCrouched)
            {
                isCrouched = false;

                // 다시 원래 크기로 복원
                Vector3 currentScale = transform.localScale;
                float heightBefore = currentScale.y;
                float heightAfter = originalScale.y;
                float deltaHeight = heightAfter - heightBefore;

                // 복원 시 다시 위로 올려줌
                transform.position += new Vector3(0, deltaHeight / 2f, 0);
                transform.localScale = originalScale;

                // --- 이동속도 원복 ---
                moveSpeed *= 2f; // 절반으로 줄였으니 다시 2배로 복원
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