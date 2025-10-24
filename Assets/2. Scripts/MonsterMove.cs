using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;         // �÷��̾� Transform
    private NavMeshAgent agent;      // ���� �̵� �����

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
            agent.SetDestination(target.position); // �÷��̾� ��ġ ����
        }
        if (agent.isOnNavMesh) // NavMesh ���� ���� ���� ����
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning("������ NavMesh ���� ���� �ʽ��ϴ�!");
        }
    }
}
