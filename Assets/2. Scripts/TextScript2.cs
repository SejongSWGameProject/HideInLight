using System.Collections;
using UnityEngine;
using TMPro;

public class TextScript2 : MonoBehaviour
{
    public TMP_Text uiText;           // TextMeshProUGUI
    public float fadeInDuration = 40f; // 서서히 나타나는 시간
    public float stayDuration = 50f;   // 최대 밝기 유지 시간
    public float fadeOutDuration = 50f; // 서서히 사라지는 시간

    private Color originalColor;

    void Awake()
    {
        if (uiText == null)
            uiText = GetComponent<TMP_Text>();

        // Rich Text 활성화
        uiText.richText = true;
        uiText.ForceMeshUpdate();

        originalColor = uiText.color;
        // 처음에는 완전히 투명
        uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f); // 시작 전 대기
        ShowText();
    }

    public void ShowText()
    {
        StartCoroutine(FadeInStayFadeOut());
    }

    IEnumerator FadeInStayFadeOut()
    {
        float time = 0f;
        while (time < fadeInDuration)
        {
            float t = Mathf.SmoothStep(0f, 10f, time / fadeInDuration); // 부드러운 easing
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            time += Time.deltaTime;
            yield return null;
        }
        uiText.color = originalColor;

        // ===== 2) 최대 밝기 유지 =====
        yield return new WaitForSeconds(stayDuration);

        // ===== 3) 페이드 아웃 =====
        time = 0f;
        while (time < fadeOutDuration)
        {
            float t = Mathf.SmoothStep(10f, 0f, time / fadeOutDuration); // 부드러운 easing
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            time += Time.deltaTime;
            yield return null;
        }
        uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // 완전히 투명하게 마무리
        uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}