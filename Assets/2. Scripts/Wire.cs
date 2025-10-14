using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Wire : MonoBehaviour
{
    private Image wireImage;
    private RectTransform rectTransform;

    void Awake()
    {
        wireImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);
    }

    public void SetProperties(Vector2 startPos, Vector2 endPos, Color color)
    {
        wireImage.color = color;
        Vector2 dir = endPos - startPos;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rectTransform.position = startPos;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        
        // ▼▼▼ 이 부분이 수정되었습니다 ▼▼▼

        // 1. 커넥터의 반지름 값을 설정합니다. (커넥터 크기가 50x50이면 반지름은 25)
        // 이 값을 조절해서 길이를 맞추세요.
        float connectorRadius = 25f; 

        // 2. 전체 길이(distance)에서 반지름만큼을 빼줍니다.
        rectTransform.sizeDelta = new Vector2(distance - connectorRadius, rectTransform.sizeDelta.y);
    }
}