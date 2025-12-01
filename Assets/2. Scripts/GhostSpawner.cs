using System.Collections.Generic;
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

    private List<GameObject> activeGhosts = new List<GameObject>();

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

        PlayerMind playerMind = player.GetComponent<PlayerMind>();
        //정신력 체크
        if(playerMind.GetPlayerMind() > 50)
        {
            return false;
        }

        //어둠 조건 체크
        return playerMind.getIsInDarkness();
    }

    void SpawnGhost()
    {
        // 플레이어 주변 랜덤 위치 계산
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // 유령 생성
        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Ghost";

        ghost.SetActive(true);
        activeGhosts.Add(ghost);
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

    public void KillAllGhosts()
    {
        foreach (GameObject g in activeGhosts)
        {
            // 중요: 이미 게임 플레이 중에 죽어서 없을 수도 있으니 확인해야 함
            if (g != null)
            {
                Destroy(g);
            }
        }

        // 다 죽였으니 명단 초기화
        activeGhosts.Clear();
        Debug.Log("모든 유령 제거 완료");
    }
}