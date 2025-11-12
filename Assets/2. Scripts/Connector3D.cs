using UnityEngine;

// 이 스크립트가 붙은 오브젝트는 'Collider'가 반드시 필요합니다.
[RequireComponent(typeof(Collider))]
public class Connector3D : MonoBehaviour
{
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
}