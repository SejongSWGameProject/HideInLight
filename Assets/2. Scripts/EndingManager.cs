using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance; // 어디서든 접근 가능하게 싱글톤 처리

    [Header("Inventory Flags")]
    public bool hasInjector = false;      // 주사기 보유 여부
    public bool hasMonsterBlood = false; // 괴물 피 보유 여부
    public bool hasFlare = false;        // 조명탄 보유 여부
    public float canUseFlareDistance = 30f;

    [Header("Ending UI / Scenes")]
    public string happyEndingSceneName = "HappyEnding";
    public string badEndingSceneName = "BadEnding";

    public AudioClip getBloodSound;

    private AudioSource audioSource;

    public Transform player;
    public GameObject cannotBoomText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(cannotBoomText != null)
        {
            cannotBoomText.SetActive(true);
        }
    }

    // 1. 해피 엔딩 조건 체크 (탈출구에서 호출)
    public void TryEscape()
    {
        if (hasMonsterBlood)
        {
            Debug.Log("해피 엔딩: 백신을 가지고 탈출 성공!");
            PlayerPrefs.SetString("EndingResult", "Happy");
            PlayerPrefs.Save();
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            Debug.Log("탈출 실패: 아직 괴물의 피가 없습니다.");
            PlayerPrefs.SetString("EndingResult", "Normal");
            PlayerPrefs.Save();
            SceneManager.LoadScene("EndingScene");
            // 여기에 "피를 뽑아야 해" 같은 UI 메시지를 띄우면 좋습니다.
        }
    }

    // 2. 폭사 엔딩 실행 (드럼통에서 호출)
    public void TriggerExplosionEnding()
    {
        bool canBoom = false;
        foreach(MonsterAI monster in MonsterAI.allMonsters)
        {
            Debug.Log(Vector3.Distance(player.transform.position, monster.transform.position) < canUseFlareDistance);
            if(Vector3.Distance(player.transform.position, monster.transform.position) < canUseFlareDistance)
            {
                canBoom = true;
            }
        }
        if (canBoom)
        {
            Debug.Log("폭사 엔딩: 다 같이 죽자!");
            PlayerPrefs.SetString("EndingResult", "Bad");
            PlayerPrefs.Save();
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            TextScript ts = cannotBoomText.GetComponent<TextScript>();
            ts.ShowTextInstantly();
        }
    }

    void LoadBadEndingScene()
    {

        //SceneManager.LoadScene(badEndingSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //NextStage();
            Debug.Log("스테이지 클리어!");
            TryEscape();
        }
    }

    public void GetBlood()
    {
        EndingManager.Instance.hasMonsterBlood = true;

        foreach (MonsterAI m in MonsterAI.allMonsters)
        {
            InteractableObject monsterInteractor = m.GetComponent<InteractableObject>();
            monsterInteractor.canInteract = false;
        }

        if(getBloodSound != null)
        {
            audioSource.PlayOneShot(getBloodSound);
        }
    }
}
