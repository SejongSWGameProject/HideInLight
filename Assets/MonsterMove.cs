using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform player;         // 플레이어 Transform
    private NavMeshAgent agent;      // 괴물 이동 제어용

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position); // 플레이어 위치 따라감
        }
    }
}
