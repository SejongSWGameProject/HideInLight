using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurvedWireLine : Graphic
{
    [SerializeField] private float lineWidth = 5f;
    [SerializeField] private int curveResolution = 20; // 곡선 해상도
    [SerializeField] private float curvature = 0.5f; // 곡선 정도 (0~1)

    private Vector2 startPos;
    private Vector2 endPos;
    private List<Vector2> linePoints = new List<Vector2>();

    public void SetPositions(Vector2 start, Vector2 end)
    {
        startPos = start;
        endPos = end;
        CalculateCurvePoints();
        SetVerticesDirty();
    }

    public void SetLineWidth(float width)
    {
        lineWidth = width;
        SetVerticesDirty();
    }

    public void SetCurvature(float curve)
    {
        curvature = Mathf.Clamp01(curve);
        CalculateCurvePoints();
        SetVerticesDirty();
    }

    void CalculateCurvePoints()
    {
        linePoints.Clear();

        // 베지어 곡선 계산
        Vector2 midPoint = (startPos + endPos) / 2f;
        Vector2 direction = (endPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        Vector2 controlPoint = midPoint + perpendicular * Vector2.Distance(startPos, endPos) * curvature;

        // 곡선 포인트 생성
        for (int i = 0; i <= curveResolution; i++)
        {
            float t = i / (float)curveResolution;
            Vector2 point = CalculateQuadraticBezierPoint(t, startPos, controlPoint, endPos);
            linePoints.Add(point);
        }
    }

    Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return point;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (linePoints.Count < 2)
            return;

        // 선 메쉬 생성
        for (int i = 0; i < linePoints.Count - 1; i++)
        {
            Vector2 point = linePoints[i];
            Vector2 nextPoint = linePoints[i + 1];

            Vector2 direction = (nextPoint - point).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * lineWidth / 2f;

            // 사각형 만들기
            int startIndex = vh.currentVertCount;

            vh.AddVert(point - perpendicular, color, Vector2.zero);
            vh.AddVert(point + perpendicular, color, Vector2.zero);
            vh.AddVert(nextPoint + perpendicular, color, Vector2.zero);
            vh.AddVert(nextPoint - perpendicular, color, Vector2.zero);

            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }
    }

    public void HideLine()
    {
        linePoints.Clear();
        SetVerticesDirty();
    }
}