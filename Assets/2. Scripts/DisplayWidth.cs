using UnityEngine;
using TMPro; // TextMeshPro용

public class DisplayWidth : MonoBehaviour
{
    public RectTransform targetUI;   // 너비 확인할 UI
    public TMP_Text widthText;        // TextMeshPro용 텍스트

    void Awake()
    {
        if (widthText == null)
            widthText = GetComponent<TMP_Text>(); // 같은 오브젝트 Text 자동 연결
    }

    void Update()
    {
        if (targetUI != null && widthText != null)
        {
            float width = targetUI.rect.width;
            widthText.text = "Battery Level: " + width.ToString("F1") + "%";
        }
    }
}