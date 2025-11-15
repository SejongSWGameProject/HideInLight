using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    CharacterController controller;

    [Header("Rotate")]
    public float mouseSpeed = 100f;
    float yRotation;
    float xRotation;
    Camera cam;

    [Header("Move")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    float h;
    float v;
    Vector3 velocity;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Flashlight")]
    public Light flashlight;
    public float flashlightRotateSpeed = 10f;
    Vector3 flashlightRotation;

    bool canLook = false;

    [Header("Crouch Settings")]
    public KeyCode crouchKey = KeyCode.Z;
    private float originalHeight;
    private float crouchHeight;
    private bool isCrouched = false;
    public float crouchSpeed = 5f;
    private float originalCameraY;
    private float crouchCameraY;

    float currentX;
    float currentY;
    float xVelocity;
    float yVelocity;
    public float smoothTime = 0.1f;

    [Header("Flash Effect")]
    public Image flashImage;
    public float flashDuration = 0.5f;

    public AudioClip[] footstepClips;
    public AudioClip flashClip;
    private AudioSource audioSource;
    public float stepInterval = 0.5f; // 발소리 사이의 간격 (초)

    private float stepTimer = 0f;


    void Start()
    {
        hiddenMouseCursor();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController component not found!");
            return;
        }

        cam = Camera.main;
        if (cam != null)
        {
            xRotation = cam.transform.eulerAngles.x;
            yRotation = cam.transform.eulerAngles.y;
        }

        if (flashlight != null)
            flashlightRotation = flashlight.transform.localEulerAngles;

        // CharacterController의 원래 높이 저장
        originalHeight = controller.height;
        crouchHeight = originalHeight * 0.5f;

        if (cam != null)
        {
            originalCameraY = cam.transform.localPosition.y;
            crouchCameraY = originalCameraY * 0.5f;
            
        }

        Invoke(nameof(EnableMouseLook), 0.5f);
    }

    void PlayRandomFootStep()
    {
        // 4. 소리 재생 (랜덤 클립, 랜덤 피치)
        if (footstepClips.Length == 0) return;

        // 랜덤 클립 선택
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

        // 랜덤 피치 설정
        //audioSource.pitch = Random.Range(minPitch, maxPitch);

        // 재생
        audioSource.PlayOneShot(clip);
    }
    public IEnumerator FlashScreen()
    {
        audioSource.PlayOneShot(flashClip);
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
        GetInput();
        CheckGround();
        HandleCrouch();
        HandleGravity();
        Move();

        if (!Input.GetMouseButton(1) && canLook)
            Rotate();

        if (Input.GetMouseButton(1))
        {
            // RotateFlashlight();
        }

        // 점프 입력
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        //Debug.Log(horizontalVelocity.magnitude);
        if (horizontalVelocity.magnitude > 1f)
        {
            //Debug.Log("재생" + stepTimer);
            // 3. 타이머가 다 됐는지?
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (!isCrouched)
                {
                    PlayRandomFootStep();
                    stepTimer = stepInterval; // 타이머 리셋
                }
                
            }
        }
    }

    void CheckGround()
    {
        // CharacterController의 isGrounded 사용
        isGrounded = controller.isGrounded;

        // 더 정확한 Ground Check가 필요하면 Raycast 사용
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
    }

    void HandleGravity()
    {
        // 땅에 있을 때 velocity.y를 약간의 음수로 유지
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
    }

    void HandleCrouch()
    {
        if (Input.GetKey(crouchKey))
        {
            if (!isCrouched)
            {
                isCrouched = true;
            }
        }
        else
        {
            isCrouched = false;
        }

        float targetHeight = isCrouched ? crouchHeight : originalHeight;
        float targetCameraY = isCrouched ? crouchCameraY : originalCameraY;
        float currentHeight = controller.height;

        

        // 부드럽게 높이 변경
        if (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
        {
            float newHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchSpeed);
            float heightDifference = newHeight - currentHeight;

            controller.height = newHeight;
            // Center를 조정하여 발이 땅에 붙어있도록
            controller.center = new Vector3(controller.center.x, newHeight / 2f, controller.center.z);
        }

        if (cam != null)
        {
            Vector3 camPos = cam.transform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchSpeed);
            cam.transform.localPosition = camPos;

        }
    }

    bool CheckCeiling()
    {
        // 캐릭터 위쪽에 장애물이 있는지 체크
        float checkDistance = (originalHeight - crouchHeight) + 0.2f;
        Vector3 start = transform.position + Vector3.up * controller.height;

        return Physics.Raycast(start, Vector3.up, checkDistance);
    }

    void GetInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
    }

    void Move()
    {
        // 1. 수평 이동 방향과 속도 계산
        Vector3 moveDirection = (transform.right * h + transform.forward * v).normalized;
        float currentSpeed = isCrouched ? moveSpeed * 0.5f : moveSpeed;
        Vector3 horizontalMove = moveDirection * currentSpeed;

        // 2. 수평 이동(horizontalMove)과 수직 이동(velocity.y)을 하나의 벡터로 합침
        // velocity.y는 HandleGravity()와 점프에서 이미 계산됨
        Vector3 finalMove = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);

        // 3. Move()는 프레임당 한 번만 호출!
        controller.Move(finalMove * Time.deltaTime);
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

        if (cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(currentX, currentY, 0);
        }
        transform.rotation = Quaternion.Euler(0, currentY, 0);
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
}