using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;         // �÷��̾� Transform
    private NavMeshAgent agent;      // ���� �̵� �����
    MonoBehaviour obj;

    public void setTarget(Transform obj)
    {
        target = obj;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position); // �÷��̾� ��ġ ����
        }
        else
        {
            Debug.LogWarning("������ NavMesh ���� ���� �ʽ��ϴ�!");
        }

        Debug.Log(Vector3.Distance(this.transform.position, this.target.transform.position));
        if (Vector3.Distance(this.transform.position, target.transform.position) < 50)
        {
            if (target.CompareTag("Lamp"))
            {
                Debug.Log("�ν�");
                LampManager.Instance.BreakLamp();
            }
        }
    }
}
