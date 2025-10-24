using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;         // 플레이어 Transform
    private NavMeshAgent agent;      // 괴물 이동 제어용

    public void setTarget(Transform obj)
    {
        target = obj;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.Warp(new Vector3(0, 0, 0));
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position); // 플레이어 위치 따라감
        }
        if (agent.isOnNavMesh) // NavMesh 위에 있을 때만 실행
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning("괴물이 NavMesh 위에 있지 않습니다!");
        }
    }
}
