using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterTMP : MonoBehaviour
{
    public TMP_Text uiText;     // TextMeshPro
    public float delay = 0.5f;  // 글자 출력 간격
    private string originalText; // Inspector에 적어둔 텍스트 저장

    void Awake()
    {
        if (uiText == null)
            uiText = GetComponent<TMP_Text>();

        originalText = uiText.text; // 기존 텍스트 저장
        uiText.text = "";           // 게임 시작 전에는 숨기기
    }

    void Start()
    {
        StartCoroutine(ShowTextWithDelay());
    }

    IEnumerator ShowTextWithDelay()
    {
        yield return new WaitForSeconds(1f); // 게임 시작 후 1초 대기

        foreach (char c in originalText)
        {
            uiText.text += c;                  // 한 글자씩 추가
            yield return new WaitForSeconds(delay);
        }
    }
}
