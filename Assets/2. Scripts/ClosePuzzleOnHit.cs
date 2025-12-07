using UnityEngine;

public class ClosePuzzleOnHit : MonoBehaviour
{
    [Header("닫을 퍼즐 UI")]
    public GameObject puzzlePopupAnchor;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했을 때만
        if (other.CompareTag("Player"))
        {
            // 퍼즐 UI가 연결되어 있고, 현재 켜져 있다면
            if (puzzlePopupAnchor != null && puzzlePopupAnchor.activeSelf)
            {
                puzzlePopupAnchor.SetActive(false); // 그냥 끈다 (다른 건 건드리지 않음)
            }
        }
    }
}