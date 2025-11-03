using UnityEngine;
using System.Collections;

public class MonsterSight : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float viewRadius = 10f; // 시야 반경 (거리)
    [Range(0, 360)]
    public float viewAngle = 90f;  // 시야각

    public LayerMask obstacleMask; // 장애물 레이어 마스크

    // "눈"의 위치 (옵션, 정확도를 높임)
    // 비워두면 이 스크립트가 붙은 오브젝트의 transform.position을 사용합니다.
    public Transform eyePosition;

    // 플레이어를 봤는지 여부
    public bool canSeePlayer { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        CheckSight();
    }

    void CheckSight()
    {
        // "눈" 위치가 설정되지 않았으면 기본 transform.position 사용
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : this.transform.position;

        // 1. 거리 체크
        float distanceToPlayer = Vector3.Distance(eyePos, player.position);
        if (distanceToPlayer > viewRadius)
        {
            canSeePlayer = false;
            return;
        }

        // 2. 시야각 체크
        Vector3 directionToPlayer = (player.position - eyePos).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > viewAngle / 2)
        {
            canSeePlayer = false;
            return;
        }


        // 3. 장애물(시선) 체크
        // Physics.Raycast를 사용해 "눈" 위치에서 플레이어 방향으로 광선을 쏩니다.
        // 이 광선이 플레이어에게 도달하기 전에 'obstacleMask'에 해당하는 장애물과 부딪히면,
        // 플레이어를 볼 수 없는 것입니다.
        if (Physics.Raycast(eyePos, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            // 장애물에 가려짐
            canSeePlayer = false;
        }
        else
        {
            // 장애물 없이 플레이어를 봄!
            canSeePlayer = true;
            Debug.Log("플레이어 발견!");
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