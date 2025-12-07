using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    public Transform target;         // 플레이어 Transform
    private NavMeshAgent monster;      // 몬스터 이동 에이전트
    private Animator animator;

    public const int NORMAL = 1;
    public const int CHASE = 2;
    public const int STUN = 3;
    public const int BREAK = 4;
    public int monsterState;           //1:����(��������ٴϴ���)   2:�÷��̾��Ѵ���   3:���ϸ���

    public Transform player;         // 플레이어 Transform
    public float viewRadius = 80f; // 시야 반경
    [Range(0, 360)]
    public float viewAngle = 300f;  // 시야각
    public LayerMask obstacleMask; // 장애물 레이어 마스크

    [SerializeField] LampManager lampManager;

    private bool isPaused = false;
    private float deltatDistance = 10f;
    private Vector3 prevPosition;

    // "눈"의 위치
    public Transform eyePosition;
    public Transform playerCamera;
    public Transform jumpScareCameraPos;

    public float killDistance = 1.2f; // 점프 스케어 거리
    public AudioClip jumpscareSound;
    private AudioSource audioSource;
    public Light jumpscareLight;

    private bool isJumpscaring = false;

    private Coroutine currentPauseCoroutine;

    public AudioClip[] footstepClips;
    public float stepInterval = 1f;
    private float stepTimer = 0f;

    public AudioClip sufferSound;
    public AudioClip growlClip;
    private AudioSource fadeOutAudioSource;

    public bool isCrazy = false;

    // 플레이어를 봤는지 여부
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
        fadeOutAudioSource = gameObject.AddComponent<AudioSource>();
        fadeOutAudioSource.playOnAwake = false;

        if (monster == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트를 찾을 수 없습니다!");
            return;
        }
    }

    private void OnEnable()
    {
        monsterState = NORMAL;
        Debug.Log("노말로 변경");
    }

    void Update()
    {
        if (isJumpscaring)
        {
            return;
        }

        animator.SetFloat("Speed", monster.speed);

        if (!isPaused)
        {
            animator.SetFloat("Speed", monster.speed);
        }

        Vector3 targetPosWithoutY = new Vector3(target.position.x, 0f, target.position.z);
        
        if (target != null && monster.isOnNavMesh)
        {
            monster.SetDestination(targetPosWithoutY);
        }
        else
        {
            // Debug.LogWarning("목표나 NavMesh가 유효하지 않습니다!");
        }

        if (monster.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayRandomFootStep();
                stepTimer = stepInterval; 
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (monsterState == NORMAL)
        {
            stepInterval = 1.1f;
            monster.speed = 10;
            animator.SetFloat("animSpeed", 1.0f);
            CheckSight();
            
            if(monster.velocity.magnitude < 1f && Vector3.Distance(this.transform.position, this.target.transform.position) < 20)
            {
                StartCoroutine(BreakLamp());
            }
        }
        else if (monsterState == CHASE)
        {
            stepInterval = 0.7f;
            monster.speed = 30;
            animator.SetFloat("animSpeed", 3.0f);
            target = player;
        }

        if (CanKillPlayer()) 
        {
            StartJumpScare();
        }
    }

    IEnumerator PlayAndFadeOut(AudioClip clip, float playDuration, float fadeDuration)
    {
        fadeOutAudioSource.volume = 1f;
        fadeOutAudioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(playDuration);

        float startVolume = fadeOutAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeOutAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        fadeOutAudioSource.Stop();
        fadeOutAudioSource.volume = 1f; 
    }

    void PlayRandomFootStep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
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
        return false;
    }

    // ▼▼▼▼▼▼ [수정된 부분] ▼▼▼▼▼▼
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

        animator.SetFloat("Speed", 0f);

        // 2. 플레이어 컨트롤 정지
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

        // 플레이어 Rigidbody 정지
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 마우스 커서 설정
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 점프스케어 조명 켜기
        if (jumpscareLight != null)
        {
            jumpscareLight.enabled = true; 
        }

        // ========================================================
        // [핵심 변경 사항] 몬스터 위치 재설정 (옆에서 덮치는 연출)
        // ========================================================
        
        // 1. 플레이어 기준으로 "앞쪽 0.8m" + "오른쪽 0.5m" 위치 계산
        // (오른쪽에서 덮치는 느낌을 주려면 player.right 값을 조절하세요)
        Vector3 safeSpawnPos = player.position + (player.forward * 0.8f) + (player.right * 0.5f);
        
        // 2. 높이(Y)는 플레이어와 같게 하여 바닥 뚫림 방지
        safeSpawnPos.y = player.position.y;

        // 3. 몬스터 강제 이동 및 플레이어 바라보기
        monster.transform.position = safeSpawnPos;
        monster.transform.LookAt(player.position);

        // 4. 카메라 위치 이동
        // (몬스터를 먼저 이동시켰으므로, 몬스터 자식인 카메라도 적절한 위치로 따라옵니다)
        if(jumpScareCameraPos != null)
        {
             playerCamera.position = jumpScareCameraPos.position;
        }
        
        // 5. 카메라는 몬스터의 눈(얼굴)을 바라봄
        if (eyePosition != null)
        {
            playerCamera.LookAt(eyePosition);
        }
        else
        {
            playerCamera.LookAt(transform.position + Vector3.up * 1.5f);
        }

        // 애니메이션 및 사운드
        animator.SetTrigger("KillPlayer");

        if (jumpscareSound != null && audioSource != null)
        {
            fadeOutAudioSource.PlayOneShot(jumpscareSound);
        }

        // 5. 게임 오버 처리 (아래 3단계 참고)
        // (이벤트 또는 Invoke 사용)
        float killAnimationLength = 2f; // 예: 킬 애니메이션의 총 길이 (초)
        Invoke("ShowGameOver", killAnimationLength);
    }
    // ▲▲▲▲▲▲ [수정 끝] ▲▲▲▲▲▲

    void ShowGameOver()
    {
        Debug.Log("GAME OVER");
        if (jumpscareLight != null)
        {
            jumpscareLight.enabled = false;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    public void setMonsterState(int state)
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

        //Debug.Log("깨기 애니메이션");
        animator.SetTrigger("doBreak");

        if (target.CompareTag("Lamp"))
        {
            LampManager.Instance.BreakLamp();
        }

        yield return new WaitForSeconds(1.3f);

        if (monster.isOnNavMesh)
        {
            monster.isStopped = false;
        }
        animator.SetFloat("Speed", originspeed);

        monsterState = NORMAL;
    }

    private IEnumerator PauseMonster(float duration)
    {
        StartCoroutine(PlayAndFadeOut(sufferSound, 3f, 0.5f));
        
        isPaused = true;
        float originspeed = monster.speed;
        monsterState = STUN;
        animator.SetFloat("Speed", 0f);
        animator.SetBool("isStunned", true);
        
        if (monster.isOnNavMesh)
        {
            monster.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        if (monster.isOnNavMesh)
        {
            monster.isStopped = false;
        }

        isPaused = false;
        animator.SetFloat("Speed", originspeed);
        animator.SetBool("isStunned", false);

        if (!isCrazy)
        {
            if (CheckSight() == false)
            {
                monsterState = NORMAL;
                deltatDistance = 10.0f;
                lampManager.SetMonsterTargetToRandomLamp();
            }
        }

        currentPauseCoroutine = null;
    }

    public bool CheckSight()
    {
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : this.transform.position;

        float distanceToPlayer = Vector3.Distance(eyePos, player.position);
        if (distanceToPlayer > viewRadius)
        {
            canSeePlayer = false;
            return false;
        }

        Vector3 directionToPlayer = (player.position - eyePos).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > viewAngle / 2)
        {
            canSeePlayer = false;
            return false;
        }

        if (Physics.Raycast(eyePos, directionToPlayer, distanceToPlayer, obstacleMask))
        {
            canSeePlayer = false;
            return false;
        }
        else
        {
            canSeePlayer = true;
            Debug.Log("발견!");
            setMonsterState(CHASE);
            return true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eyePos = (eyePosition != null) ? eyePosition.position : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePos, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.DrawLine(eyePos, eyePos + viewAngleA * viewRadius);
        Gizmos.DrawLine(eyePos, eyePos + viewAngleB * viewRadius);

        if (player != null)
        {
            if (canSeePlayer)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(eyePos, player.position);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(eyePos, player.position);
            }
        }
    }

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
            prevPosition = transform.position;
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