using UnityEngine;
using UnityEngine.UI;

public class WireLine : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image lineImage;
    private RectTransform startPoint;
    private Vector2 startPosition;
    private Color lineColor;
    private float lineWidth;

    public void Initialize(RectTransform start, Color color, float width)
    {
        startPoint = start;
        startPosition = start.anchoredPosition;
        lineColor = color;
        lineWidth = width;

        // RectTransform 설정
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);

        // Image 컴포넌트 추가
        lineImage = gameObject.AddComponent<Image>();
        lineImage.color = color;
        lineImage.raycastTarget = false;

        HideLine();
    }

    public void UpdateLine(Vector2 endPosition)
    {
        if (rectTransform == null || lineImage == null) return;

        // 시작점은 원래 위치 사용
        Vector2 direction = endPosition - startPosition;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 선 위치와 크기 설정
        rectTransform.anchoredPosition = startPosition;
        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        lineImage.enabled = true;
    }

    public void HideLine()
    {
        if (lineImage != null)
            lineImage.enabled = false;
    }

    public void SetColor(Color color)
    {
        lineColor = color;
        if (lineImage != null)
            lineImage.color = color;
    }
}