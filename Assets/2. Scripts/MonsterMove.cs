using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;         // �÷��̾� Transform
    private NavMeshAgent agent;      // ���� �̵� �����
    MonoBehaviour obj;
    int breakDistance = 5;

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

        //Debug.Log(Vector3.Distance(this.transform.position, this.target.transform.position));
        Vector3 targetPosWithoutY = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 monsterPosWithoutY = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        if (Vector3.Distance(targetPosWithoutY, monsterPosWithoutY) < breakDistance)
        {
            if (target.CompareTag("Lamp"))
            {
                Debug.Log("�ν�");
                LampManager.Instance.BreakLamp();
            }
        }
    }
}
