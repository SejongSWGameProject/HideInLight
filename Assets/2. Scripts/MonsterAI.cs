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
    public const int BREAK = 4;
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
    public Transform playerCamera;
    public Transform jumpScareCameraPos;

    public float killDistance = 1.2f; // 점프 스케어 시 몬스터와 카메라 사이의 거리
    public AudioClip jumpscareSound;
    private AudioSource audioSource;
    public Light jumpscareLight;

    private bool isJumpscaring = false;

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
        audioSource = GetComponent<AudioSource>();
        //this.gameObject.SetActive(false);

        if (monster == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트를 찾을 수 없습니다!");
            return;
        }

    }

    void Update()
    {
        if (isJumpscaring)
        {
            return;
        }

        // 1. 걷기/뛰기 제어
        // 몬스터의 현재 속도(currentMonsterSpeed)를 
        // Animator의 "Speed" 파라미터에 계속 전달합니다.
        // 이 값에 따라 Animator가 알아서 걷기/뛰기/대기 상태를 전환합니다.
        animator.SetFloat("Speed", monster.speed);

        if (!isPaused)
        {
            // (기존 로직)
            // 몬스터의 현재 속도를 Speed 파라미터에 전달

            animator.SetFloat("Speed", monster.speed);
        }


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
            monster.speed = 10;
            animator.SetFloat("animSpeed", 1.0f);
            CheckSight();
            //Debug.Log(monster.velocity.magnitude);
            if(monster.velocity.magnitude < 1f && Vector3.Distance(this.transform.position, this.target.transform.position) < 20)
            {
                StartCoroutine(BreakLamp());
            }
        }
        else if (monsterState == CHASE)
        {
            monster.speed = 30;
            animator.SetFloat("animSpeed", 3.0f);
            target = player;

            if (Input.GetKeyDown(KeyCode.P) && !isPaused)
            {
                StartCoroutine(PauseMonster(3.0f));
            }
        }
        //Debug.Log(Vector3.Distance(this.transform.position, this.target.transform.position));

        // 2. 죽이기 공격 제어 (예시: 특정 조건이 만족되면)
        if (CanKillPlayer()) // "플레이어를 죽일 수 있는 조건인가?"를 체크하는 함수(직접 구현 필요)
        {
            
            StartJumpScare();
        }
    }

    public void StartMove()
    {
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isJumpscaring || !other.CompareTag("Player"))
        {
            return;
        }

        if (other.CompareTag("Player") && monsterState!=STUN)
        {
            StartJumpScare();
        }
    }

    bool CanKillPlayer()
    {
        // 예: 플레이어와의 거리가 1미터 미만이고 공격 쿨타임이 지났다면
        if (Input.GetKeyDown(KeyCode.K))
        {
            return true;
        }
        return false;
    }

    void StartJumpScare()
    {
        isJumpscaring = true;
        // 1. 몬스터 AI 정지
        if (monster != null)
        {
            monster.isStopped = true;
            monster.enabled = false;
            monster.velocity = Vector3.zero;
        }

   

        animator.SetFloat("Speed", 0f); // 혹시 모르니 애니메이터 속도도 0

        // 2. 플레이어 컨트롤 정지
        // (플레이어 이동 스크립트 이름이 'PlayerMovement'라고 가정)
        PlayerMove playerMovement = player.GetComponent<PlayerMove>();
        SlopeStabilizer playerMovement2 = player.GetComponent<SlopeStabilizer>();
        PlayerLight playerLight = player.GetComponentInChildren<PlayerLight>();
        if (playerMovement != null)
        {
            playerMovement.moveSpeed = 0f;
            playerMovement.enabled = false;
        }
        if (playerMovement2 != null)
        {
            playerMovement2.enabled = false;
        }
        if (playerLight != null)
        {
            playerLight.flashlight.enabled = false;
            playerLight.enabled = false;
        }

        // (B) 플레이어 'Rigidbody'의 속도를 강제로 0으로 만듦
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 모든 물리적 움직임(속도)과 회전 속도를 즉시 0으로 만듭니다.
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // (선택 사항이지만 권장)
            // 씬 연출 중에는 Rigidbody가 다른 물체에 밀리거나 중력에 
            // 반응하지 않도록 'Kinematic'으로 만드는 것이 가장 안전합니다.
            rb.isKinematic = true;
        }


        // 마우스 커서 잠금 해제 (필요시)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (jumpscareLight != null)
        {
            jumpscareLight.enabled = true; // <--- 조명 활성화
        }

        playerCamera.position = jumpScareCameraPos.position;
        Vector3 scarePos = transform.position;
        //scarePos.y -= 2;
        monster.transform.position = scarePos;

        playerCamera.LookAt(transform);

        // 플레이어 카메라도 몬스터의 '눈높이 타겟'을 정면으로 바라보게 함
        if (eyePosition != null)
        {
            playerCamera.LookAt(eyePosition);
        }
        else
        {
            // monsterLookTarget이 설정 안 된 경우를 대비한 예외 처리
            playerCamera.LookAt(transform.position);
        }

       

        // 4. 애니메이션 및 사운드 재생
        // (이전에 설정한 'KillPlayer' 트리거 사용)
        animator.SetTrigger("KillPlayer");

        if (jumpscareSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }

        // 5. 게임 오버 처리 (아래 3단계 참고)
        // (이벤트 또는 Invoke 사용)
        float killAnimationLength = 1.5f; // 예: 킬 애니메이션의 총 길이 (초)
        Invoke("ShowGameOver", killAnimationLength);

    }

    void ShowGameOver()
    {
        Debug.Log("GAME OVER");
        // 여기에 게임 오버 UI를 띄우거나 씬을 다시 로드하는 코드를 넣습니다.
        // 예: FindObjectOfType<GameManager>().DisplayGameOverScreen();
        // 예: UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");

        // 3. 게임 오버 시 조명 정리 (씬 리로드 시 불필요할 수 있음)
        if (jumpscareLight != null)
        {
            jumpscareLight.enabled = false;
        }
    }
    
    public void setMonsterState(int state)  //NORMAL(1), CHASE(2), STUN(3)
    {
        monsterState = state;
    }

    private IEnumerator BreakLamp()
    {
        float originspeed = monster.speed;

        if (monster.isOnNavMesh)
        {
            monster.isStopped = true;
        }
        animator.SetFloat("Speed", 0f);

        Debug.Log("깨기 애니메이션");
        animator.SetTrigger("doBreak");

        if (target.CompareTag("Lamp"))
        {
            //Debug.Log("�μ�");
            LampManager.Instance.BreakLamp();
        }

        yield return new WaitForSeconds(1.3f);

        if (monster.isOnNavMesh) // NavMesh ���� ���� ����
        {
            monster.isStopped = false;
        }
        animator.SetFloat("Speed", originspeed);

        monsterState = NORMAL;

    }

    // 4. ���͸� ���� �ð� ���߰� �ϴ� �ڷ�ƾ
    private IEnumerator PauseMonster(float duration)
    {
        // 5. ���� ���·� ����
        isPaused = true;
        float originspeed = monster.speed;
        monsterState = STUN;
        animator.SetFloat("Speed", 0f);
        animator.SetBool("isStunned", true);
        // 6. NavMeshAgent�� �̵��� ����
        //    (agent.enabled = false; ���� �� ����� �� �����մϴ�)
        if (monster.isOnNavMesh) // NavMesh ���� ���� ����
        {
            monster.isStopped = true;
        }

        // 7. ������ �ð�(duration)��ŭ ���
        yield return new WaitForSeconds(duration);

        // 8. 3�ʰ� ���� ��, NavMeshAgent�� �̵��� �ٽ� ����
        if (monster.isOnNavMesh)
        {
            monster.isStopped = false;
        }

        Debug.Log(Vector3.Distance(this.transform.position, player.position));

        // --- (����) �ִϸ��̼� �ٽ� ��� ---
        // if (animator != null)
        // {
        //     animator.speed = 1; // �ִϸ��̼� �ӵ��� �ٽ� 1��
        // }
        // ---------------------------------

        // 9. ���� ���� ����
        isPaused = false;
        animator.SetFloat("Speed", originspeed);
        animator.SetBool("isStunned", false);

        if (CheckSight() == false)
        {
            monsterState = NORMAL;
            deltatDistance = 10.0f;
            lampManager.SetMonsterTargetToRandomLamp();
        }

        currentPauseCoroutine = null;
    }

    public bool CheckSight()
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
            Debug.Log("발견!");
            setMonsterState(CHASE);
            return true;
            // ���⿡ �÷��̾ �߰����� ���� ������ �߰� (��: �߰� ����)
        }
    }

    // (��) ����� ����ϸ� ��(Scene) �信�� �þ߰��� �ð������� Ȯ���� �� �ֽ��ϴ�.
    // 선택했을 때만 기즈모를 그립니다. (항상 보고 싶으면 OnDrawGizmos로 이름 변경)
    void OnDrawGizmosSelected()
    {
        // 1. 눈 위치 가져오기 (로직과 동일하게)
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : transform.position;

        // 2. 시야 거리(반지름) 그리기 - 노란색 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePos, viewRadius);

        // 3. 시야각(부채꼴) 그리기
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(eyePos, eyePos + viewAngleA * viewRadius);
        Gizmos.DrawLine(eyePos, eyePos + viewAngleB * viewRadius);

        // 4. 플레이어와의 연결 선 그리기
        if (player != null)
        {
            // 플레이어가 시야 안에 있고 장애물이 없으면 초록색, 아니면 빨간색
            if (canSeePlayer)
            {
                Gizmos.color = Color.green;
                // 플레이어를 보고 있다는 것을 명확히 선으로 연결
                Gizmos.DrawLine(eyePos, player.position);
            }
            else
            {
                Gizmos.color = Color.red;
                // 플레이어 방향으로 선을 긋되, 시야 거리까지만 표시 (디버깅용)
                // 실제 플레이어 위치까지 긋고 싶으면 player.position을 사용하세요.
                Gizmos.DrawLine(eyePos, player.position);
            }
        }
    }

    // 각도를 벡터(방향)로 변환해주는 헬퍼 함수
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
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
        if(monster != null)
        {
            currentPauseCoroutine = StartCoroutine(PauseMonster(duration));

        }
    }
}
