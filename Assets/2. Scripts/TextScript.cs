using System.Collections;
using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour
{
    public TMP_Text uiText;       // TextMeshPro
    public float delay = 0.5f;    // 글자 출력 간격
    public float fadeDuration = 1.5f; // 서서히 사라지는 시간
    public float stayDuration = 2.0f; // 텍스트가 유지되는 시간

    private string originalText;
    private Coroutine currentRoutine;

    void Awake()
    {
        if (uiText == null)
            uiText = GetComponent<TMP_Text>();

        originalText = uiText.text;
        uiText.text = "";
        gameObject.SetActive(true);
    }

    void Start()
    {
    }

    public void ShowTextInstantly()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(ShowInstantlyRoutine());
    }

    public void ShowTextInstantly(string s)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(ShowInstantlyRoutine(s));
    }

    public void ShowFindGeneratorText()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ShowTextWithDelay());

        }
    }

    IEnumerator ShowInstantlyRoutine()
    {
        // 1) 텍스트와 투명도 즉시 초기화 (한번에 등장)
        uiText.text = originalText;
        Color c = uiText.color;
        uiText.color = new Color(c.r, c.g, c.b, 1f); // 알파값 1로 복구

        // 2) 지정된 시간만큼 대기 (유지)
        yield return new WaitForSeconds(stayDuration);

        // 3) 페이드 아웃 시작
        yield return StartCoroutine(FadeOut());

        currentRoutine = null;
    }

    IEnumerator ShowInstantlyRoutine(string s)
    {
        // 1) 텍스트와 투명도 즉시 초기화 (한번에 등장)
        uiText.text = s+originalText;
        Color c = uiText.color;
        uiText.color = new Color(c.r, c.g, c.b, 1f); // 알파값 1로 복구

        // 2) 지정된 시간만큼 대기 (유지)
        yield return new WaitForSeconds(stayDuration);

        // 3) 페이드 아웃 시작
        yield return StartCoroutine(FadeOut());

        currentRoutine = null;
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
        yield return new WaitForSeconds(stayDuration);

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
