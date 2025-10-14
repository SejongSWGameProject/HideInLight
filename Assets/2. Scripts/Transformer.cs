using UnityEngine;

public class Transformer : MonoBehaviour
{
    public GameObject PuzzlePanel; // 연결할 패널

    // 마우스로 3D 오브젝트 클릭 시 호출됨
    void OnMouseDown()
    {
        Debug.Log("변압기 클릭됨!");

        if (PuzzlePanel != null)
        {
            PuzzlePanel.SetActive(true);
            Debug.Log("퍼즐 패널 열림!");
        }
        else
        {
            Debug.LogWarning("PuzzlePanel이 연결되지 않았습니다!");
        }
    }
}
