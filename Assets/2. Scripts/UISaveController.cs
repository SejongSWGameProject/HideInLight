using UnityEngine;

public class UISaveController : MonoBehaviour
{
    public UIState state;             // UIState.asset 연결
    public RectTransform shape;       // 조절한 UI 오브젝트

    public void SaveUIState()
    {
        // RectTransform width 저장
        state.length = shape.sizeDelta.x;
        Debug.Log("UI 길이 저장됨: " + state.length);
    }
    public void LoadUIState()
    {
        if (state != null && shape != null)
        {
            // 저장된 길이를 UI 도형에 적용
            shape.sizeDelta = new Vector2(state.length, shape.sizeDelta.y);

            Vector2 pos = shape.anchoredPosition;
            pos.x -= (50f-state.length)/2f; // 왼쪽 이동
            shape.anchoredPosition = pos;

            Debug.Log("UI 길이 불러오기 완료: " + state.length);
        }
    }
}