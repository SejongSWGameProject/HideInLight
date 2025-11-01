using UnityEngine;

public class ClickableSwitch : MonoBehaviour
{
    [Header("상태 및 색상")]
    public bool isOn = false; 
    public Color onColor = Color.green;
    public Color offColor = Color.red;

    [Header("연결")]
    // ▼▼▼ 여기 타입이 바뀌었습니다! ▼▼▼
    public SwitchPuzzleManager puzzleManager; // 연결할 스위치 퍼즐 매니저

    private MeshRenderer myRenderer; 

    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
        UpdateColor();
    }

    private void OnMouseDown()
    {
        isOn = !isOn; 
        UpdateColor();

        // 스위치 퍼즐 매니저에게 상태 변경을 알림
        if (puzzleManager != null)
        {
            puzzleManager.CheckPuzzleSolution();
        }
    }

    void UpdateColor()
    {
        if (myRenderer != null)
        {
            myRenderer.material.color = isOn ? onColor : offColor;
        }
    }
}