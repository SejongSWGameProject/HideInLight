using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSceneManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image backgroundImage;      // 배경 이미지
    public TextMeshProUGUI narrationText; // 텍스트
    public Image fadeImage;            // 페이드 효과용 검은 패널 (추가됨)

    [Header("설정")]
    public float fadeDuration = 2.0f;  // 페이드인 시간 (초)
    public float typingSpeed = 0.05f;  // 타자 속도

    [Header("데이터")]
    public List<EndingData> endingDataList;

    private EndingData currentEnding;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isFading = false; // 페이드 중 클릭 방지

    void Start()
    {
        // 1. 엔딩 데이터 로드
        string savedEnding = PlayerPrefs.GetString("EndingResult", "Normal");
        EndingType type = (EndingType)System.Enum.Parse(typeof(EndingType), savedEnding);
        currentEnding = endingDataList.Find(x => x.endingType == type);

        // 2. 초기화 및 연출 시작
        if (currentEnding != null)
        {
            backgroundImage.sprite = currentEnding.backgroundImage;
            narrationText.text = ""; // 텍스트 비우기

            // 코루틴 시작: 페이드인 -> 첫 대사 출력
            StartCoroutine(StartEndingSequence());
        }
    }

    IEnumerator StartEndingSequence()
    {
        isFading = true;

        // (1) 페이드 이미지 알파값 1(불투명)로 시작
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // (2) 서서히 투명해지기
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 확실하게 투명하게 만들고 비활성화 (클릭 방해 안 되게)
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);

        isFading = false;

        // (3) 페이드 끝나면 첫 대사 시작
        StartCoroutine(TypeWriterEffect(currentEnding.narrationLines[0]));
    }

    void Update()
    {
        // 페이드 중에는 클릭 무시
        if (isFading) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                narrationText.text = currentEnding.narrationLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentEnding.narrationLines.Length)
        {
            StartCoroutine(TypeWriterEffect(currentEnding.narrationLines[currentLineIndex]));
        }
        else
        {
            GoToMainMenu();
        }
    }

    IEnumerator TypeWriterEffect(string line)
    {
        isTyping = true;
        narrationText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            narrationText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void GoToMainMenu()
    {
        Debug.Log("메인 메뉴로 이동");
        // SceneManager.LoadScene("MainMenu");
    }
}
