using UnityEngine;

public class Transformer : MonoBehaviour
{
    public GameObject puzzlePanel; // Inspector에서 드래그로 연결
    private bool isPuzzleActive = false;

    public void Interact()
    {
        if (isPuzzleActive) return;
        Debug.Log("변압기와 상호작용!");
        puzzlePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPuzzleActive = true;
    }

    public void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPuzzleActive = false;
        Debug.Log("퍼즐 완료, 변압기 완료 처리!");
        // 여기에 변압기 작동 애니메이션/사운드/스위치 로직 추가 가능
    }
}
