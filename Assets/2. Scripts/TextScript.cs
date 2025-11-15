using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterTMP : MonoBehaviour
{
    public TMP_Text uiText;       // TextMeshPro
    public float delay = 0.5f;    // 글자 출력 간격
    public float fadeDuration = 1.5f; // 서서히 사라지는 시간

    private string originalText;

    void Awake()
    {
        if (uiText == null)
            uiText = GetComponent<TMP_Text>();

        originalText = uiText.text;
        uiText.text = "";
    }

    void Start()
    {
        StartCoroutine(ShowTextWithDelay());
    }

    IEnumerator ShowTextWithDelay()
    {
        yield return new WaitForSeconds(1f);

        // 1) 타이핑 효과
        foreach (char c in originalText)
        {
            uiText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // 2) 타이핑 끝난 뒤 2초 유지
        yield return new WaitForSeconds(2f);

        // 3) 페이드 아웃 시작
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;
        Color originalColor = uiText.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;

            uiText.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                Mathf.Lerp(1f, 0f, t)
            );

            time += Time.deltaTime;
            yield return null;
        }

        // 완전히 투명하게
        uiText.color = new Color(
            originalColor.r,
            originalColor.g,
            originalColor.b,
            0f
        );
    }
}
