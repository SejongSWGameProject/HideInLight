using UnityEngine;
using UnityEngine.Events;

public class SwitchPuzzleController : MonoBehaviour
{
    [Header("퍼즐 구성요소")]
    [Tooltip("여기에 개별 스위치 오브젝트들을 순서대로 넣으세요")]
    public Switch[] switches;

    [Header("정답 설정")]
    [Tooltip("위의 스위치 순서대로 정답(켜짐=체크)을 설정하세요")]
    public bool[] correctSolution; 

    [Header("연출 및 플레이어 제어")]
    public GameObject puzzleLight;      
    public GameObject playerFlashlight; 
    public PlayerLockControl playerLock; 

    [Header("이벤트")]
    public UnityEvent OnPuzzleSolved;    
    public UnityEvent OnPuzzleCancelled; 

    private bool isAlreadySolved = false; // 이미 푼 퍼즐인지 체크

    void OnEnable()
    {
        if (isAlreadySolved) return; 

        if (playerLock != null) playerLock.FreezePlayer();
        if (playerFlashlight != null) playerFlashlight.SetActive(false);
        if (puzzleLight != null) puzzleLight.SetActive(true);

        // ★ 현재 보유 개수에 맞춰 스위치 켜기
        UpdateSwitchesByCount(); 

        if (switches == null) return;
        foreach (Switch s in switches)
        {
            if (s == null) continue;
            s.OnToggled.RemoveListener(CheckVictory);
            s.OnToggled.AddListener(CheckVictory);
        }
    }

    void OnDisable()
    {
        if (playerLock != null) playerLock.UnfreezePlayer();
        if (playerFlashlight != null) playerFlashlight.SetActive(true);
        if (puzzleLight != null) puzzleLight.SetActive(false);
    }

    void Update()
    {
        if (isAlreadySolved) return; 

        if (Input.GetKeyDown(KeyCode.Escape)) ClosePuzzle();

        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Switch clickedSwitch = hit.collider.GetComponent<Switch>();
                if (clickedSwitch != null) clickedSwitch.Toggle();
            }
        }
    }

    // ★ 투명망토 모드: 물체는 켜두고(Active True), 눈과 손만 끕니다.
    void UpdateSwitchesByCount()
    {
        if (SwitchManager.Instance == null) return;

        int currentCount = SwitchManager.Instance.currentCount;
        int activeCount = Mathf.Min(currentCount, 4); // 최대 4개까지만 활성
        
        Debug.Log($"🧩 상태 갱신: 보유 {currentCount}개 -> {activeCount}개만 보이고 나머지는 투명해짐");

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i] == null) continue;

            // 보여줄지 말지 결정
            bool shouldShow = (i < activeCount);

            // 1. [중요] 게임 오브젝트는 무조건 켜둡니다! (연결고리 유지)
            switches[i].gameObject.SetActive(true);

            // 2. '그림(Renderer)' On/Off (눈에 보임/안보임)
            Renderer[] renderers = switches[i].GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) r.enabled = shouldShow;

            // 3. '충돌(Collider)' On/Off (클릭 됨/안됨)
            Collider[] colliders = switches[i].GetComponentsInChildren<Collider>(true);
            foreach (var c in colliders) c.enabled = shouldShow;
            
            // 4. (개별 조명이나 캔버스가 있다면 그것도 처리)
            Light[] lights = switches[i].GetComponentsInChildren<Light>(true);
            foreach (var l in lights) l.enabled = shouldShow;
            
            Canvas[] canvases = switches[i].GetComponentsInChildren<Canvas>(true);
            foreach (var c in canvases) c.enabled = shouldShow;
        }
    }

    // ★ 정답 체크 + 상세 로그 + 부품 소비
    void CheckVictory()
    {
        if (switches == null || correctSolution == null) return;

        // 4개가 다 안 모였으면 정답 체크 자체를 안 함 (못 푸는 상태)
        if (SwitchManager.Instance.currentCount < 4) return;

        bool isCorrect = true;
        string logMsg = "🔍 채점표: "; // 콘솔에 띄울 메시지

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i] == null) continue; 

            bool current = switches[i].isOn;
            bool target = correctSolution[i];

            // 상세 로그 작성
            if (current != target)
            {
                isCorrect = false;
                logMsg += $"[❌{i}번] "; // 틀림
            }
            else
            {
                logMsg += $"[✅{i}번] "; // 맞음
            }
        }

        // 콘솔창에 결과 출력
        Debug.Log(logMsg);

        if (isCorrect)
        {
            Debug.Log("🎉 정답입니다! 부품 4개를 사용하여 수리합니다.");
            
            // ★ 정답을 맞췄으므로 부품 4개 차감!
            SwitchManager.Instance.ConsumeParts(4);
            
            isAlreadySolved = true; 
            OnPuzzleSolved.Invoke(); 
            this.gameObject.SetActive(false);
        }
    }

    public void ClosePuzzle()
    {
        OnPuzzleCancelled.Invoke();
        this.gameObject.SetActive(false);
    }
}