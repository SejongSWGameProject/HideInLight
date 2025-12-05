using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class GhostAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float minHeight = 4f; // 월드 Y축 최소 높이

    [Header("Target")]
    private Transform player;
    private Transform mainCamera;
    private PlayerMove playerMove;
    private PlayerLight playerLight;
    private PlayerMind playerMind;

    [Header("Attack")]
    public float attackRange = 7f;

    [Header("Visual")]
    // [주의] 애니메이션으로 둥둥 뜨는 모션을 만들었다면 이 체크를 해제하세요! (겹침 방지)
    public bool floatEffect = false;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    private float floatTimer;

    [Header("Death Settings")]
    public float fadeOutDuration = 1f;
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;
    public AudioClip attackSound;

    private bool isDying = false;
    private Renderer ghostRenderer;
    private AudioSource audioSource;
    private Animator animator; // 애니메이터

    void Start()
    {
        // 안전하게 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMove = player.GetComponent<PlayerMove>();
            playerLight = player.GetComponentInChildren<PlayerLight>();
            playerMind = player.GetComponentInChildren<PlayerMind>();
        }

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

        animator = GetComponentInChildren<Animator>();
        ghostRenderer = GetComponentInChildren<Renderer>();

        audioSource = GetComponent<AudioSource>(); // 오디오 소스

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (player == null || isDying) return; // 죽는 중이면 아무것도 안 함

        MoveTowardsPlayer();
        RotateTowardsPlayer();

        // 애니메이션이 없을 때만 코드로 움직이게 설정
        if (floatEffect)
        {
            ApplyFloatEffect();
        }

        if (playerLight != null)
        {
            if (playerLight.IsObjectIlluminated(this.gameObject) && playerLight.colorIndex == 2)
            {
                Debug.Log("빛받음");
                Die(true);
            }
        }
        CheckAttack();
    }

    void MoveTowardsPlayer()
    {
        // 플레이어를 향한 3D 방향 (Y축 포함)
        Vector3 direction = mainCamera.position - transform.position;
        direction.Normalize();

        // 3D 공간에서 자유롭게 이동
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        // 최소 높이 체크
        if (newPosition.y < minHeight)
        {
            newPosition.y = minHeight;
        }

        transform.position = newPosition;
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = mainCamera.position - transform.position;
        direction.y = 0; // 회전은 Y축 기준으로만

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void ApplyFloatEffect()
    {
        floatTimer += Time.deltaTime * floatFrequency;
        float offsetY = Mathf.Sin(floatTimer) * floatAmplitude;

        // 현재 위치에서 Y축만 둥둥 효과 추가
        Vector3 pos = transform.position;
        pos.y += offsetY * Time.deltaTime * 2f; // 부드러운 떠다니기

        // 최소 높이 체크
        if (pos.y < minHeight)
        {
            pos.y = minHeight;
        }

        transform.position = pos;
    }

    public void Die(bool playSound)
    {
        if (isDying) return;
        isDying = true;

        // 2. 더 이상 추적하지 못하게 스크립트 기능 끄기 (하지만 코루틴은 돌아감)
        // 이 스크립트(GhostAI)가 꺼져도 이미 시작된 코루틴은 끝까지 돕니다.
        // 다만 Update()는 멈춥니다.
        // this.enabled = false; // 필요하다면 주석 해제

        // 3. 충돌체 끄기 (시체가 길을 막지 않게)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. 소리 및 이펙트
        if (audioSource != null && deathSound != null && playSound)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 5. 서서히 사라지기 시작
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        if (ghostRenderer == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Material mat = ghostRenderer.material;

        // HDRP/Standard 쉐이더 호환성 체크
        bool isHDRP = mat.HasProperty("_BaseColor");
        Color startColor = isHDRP ? mat.GetColor("_BaseColor") : mat.color;

        float startAlpha = startColor.a;
        float timer = 0f;
        Vector3 originalPosition = transform.position;

        // 떨림 설정
        float shakeIntensity = 0.2f;
        float shakeSpeed = 30f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeOutDuration;

            // 알파값 감소 (페이드 아웃)
            float newAlpha = Mathf.Lerp(startAlpha, 0f, progress);
            Color newColor = startColor;
            newColor.a = newAlpha;

            if (isHDRP) mat.SetColor("_BaseColor", newColor);
            else mat.color = newColor;

            // 양 옆으로 부르르 떨림 (처음 강했다가 점점 약해짐)
            float shake = Mathf.Sin(timer * shakeSpeed) * shakeIntensity * (1f - progress);
            Vector3 shakeOffset = transform.right * shake;
            transform.position = originalPosition + shakeOffset;

            yield return null;
        }

        Destroy(gameObject);
    }

    private bool isAttacking = false;

    void CheckAttack()
    {
        if (isAttacking) return; // 이미 공격 중이면 무시

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            isAttacking = true; // 공격 시작 플래그
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        // 1. 플레이어 방향으로 회전 (LookRotation)
        Vector3 direction = mainCamera.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            // [중요] 만약 모델이 뒤집혀 있다면 아래 줄 주석을 해제해서 180도 돌리세요
            // targetRot *= Quaternion.Euler(0, 180, 0); 
            transform.rotation = targetRot;
        }

        gameObject.layer = LayerMask.NameToLayer("Default");
        transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");

        // 2. 공격 애니메이션 실행
        //if (animator != null) animator.SetTrigger("doAttack");

        // 3. 데미지 처리
        //Debug.Log("공격 및 돌진!");
        if (playerMind != null) playerMind.IncreasePlayerMind(-10);

        audioSource.PlayOneShot(attackSound);

        // 4. [핵심] 앞으로 돌진 (Lunge) 구현
        // 0.5초 동안 유령을 앞으로 강제 이동시킵니다.
        float lungeDuration = 0.25f; // 돌진하는 시간
        float lungeSpeed = 30f;     // 돌진 속도 (빠르게)
        float timer = 0;

        while (timer < lungeDuration)
        {
            timer += Time.deltaTime;
            // 유령이 바라보는 방향(Forward)으로 이동
            transform.Translate(Vector3.forward * lungeSpeed * Time.deltaTime);
            yield return null;
        }

        // 5. 돌진이 끝나면 사망 처리
        Die(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 최소 높이 시각화
        Gizmos.color = Color.yellow;
        Vector3 minHeightPos = transform.position;
        minHeightPos.y = minHeight;
        Gizmos.DrawWireSphere(minHeightPos, 0.2f);
    }
}