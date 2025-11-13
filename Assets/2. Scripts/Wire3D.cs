using UnityEngine;

// 이 스크립트가 붙은 프리팹은 'LineRenderer'가 반드시 필요합니다.
[RequireComponent(typeof(LineRenderer))]
public class Wire3D : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // 항상 점 2개(시작, 끝)
        lineRenderer.useWorldSpace = true; // 3D 팝업이 움직여도 월드 좌표 기준
    }

    /// <summary>
    //... (이하 코드 동일)
    /// </summary>
    public void SetProperties(Vector3 startPos, Vector3 endPos, Color color)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // 머티리얼의 색상을 변경 (또는 startColor/endColor 사용)
        lineRenderer.material.color = color;
    }

    /// <summary>
    //... (이하 코드 동일)
    /// </summary>
    public void UpdateEndPosition(Vector3 endPos)
    {
        lineRenderer.SetPosition(1, endPos);
    }
}