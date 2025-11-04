using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFollow : MonoBehaviour
{
    public Transform target;         // 플레이어 Transform
    private NavMeshAgent agent;      // 괴물 이동 제어용
    MonoBehaviour obj;
    int breakDistance = 5;

    int monsterState = 1;           //1:평상시(전등깨고다니는중)   2:플레이어쫓는중   3:스턴맞음

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
        //Debug.Log(Vector3.Distance(this.transform.position, this.target.transform.position));
        Vector3 targetPosWithoutY = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 monsterPosWithoutY = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(targetPosWithoutY); // 플레이어 위치 따라감
        }
        else
        {
            Debug.LogWarning("괴물이 NavMesh 위에 있지 않습니다!");
        }

        
        if (Vector3.Distance(targetPosWithoutY, monsterPosWithoutY) < breakDistance)
        {
            if (target.CompareTag("Lamp"))
            {
                Debug.Log("부심");
                LampManager.Instance.BreakLamp();
            }
        }
    }
}
