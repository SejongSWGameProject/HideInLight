using UnityEngine;

public class WirePort : MonoBehaviour
{
    public enum PortType { Top, Bottom }

    [Header("포트 설정")]
    public PortType type; 
    public int portID;    

    // ★★★ [추가] 전선이 꽂힐 정확한 위치 (빈 오브젝트 연결)
    [Header("연결 지점 (비어 있으면 자기 자신 위치 사용)")]
    public Transform connectionPoint; 
}