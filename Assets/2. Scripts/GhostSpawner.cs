using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject ghostPrefab;
    public float spawnRadius = 40f;
    public float minSpawnRadius = 20f;
    public float spawnInterval = 5f;

    [Header("Spawn Conditions")]
    public int maxGhosts = 5;
    public bool spawnInDarkness = true;
    public float darknessThreshold = 0.3f;

    private Transform player;

    private float nextSpawnTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            if (CanSpawnGhost())
            {
                SpawnGhost();
            }
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    bool CanSpawnGhost()
    {
        // 최대 유령 수 체크
        if (GameObject.FindGameObjectsWithTag("Ghost").Length >= maxGhosts)
            return false;

        //어둠 조건 체크
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        return playerMove.getIsInDarkness();
    }

    void SpawnGhost()
    {
        // 플레이어 주변 랜덤 위치 계산
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // 유령 생성
        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Ghost";

        ghost.SetActive(true);

        Debug.Log($"Ghost spawned at {spawnPosition}");
    }

    Vector3 GetRandomSpawnPosition()
    {
        // 플레이어 주변 원형 범위에서 랜덤 위치
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnRadius, spawnRadius);

        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y) * distance;
        Vector3 spawnPos = player.position + offset;

        // 바닥 높이에 맞추기 (옵션)
        spawnPos.y = player.position.y;

        return spawnPos;
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
}