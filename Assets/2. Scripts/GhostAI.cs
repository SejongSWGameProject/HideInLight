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
        ghostRenderer = GetComponent<Renderer>();
        if (ghostRenderer == null) ghostRenderer = GetComponentInChildren<Renderer>(); // 자식에 있을 경우 대비

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
                Die();
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

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        // 1. 애니메이션 먼저 실행
        if (animator != null)
        {
            animator.SetTrigger("doDie");
        }

        // 2. 더 이상 추적하지 못하게 스크립트 기능 끄기 (하지만 코루틴은 돌아감)
        // 이 스크립트(GhostAI)가 꺼져도 이미 시작된 코루틴은 끝까지 돕니다.
        // 다만 Update()는 멈춥니다.
        // this.enabled = false; // 필요하다면 주석 해제

        // 3. 충돌체 끄기 (시체가 길을 막지 않게)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. 소리 및 이펙트
        if (audioSource != null && deathSound != null)
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

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, timer / fadeOutDuration);

            Color newColor = startColor;
            newColor.a = newAlpha;

            if (isHDRP) mat.SetColor("_BaseColor", newColor);
            else mat.color = newColor;

            yield return null;
        }

        Destroy(gameObject);
    }

    void CheckAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, mainCamera.position);

        if (distanceToPlayer <= attackRange)
        {
            // 공격 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("doAttack");
            }

            Debug.Log("공격!");
            if (playerMind != null) playerMind.IncreasePlayerMind(-10);

            // [확인] 공격 후 바로 죽는 것이 의도된 것인가요? (자폭 유령)
            // 만약 계속 살아서 공격해야 한다면 Die()를 지우고 쿨타임(Cooldown)을 넣어야 합니다.
            Die();
        }
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