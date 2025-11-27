using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class GhostAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Target")]
    private Transform player;
    private PlayerMove playerMove;
    private PlayerLight playerLight;

    [Header("Attack")]
    public float attackRange = 2f;

    [Header("Visual")]
    public bool floatEffect = true;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    private float startY;
    private float floatTimer;

    [Header("Death Settings")]
    public float fadeOutDuration = 1f;
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    private bool isDying = false;
    private Renderer ghostRenderer;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMove = player.GetComponent<PlayerMove>();
        playerLight = player.GetComponentInChildren<PlayerLight>();
        startY = transform.position.y;

        // Rigidbody가 있다면 kinematic으로 설정
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Collider를 trigger로 설정 (벽 통과용)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();
        RotateTowardsPlayer();

        if (floatEffect)
        {
            ApplyFloatEffect();
        }

        if (playerLight.IsObjectIlluminated(this.gameObject))
        {
            Debug.Log("빛받음");
            Die();
        }
    }

    void MoveTowardsPlayer()
    {
        // 플레이어 방향 계산 (Y축 무시)
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        // 벽을 뚫고 이동 (Transform 직접 이동)
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void RotateTowardsPlayer()
    {
        // 플레이어를 바라보도록 회전
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

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
        // 부드러운 상하 움직임
        floatTimer += Time.deltaTime * floatFrequency;
        float newY = startY + Mathf.Sin(floatTimer) * floatAmplitude;

        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        GhostAI ai = GetComponent<GhostAI>();
        if(ai != null)
        {
            ai.enabled = false;
        }
        // 사망 효과음
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // 파티클 효과
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 페이드아웃 시작
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        // [중요 수정 2] 렌더러가 없으면 에러가 나므로, 바로 삭제하고 종료
        if (ghostRenderer == null)
        {
            Destroy(gameObject);
            yield break; // 코루틴 즉시 종료
        }

        Material mat = ghostRenderer.material;
        Color startColor = mat.color; // HDRP에서는 쉐이더에 따라 _BaseColor일 수 있음
        float startAlpha = startColor.a;
        float duration = 1.0f; // 1초 동안 사라짐
        float timer = 0f;

        // HDRP/URP 투명 모드 설정이 되어 있어야 투명해짐
        // (보통 Surface Type이 Transparent여야 함)

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, timer / duration);

            // 색상 업데이트
            Color newColor = startColor;
            newColor.a = newAlpha;
            mat.color = newColor; // HDRP 표준 쉐이더는 mat.SetColor("_BaseColor", newColor)가 필요할 수 있음

            yield return null;
        }

        Destroy(gameObject);
    }

    void CheckAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            //Attack()
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}


