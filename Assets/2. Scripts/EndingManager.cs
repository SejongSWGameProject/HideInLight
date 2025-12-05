using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance; // 어디서든 접근 가능하게 싱글톤 처리
    public NavMeshAgent agent;

    [Header("Inventory Flags")]
    public bool hasInjector = false;      // 주사기 보유 여부
    public bool hasMonsterBlood = false; // 괴물 피 보유 여부
    public bool hasFlare = false;        // 조명탄 보유 여부

    [Header("Ending UI / Scenes")]
    public string happyEndingSceneName = "HappyEnding";
    public string badEndingSceneName = "BadEnding";

    public AudioClip getBloodSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 1. 해피 엔딩 조건 체크 (탈출구에서 호출)
    public void TryEscape()
    {
        if (hasMonsterBlood)
        {
            Debug.Log("해피 엔딩: 백신을 가지고 탈출 성공!");
            //SceneManager.LoadScene(happyEndingSceneName);
        }
        else
        {
            Debug.Log("탈출 실패: 아직 괴물의 피가 없습니다.");
            // 여기에 "피를 뽑아야 해" 같은 UI 메시지를 띄우면 좋습니다.
        }
    }

    // 2. 폭사 엔딩 실행 (드럼통에서 호출)
    public void TriggerExplosionEnding()
    {
        Debug.Log("폭사 엔딩: 다 같이 죽자!");
        // 바로 씬을 넘기기보다, 펑 터지는 효과 후 넘기는 게 좋으므로 코루틴 등 사용 권장
        //Invoke("LoadBadEndingScene", 2.0f); // 2초 뒤 이동
    }

    void LoadBadEndingScene()
    {

        //SceneManager.LoadScene(badEndingSceneName);
    }



    public void GetBlood()
    {
        EndingManager.Instance.hasMonsterBlood = true;

        InteractableObject monsterInteractor = agent.GetComponent<InteractableObject>();
        monsterInteractor.canInteract = false;

        if(getBloodSound != null)
        {
            audioSource.PlayOneShot(getBloodSound);
        }
    }
}
