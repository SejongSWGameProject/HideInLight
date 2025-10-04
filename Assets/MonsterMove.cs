using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform player;         // �÷��̾� Transform
    private NavMeshAgent agent;      // ���� �̵� �����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position); // �÷��̾� ��ġ ����
        }
    }
}
