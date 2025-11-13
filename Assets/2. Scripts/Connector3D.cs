using UnityEngine;

// 이 스크립트가 붙은 오브젝트는 'Collider'가 반드시 필요합니다.
[RequireComponent(typeof(Collider))]
public class Connector3D : MonoBehaviour
{
    // ▼▼▼ [추가됨] 1. 전선이 시작될 정확한 위치 ▼▼▼
    [Header("전선 연결 지점")]
    [Tooltip("이 포트의 시각적 중심. 비어있으면 이 오브젝트의 Transform을 사용합니다.")]
    public Transform wireSnapPoint;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    // PuzzleManager3D가 이 값들을 읽어갑니다.
    public int ConnectorId { get; private set; }
    public bool IsLeft { get; private set; }
    
    // IsConnected는 Manager가 외부에서 변경합니다.
    public bool IsConnected { get; set; }

    /// <summary>
    /// PuzzleManager3D가 퍼즐을 세팅할 때 이 함수를 호출합니다.
    /// </summary>
    public void Initialize(int id, bool isLeft)
    {
        this.ConnectorId = id;
        this.IsLeft = isLeft;
        this.IsConnected = false;
    }

    // ▼▼▼ [추가됨] 2. 전선 시작 위치를 반환하는 함수 ▼▼▼
    /// <summary>
    /// 전선이 시작되어야 할 정확한 3D 위치를 반환합니다.
    /// (Snap Point가 설정되지 않았으면 기본 Transform을 사용합니다)
    /// </summary>
    public Vector3 GetWireStartPosition()
    {
        if (wireSnapPoint != null)
        {
            return wireSnapPoint.position;
        }
        else
        {
            // 'wireSnapPoint'가 비어있을 경우의 안전장치
            return transform.position;
        }
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
}