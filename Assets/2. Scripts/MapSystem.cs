using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject mapPanel; // 1단계에서 만든 지도 UI(MapPanel)를 여기에 드래그

    [Header("상태")]
    public bool hasMap = false; // 지도를 먹었는지 여부
    private bool isMapOpen = false; // 지금 지도를 보고 있는지 여부

    void Update()
    {
        // 지도를 가지고 있을 때만 M키가 작동
        if (hasMap && Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    void ToggleMap()
    {
        isMapOpen = !isMapOpen; // 상태 반전 (켜짐 <-> 꺼짐)
        mapPanel.SetActive(isMapOpen); // UI 껐다 켜기

        // (선택사항) 지도 볼 때 게임 일시정지 하려면 아래 주석 해제
        /*
        if (isMapOpen) 
        {
            Time.timeScale = 0; // 시간 정지
            Cursor.lockState = CursorLockMode.None; // 마우스 커서 보이기
            Cursor.visible = true;
        }
        else 
        {
            Time.timeScale = 1; // 시간 흐름
            Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 숨기기
            Cursor.visible = false;
        }
        */
    }

    // 아이템을 먹었을 때 호출할 함수 (외부에서 부름)
    public void GetMapItem()
    {
        hasMap = true;
        Debug.Log("지도 획득! 이제 M키를 누를 수 있습니다.");
        
        // 여기에 "지도를 획득했습니다" 같은 안내 UI를 띄워줘도 좋음
    }
}