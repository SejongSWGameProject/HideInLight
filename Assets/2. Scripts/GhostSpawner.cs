using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ghostPrefab;
    public float spawnRadius = 40f;
    public float minSpawnRadius = 20f;

    [Header("Spawn Interval (based on Mind)")]
    [Tooltip("정신력이 높을 때 스폰 간격 (초)")]
    public float maxSpawnInterval = 10f;
    [Tooltip("정신력이 낮을 때 스폰 간격 (초)")]
    public float minSpawnInterval = 2f;

    [Header("Spawn Conditions")]
    public int maxGhosts = 5;
    [Tooltip("정신력이 이 값 이하일 때만 스폰")]
    public float spawnMindThreshold = 50f;
    [Tooltip("스폰 위치의 밝기가 이 값 이하일 때만 스폮 (어둠 판정)")]
    public float spawnDarknessThreshold = 1.0f;
    [Tooltip("벽 체크용 레이어")]
    public LayerMask obstacleLayer;

    private Transform player;
    private PlayerMind playerMind;
    private float nextSpawnTime;
    private List<GameObject> activeGhosts = new List<GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMind = player.GetComponent<PlayerMind>();
        nextSpawnTime = Time.time + maxSpawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            if (CanSpawnGhost())
            {
                SpawnGhost();
            }

            // 다음 스폰 시간을 정신력에 따라 동적으로 설정
            float currentInterval = CalculateSpawnInterval();
            nextSpawnTime = Time.time + currentInterval;
        }
    }

    bool CanSpawnGhost()
    {
        // 최대 유령 수 체크
        if (GameObject.FindGameObjectsWithTag("Ghost").Length >= maxGhosts)
            return false;

        // 정신력 체크 (임계값 이하일 때만 스폰)
        if (playerMind.GetPlayerMind() > spawnMindThreshold)
            return false;

        // 어둠 조건 체크
        if (!playerMind.getIsInDarkness())
            return false;

        return true;
    }

    /// <summary>
    /// 정신력에 따라 스폰 간격을 계산
    /// 정신력이 낮을수록 짧은 간격으로 스폰
    /// </summary>
    float CalculateSpawnInterval()
    {
        float currentMind = playerMind.GetPlayerMind();

        // 정신력을 0~spawnMindThreshold 범위로 정규화 (0~1)
        float normalizedMind = Mathf.Clamp01(currentMind / spawnMindThreshold);

        // 정신력이 높을수록 긴 간격, 낮을수록 짧은 간격
        float interval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, normalizedMind);

        return interval;
    }

    void SpawnGhost()
    {
        // 어두운 위치를 찾을 때까지 시도 (최대 10번)
        Vector3 spawnPosition = Vector3.zero;
        bool foundDarkSpot = false;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 candidatePos = GetRandomSpawnPosition();

            if (IsPositionInDarkness(candidatePos))
            {
                spawnPosition = candidatePos;
                foundDarkSpot = true;
                break;
            }
        }

        // 어두운 위치를 찾지 못하면 스폰하지 않음
        if (!foundDarkSpot)
        {
            Debug.Log("어두운 스폰 위치를 찾지 못했습니다.");
            return;
        }

        // 유령 생성
        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Ghost";
        ghost.SetActive(true);
        activeGhosts.Add(ghost);

        Debug.Log($"Ghost spawned at {spawnPosition} (Mind: {playerMind.GetPlayerMind():F1}, Interval: {CalculateSpawnInterval():F1}s)");
    }

    Vector3 GetRandomSpawnPosition()
    {
        // 플레이어 주변 원형 범위에서 랜덤 위치
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnRadius, spawnRadius);
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * distance;
        Vector3 spawnPos = player.position + offset;

        // 바닥 높이에 맞추기
        spawnPos.y = player.position.y;

        return spawnPos;
    }

    /// <summary>
    /// 특정 위치가 어두운지 체크 (PlayerMind의 CheckDarkness 로직 사용)
    /// </summary>
    bool IsPositionInDarkness(Vector3 position)
    {
        // 1. 반경 40m 내의 전등 레이어만 감지
        Collider[] hitColliders = Physics.OverlapSphere(position, 40f, LayerMask.GetMask("Lamp"));
        float distSum = 0.0f;

        // 2. 배열을 한 번만 돌면서 검사와 계산을 동시에 수행
        foreach (Collider c in hitColliders)
        {
            // 전등 컴포넌트 가져오기
            LampController l = c.GetComponent<LampController>();

            // 예외 처리: 컴포넌트가 없거나, 전등이 꺼져있으면 패스
            if (l == null || !l.lamp.enabled) continue;

            Vector3 direction = c.transform.position - position;
            float distance = direction.magnitude; // 레이캐스트용 실제 거리

            // 3. 벽 체크 (Raycast)
            // 장애물에 막히지 않았을 때만 계산
            if (!Physics.Raycast(position, direction.normalized, distance, obstacleLayer))
            {
                // 벽이 없다면 빛 계산
                float distSquare = direction.sqrMagnitude; // 거리 제곱 (최적화)

                // 0으로 나누기 방지
                if (distSquare > 0.001f)
                {
                    distSum += (1000.0f / distSquare);
                }
            }
        }

        // 4. 최종 판정 - distSum이 임계값보다 작으면 어둠
        return distSum < spawnDarknessThreshold;
    }

    // 디버그 시각화
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }

    public void KillAllGhosts()
    {
        foreach (GameObject g in activeGhosts)
        {
            if (g != null)
            {
                Destroy(g);
            }
        }
        activeGhosts.Clear();
    }
}