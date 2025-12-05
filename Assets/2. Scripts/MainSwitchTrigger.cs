using UnityEngine;

public class MainSwitchTrigger : MonoBehaviour
{
    [Header("연결해야 할 것들")]
    public GameObject wallHandleModel; 
    public GameObject puzzleSystemObject;

    public TextScript noLeverText;
    public TextScript mainFirstText;

    [Header("순서 설정 (선택사항)")]
    [Tooltip("이 발전기보다 먼저 풀어야 하는 발전기가 있다면 여기에 넣으세요.")]
    public MainSwitchTrigger previousGenerator; 

    // 외부에서 확인할 수 있게 public으로 변경 (중요!)
    public bool isCleared = false; 

    public void TryInteract()
    {
        if (isCleared) return; 

        // ★★★ [추가됨] 순서 체크 로직 ★★★
        // 1. 만약 선행 발전기가 연결되어 있는데, 그게 아직 안 풀렸다면?
        if (previousGenerator != null && previousGenerator.isCleared == false)
        {
            Debug.Log("⚠️ 메인 발전기(파란색)를 먼저 수리해야 합니다!");
            mainFirstText.ShowTextInstantly();
            // 여기에 "전력이 들어오지 않습니다" 같은 안내 UI를 띄워도 좋습니다.
            return; // 여기서 강제 종료 (문 안 열어줌)
        }

        // 2. 부품 개수 체크 (기존 로직)
        if (SwitchManager.Instance != null)
        {
            if (SwitchManager.Instance.currentCount > 0)
            {
                OpenPuzzle();
                
                // 만약 4개를 다 모은 상태라면 손잡이 미리 보여주기
                if (SwitchManager.Instance.IsReady())
                {
                    if(wallHandleModel != null) wallHandleModel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("부품이 하나도 없습니다.");
                noLeverText.ShowTextInstantly();
            }
        }
    }

    void OpenPuzzle()
    {
        if (puzzleSystemObject != null)
        {
            puzzleSystemObject.SetActive(true);
        }
    }
    
    // 퍼즐 성공 시 호출
    public void SetCleared()
    {
        isCleared = true;
        if(wallHandleModel != null) wallHandleModel.SetActive(true);
    }
}