using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 나중에 유니티 에디터에서 PuzzlePanel 오브젝트를 여기다 연결할 겁니다.
    public GameObject puzzlePanel;

    // 패널을 닫는 기능을 하는 함수입니다.
    public void ClosePuzzlePanel()
    {
        Debug.Log("닫기 버튼이 클릭되었습니다!"); 
        // puzzlePanel이 null이 아닐 때만 실행되도록 안전장치를 추가합니다.
        if (puzzlePanel != null)
        {
            // puzzlePanel 게임 오브젝트를 비활성화 시켜서 화면에서 안보이게 합니다.
            puzzlePanel.SetActive(false);
            Debug.Log("패널 닫힘!"); // 콘솔에 메시지를 출력해서 확인
        }
    }

    // (참고) 혹시 패널을 여는 기능도 이 스크립트로 관리하고 싶다면 아래 함수를 사용하세요.
    public void OpenPuzzlePanel()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            Debug.Log("패널 열림!");
        }
    }
}