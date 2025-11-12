using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public Transform target;         // �÷��̾� Transform
    private NavMeshAgent monster;      // ���� �̵� �����
    private Animator animator;

    public const int NORMAL = 1;
    public const int CHASE = 2;
    public const int STUN = 3;
    int monsterState = NORMAL;           //1:����(��������ٴϴ���)   2:�÷��̾��Ѵ���   3:���ϸ���

    public Transform player;         // �÷��̾� Transform
    public float viewRadius = 80f; // �þ� �ݰ� (�Ÿ�)
    [Range(0, 360)]
    public float viewAngle = 300f;  // �þ߰�
    public LayerMask obstacleMask; // ��ֹ� ���̾� ����ũ

    [SerializeField] LampManager lampManager;

    private bool isPaused = false;
    private float deltatDistance = 10f;
    private Vector3 prevPosition;

    // "��"�� ��ġ (�ɼ�, ��Ȯ���� ����)
    // ����θ� �� ��ũ��Ʈ�� ���� ������Ʈ�� transform.position�� ����մϴ�.
    public Transform eyePosition;

    private Coroutine currentPauseCoroutine;

    // �÷��̾ �ô��� ����
    public bool canSeePlayer { get; private set; }

    public void setTarget(Transform obj)
    {
        target = obj;
    }

    void Start()
    {
        monster = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(CalculateDeltaDistance(0.5f));
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
            Debug.LogWarning("������ NavMesh ���� ���� �ʽ��ϴ�!");
        }
        if (monster.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (monsterState == NORMAL)
        {
            monster.speed = 15;
            animator.SetFloat("animSpeed", 1.0f);
            CheckSight();
                
            if (deltatDistance < 0.1f && Vector3.Distance(this.transform.position, this.target.transform.position)<20)
            {
                animator.SetBool("isWalking", false);

                if (target.CompareTag("Lamp"))
                {
                    //Debug.Log("�μ�");
                    LampManager.Instance.BreakLamp();
                }
            }
        }
        else if(monsterState == CHASE)
        {
            animator.SetFloat("animSpeed", 3.0f);
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

    // 4. ���͸� ���� �ð� ���߰� �ϴ� �ڷ�ƾ
    private IEnumerator PauseMonster(float duration)
    {
        // 5. ���� ���·� ����
        isPaused = true;

        // 6. NavMeshAgent�� �̵��� ����
        //    (agent.enabled = false; ���� �� ����� �� �����մϴ�)
        if (monster.isOnNavMesh) // NavMesh ���� ���� ����
        {
            monster.isStopped = true;
        }

        // --- (����) �ִϸ��̼ǵ� ���߱� ---
        // Animator animator = GetComponent<Animator>();
        // if (animator != null)
        // {
        //     animator.speed = 0; // �ִϸ��̼� �ӵ��� 0���� ����� ����
        // }
        // ---------------------------------

        Debug.Log("���� ����!");

        // 7. ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration);

        // 8. 3�ʰ� ���� ��, NavMeshAgent�� �̵��� �ٽ� ����
        if (monster.isOnNavMesh)
        {
            monster.isStopped = false;
        }

        // --- (����) �ִϸ��̼� �ٽ� ��� ---
        // if (animator != null)
        // {
        //     animator.speed = 1; // �ִϸ��̼� �ӵ��� �ٽ� 1��
        // }
        // ---------------------------------

        Debug.Log("���� �ٽ� ������!");

        // 9. ���� ���� ����
        isPaused = false;

        if (CheckSight() == false)
        {
            Debug.Log("���� �� �Ⱥ���");
            monsterState = NORMAL;
            deltatDistance = 10.0f;
            lampManager.SetMonsterTargetToRandomLamp();
        }

        currentPauseCoroutine = null;
    }

    bool CheckSight()
    {
        // "��" ��ġ�� �������� �ʾ����� �⺻ transform.position ���
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : this.transform.position;

        // 1. �Ÿ� üũ
        float distanceToPlayer = Vector3.Distance(eyePos, player.position);
        if (distanceToPlayer > viewRadius)
        {
            canSeePlayer = false;
            return false;
        }

        // 2. �þ߰� üũ
        Vector3 directionToPlayer = (player.position - eyePos).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > viewAngle / 2)
        {
            canSeePlayer = false;
            return false;
        }


        // 3. ��ֹ�(�ü�) üũ
        // Physics.Raycast�� ����� "��" ��ġ���� �÷��̾� �������� ������ ���ϴ�.
        // �� ������ �÷��̾�� �����ϱ� ���� 'obstacleMask'�� �ش��ϴ� ��ֹ��� �ε�����,
        // �÷��̾ �� �� ���� ���Դϴ�.
        if (Physics.Raycast(eyePos, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            // ��ֹ��� ������
            canSeePlayer = false;
            return false;
        }
        else
        {
            // ��ֹ� ���� �÷��̾ ��!
            canSeePlayer = true;
            Debug.Log("�߰�!");
            setMonsterState(CHASE);
            return true;
            // ���⿡ �÷��̾ �߰����� ���� ������ �߰� (��: �߰� ����)
        }
    }

    // (��) ����� ����ϸ� ��(Scene) �信�� �þ߰��� �ð������� Ȯ���� �� �ֽ��ϴ�.
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

    public IEnumerator CalculateDeltaDistance(float delta)
    {
        while (true)
        {
            deltatDistance = Vector3.Distance(transform.position, prevPosition);
            // 4. ���� ��ġ(transform.position)�� ������ �����մϴ�.
            prevPosition = transform.position;

            // (���� ����) �ֿܼ� �α׸� ��� Ȯ���մϴ�.
            //Debug.Log(deltatDistance);

            // 5. �ڷ�ƾ�� 0.5��(���� �ð� ����) ���� '�Ͻ� ����' ��ŵ�ϴ�.
            // 0.5�ʰ� ������ while ������ ó������ ���ư� 4������ �ٽ� �����մϴ�.
            yield return new WaitForSeconds(delta);
        }
    }

    public void RequestPause(float duration)
    {
        if (currentPauseCoroutine != null)
        {
            StopCoroutine(currentPauseCoroutine);
        }
        currentPauseCoroutine = StartCoroutine(PauseMonster(duration));
    }
}
