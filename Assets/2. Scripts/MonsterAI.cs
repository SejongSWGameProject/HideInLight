using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public Transform target;         // 플레이어 Transform
    private NavMeshAgent monster;      // 괴물 이동 제어용
    int breakDistance = 5;

    public const int NORMAL = 1;
    public const int CHASE = 2;
    public const int STUN = 3;
    int monsterState = NORMAL;           //1:평상시(전등깨고다니는중)   2:플레이어쫓는중   3:스턴맞음

    public Transform player;         // 플레이어 Transform
    public float viewRadius = 80f; // 시야 반경 (거리)
    [Range(0, 360)]
    public float viewAngle = 300f;  // 시야각
    public LayerMask obstacleMask; // 장애물 레이어 마스크

    [SerializeField] LampManager lampManager;

    private bool isPaused = false;

    // "눈"의 위치 (옵션, 정확도를 높임)
    // 비워두면 이 스크립트가 붙은 오브젝트의 transform.position을 사용합니다.
    public Transform eyePosition;

    // 플레이어를 봤는지 여부
    public bool canSeePlayer { get; private set; }

    public void setTarget(Transform obj)
    {
        target = obj;
    }

    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {

        Vector3 targetPosWithoutY = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 monsterPosWithoutY = new Vector3(this.transform.position.x, 0f, this.transform.position.z);
        if (target != null && monster.isOnNavMesh)
        {
            monster.SetDestination(targetPosWithoutY);
        }
        else
        {
            Debug.LogWarning("괴물이 NavMesh 위에 있지 않습니다!");
        }
        if (monsterState == NORMAL)
        {
            monster.speed = 15;
            //CheckSight();
            //Debug.Log(Vector3.Distance(targetPosWithoutY, monsterPosWithoutY));
            if (Vector3.Distance(targetPosWithoutY, monsterPosWithoutY) < breakDistance)
            {
                if (target.CompareTag("Lamp"))
                {
                    LampManager.Instance.BreakLamp();
                }
            }
        }
        else if(monsterState == CHASE)
        {
            monster.speed = 30;
            target = player;

            if (Input.GetKeyDown(KeyCode.P) && !isPaused)
            {
                StartCoroutine(PauseMonster(3.0f));
            }
        }
        
        //Debug.Log(Vector3.Distance(this.transform.position, this.target.transform.position));
    }

    public void setMonsterState(int state)
    {
        monsterState = state;
    }

    // 4. 몬스터를 일정 시간 멈추게 하는 코루틴
    public IEnumerator PauseMonster(float duration)
    {
        // 5. 멈춤 상태로 변경
        isPaused = true;

        // 6. NavMeshAgent의 이동을 중지
        //    (agent.enabled = false; 보다 이 방법이 더 안전합니다)
        if (monster.isOnNavMesh) // NavMesh 위에 있을 때만
        {
            monster.isStopped = true;
        }

        // --- (선택) 애니메이션도 멈추기 ---
        // Animator animator = GetComponent<Animator>();
        // if (animator != null)
        // {
        //     animator.speed = 0; // 애니메이션 속도를 0으로 만들어 멈춤
        // }
        // ---------------------------------

        Debug.Log("몬스터 멈춤!");

        // 7. 지정된 시간(duration)만큼 대기
        yield return new WaitForSeconds(duration);

        // 8. 3초가 지난 후, NavMeshAgent의 이동을 다시 시작
        if (monster.isOnNavMesh)
        {
            monster.isStopped = false;
        }

        // --- (선택) 애니메이션 다시 재생 ---
        // if (animator != null)
        // {
        //     animator.speed = 1; // 애니메이션 속도를 다시 1로
        // }
        // ---------------------------------

        Debug.Log("몬스터 다시 움직임!");

        // 9. 멈춤 상태 해제
        isPaused = false;

        if (CheckSight() == false)
        {
            Debug.Log("스턴 후 안보임");
            monsterState = NORMAL;
            lampManager.SetMonsterTargetToRandomLamp();
        }
    }

    bool CheckSight()
    {
        // "눈" 위치가 설정되지 않았으면 기본 transform.position 사용
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : this.transform.position;

        // 1. 거리 체크
        float distanceToPlayer = Vector3.Distance(eyePos, player.position);
        if (distanceToPlayer > viewRadius)
        {
            canSeePlayer = false;
            return false;
        }

        // 2. 시야각 체크
        Vector3 directionToPlayer = (player.position - eyePos).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > viewAngle / 2)
        {
            canSeePlayer = false;
            return false;
        }


        // 3. 장애물(시선) 체크
        // Physics.Raycast를 사용해 "눈" 위치에서 플레이어 방향으로 광선을 쏩니다.
        // 이 광선이 플레이어에게 도달하기 전에 'obstacleMask'에 해당하는 장애물과 부딪히면,
        // 플레이어를 볼 수 없는 것입니다.
        if (Physics.Raycast(eyePos, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            // 장애물에 가려짐
            Debug.Log("가려짐");

            canSeePlayer = false;
            return false;
        }
        else
        {
            // 장애물 없이 플레이어를 봄!
            canSeePlayer = true;
            Debug.Log("발견!");
            setMonsterState(CHASE);
            return true;
            // 여기에 플레이어를 발견했을 때의 로직을 추가 (예: 추격 시작)
        }
    }

    // (팁) 기즈모를 사용하면 씬(Scene) 뷰에서 시야각을 시각적으로 확인할 수 있습니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : transform.position;
        Gizmos.DrawWireSphere(eyePos, viewRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward * viewRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, transform.up) * transform.forward * viewRadius;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(eyePos, fovLine1);
        Gizmos.DrawRay(eyePos, fovLine2);

        if (canSeePlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, player.position);
        }
    }
}
