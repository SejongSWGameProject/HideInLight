using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSceneManager : MonoBehaviour
{
    [Header("UI 연결 (Inspector에서 할당)")]
    public Image backgroundImage;       // 배경 이미지가 들어갈 Image
    public TextMeshProUGUI narrationText; // 대사가 나올 Text (TMP)
    public Image fadeImage;             // 검은색 페이드 패널 Image
    public GameObject mainMenuButton;   // 나중에 나타날 '메인으로' 버튼 오브젝트

    [Header("설정")]
    public float fadeDuration = 2.0f;   // 화면이 밝아지는 데 걸리는 시간
    public float typingSpeed = 0.05f;   // 글자가 찍히는 속도

    [Header("엔딩 데이터 (Inspector에서 할당)")]
    public List<EndingData> endingDataList; // 노말, 해피, 배드 데이터 3개를 여기에 넣으세요

    // 내부 변수
    private EndingData currentEnding;
    private int currentLineIndex = 0;
    private bool isTyping = false;      // 현재 타자 치는 중인지 확인
    private bool isFading = false;      // 페이드 효과 중인지 확인

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // 마우스를 자유롭게 풀어줌
        Cursor.visible = true;

        // 1. 시작할 때 메인 메뉴 버튼은 숨김
        if (mainMenuButton != null)
            mainMenuButton.SetActive(false);

        // 2. 저장된 엔딩 결과 불러오기 (기본값: Normal)
        string savedEnding = PlayerPrefs.GetString("EndingResult", "Normal");

        // 문자열을 Enum으로 변환
        EndingType type = (EndingType)System.Enum.Parse(typeof(EndingType), savedEnding);

        // 3. 리스트에서 해당 엔딩 데이터 찾기
        currentEnding = endingDataList.Find(x => x.endingType == type);

        // 4. 데이터가 있다면 엔딩 연출 시작
        if (currentEnding != null)
        {
            // 배경 이미지 교체
            backgroundImage.sprite = currentEnding.backgroundImage;

            // 텍스트 초기화
            narrationText.text = "";

            // 페이드인 효과 시작
            StartCoroutine(StartEndingSequence());
        }
        else
        {
            Debug.LogError("엔딩 데이터를 찾을 수 없습니다! Inspector를 확인해주세요.");
        }
    }

    // 초기 페이드인 연출 코루틴
    IEnumerator StartEndingSequence()
    {
        isFading = true;

        // (1) 검은 화면(페이드 패널) 켜기 및 불투명하게 설정
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // (2) 서서히 투명해지기 (검은색 -> 투명)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp: 선형 보간 (1에서 0으로 부드럽게 변경)
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // (3) 완전히 투명하게 만들고 끄기
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);

        isFading = false;

        // (4) 페이드가 끝나면 첫 번째 대사 출력 시작
        if (currentEnding.narrationLines.Length > 0)
        {
            StartCoroutine(TypeWriterEffect(currentEnding.narrationLines[0]));
        }
    }

    void Update()
    {
        // 페이드 효과 중에는 클릭 무시
        if (isFading) return;

        // 마우스 왼쪽 클릭 또는 스페이스바 입력
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            // 아직 대사가 모두 출력되지 않았다면? -> 즉시 전체 출력
            if (isTyping)
            {
                StopAllCoroutines(); // 타자 치는 효과 중단
                narrationText.text = currentEnding.narrationLines[currentLineIndex];
                isTyping = false;
            }
            // 이미 대사가 다 나왔다면? -> 다음 대사로
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        currentLineIndex++;

        // 아직 보여줄 대사가 남아있다면
        if (currentLineIndex < currentEnding.narrationLines.Length)
        {
            StartCoroutine(TypeWriterEffect(currentEnding.narrationLines[currentLineIndex]));
        }
        else
        {
            // 모든 대사가 끝남 -> 버튼 표시
            Debug.Log("모든 대사 종료. 버튼 활성화.");
            if (mainMenuButton != null)
                mainMenuButton.SetActive(true);
        }
    }

    // 한 글자씩 출력하는 타자기 효과
    IEnumerator TypeWriterEffect(string line)
    {
        isTyping = true;
        narrationText.text = ""; // 텍스트 비우고 시작

        foreach (char letter in line.ToCharArray())
        {
            narrationText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // [버튼 기능] 메인 메뉴 버튼을 눌렀을 때 실행될 함수
    // Inspector의 Button OnClick 이벤트에 이 함수를 연결하세요.
    public void GoToMainMenu()
    {
        Debug.Log("메인 메뉴 씬으로 이동합니다.");
        // 실제 메인 메뉴 씬 이름이 "MainMenu"라면 아래 주석 해제
        SceneManager.LoadScene("MainMenu"); 
    }
}
