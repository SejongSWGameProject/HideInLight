using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    public static SwitchManager Instance;
    public TextScript leverCntText;

    [Header("상태")]
    public int currentCount = 0;   // 현재 주머니에 있는 부품 개수
    public int maxTotalParts = 8;  // 맵 전체에 있는 부품 총 개수 (참고용)

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CollectPart()
    {
        currentCount++;
        Debug.Log($"부품 획득! 현재 보유량: {currentCount}");
        leverCntText.ShowTextInstantly("" + currentCount);
    }

    // ★ [추가됨] 부품 사용하기 (정답 맞췄을 때 호출)
    public void ConsumeParts(int amount)
    {
        currentCount -= amount;
        if (currentCount < 0) currentCount = 0;
        
        Debug.Log($"부품 {amount}개 사용함. 남은 개수: {currentCount}");
    }

    // 퍼즐을 풀기에 충분한지 확인 (4개 이상인가?)
    public bool IsReady()
    {
        return currentCount >= 4;
    }
}