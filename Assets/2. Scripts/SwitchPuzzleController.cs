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
    public GameObject puzzleLight;     // 퍼즐 전용 포인트 라이트
    public GameObject playerFlashlight; // 플레이어 손전등
    public PlayerLockControl playerLock; // 플레이어 고정 스크립트 (Player 오브젝트에 있는 것)

    [Header("이벤트")]
    public UnityEvent OnPuzzleSolved;    // 성공했을 때
    public UnityEvent OnPuzzleCancelled; // ESC 눌렀을 때

    // 퍼즐이 켜질 때 (F키 눌렀을 때) 자동으로 실행
    void OnEnable()
    {
        // 1. 플레이어 고정 및 마우스 보이기
        if (playerLock != null) playerLock.FreezePlayer();

        // 2. 조명 연출 (플래시 끄고, 퍼즐 조명 켜기)
        if (playerFlashlight != null) playerFlashlight.SetActive(false);
        if (puzzleLight != null) puzzleLight.SetActive(true);

        // 3. 스위치들의 변경 감지 연결
        if (switches == null) return;
        foreach (Switch s in switches)
        {
            if (s == null) continue;
            s.OnToggled.RemoveListener(CheckVictory); // 중복 방지
            s.OnToggled.AddListener(CheckVictory);
        }
    }

    // 퍼즐이 꺼질 때 (닫힐 때) 자동으로 실행
    void OnDisable()
    {
        // 1. 플레이어 풀기 및 마우스 숨기기
        if (playerLock != null) playerLock.UnfreezePlayer();

        // 2. 조명 원상복구
        if (playerFlashlight != null) playerFlashlight.SetActive(true);
        if (puzzleLight != null) puzzleLight.SetActive(false);
    }

    // ★★★ [수정된 부분] Update 함수 ★★★
    void Update()
    {
        // 1. ESC 키 누르면 퍼즐 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzle();
        }

        // 2. 레이캐스트로 스위치 클릭 감지
        if (Input.GetMouseButtonDown(0)) 
        {
            // 카메라에서 마우스 위치로 광선을 쏨
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 광선이 무언가에 맞았다면 (콜라이더 필요!)
            if (Physics.Raycast(ray, out hit))
            {
                // 맞은 물체에서 'Switch' 스크립트를 찾는다.
                Switch clickedSwitch = hit.collider.GetComponent<Switch>();

                // 'Switch' 스크립트를 찾았다면 (즉, 손잡이를 클릭했다면)
                if (clickedSwitch != null)
                {
                    // 찾은 스위치의 Toggle() 함수를 강제로 실행!
                    clickedSwitch.Toggle();
                }
            }
        }
    }
    // ★★★ [수정 끝] ★★★

    // 정답 체크 (스위치 누를 때마다 실행됨)
    void CheckVictory()
    {
        // 개수가 안 맞으면 에러 방지
        if (switches == null || correctSolution == null || switches.Length != correctSolution.Length) return;

        bool isCorrect = true;
        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i] == null) continue; // 혹시 모를 빈칸 방지

            // 하나라도 정답과 다르면 실패
            if (switches[i].isOn != correctSolution[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("스위치 퍼즐 성공!");
            OnPuzzleSolved.Invoke(); // 문 열기 등 이벤트 실행
            
            // 성공했으니 퍼즐 창 닫기
            this.gameObject.SetActive(false);
        }
    }

    // 강제로 닫기 (ESC)
    public void ClosePuzzle()
    {
        OnPuzzleCancelled.Invoke();
        this.gameObject.SetActive(false);
    }
}