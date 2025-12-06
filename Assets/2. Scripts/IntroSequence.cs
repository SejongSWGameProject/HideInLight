using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [Header("오브젝트 연결")]
    public Light spotLight;           // 메인 조명
    public Light areaLight;           // 보조 조명
    public MeshRenderer lampRenderer; // 형광등 모델
    public GameObject titleGroup;     // 제목과 버튼 UI 그룹
    
    // 깜빡임을 담당하던 MainMenuController를 끄기 위해 연결
    [Tooltip("깜빡임을 담당하는 MainMenuController가 붙은 오브젝트")]
    public MainMenuController mainMenuController; 

    [Header("이동할 게임 씬 이름")]
    public string gameSceneName = "FirstScene";

    [Header("시간 설정")]
    public float blackoutDuration = 2.0f; // 깨진 후 암전 유지 시간

    [Header("효과음")]
    public AudioSource audioSource;
    public AudioClip breakSound;       

    private bool isSequenceStarted = false;

    // ★ 버튼(Start)에 연결할 함수
    public void StartGameSequence()
    {
        if (isSequenceStarted) return; // 중복 클릭 방지
        isSequenceStarted = true;

        // 1. 깜빡임을 담당하던 MainMenuController 끄기 (더 이상 깜빡이면 안 되니까)
        if (mainMenuController != null)
        {
            mainMenuController.StopAllCoroutines(); // 깜빡임 코루틴 중지
            mainMenuController.enabled = false;     // 스크립트 비활성화
        }

        // 2. 파괴 연출 시작
        StartCoroutine(BreakAndLoadRoutine());
    }

    // 쨍그랑! 하고 암전 후 이동하는 함수
    IEnumerator BreakAndLoadRoutine()
    {
        // 1. 즉시 모든 불 끄기 (깨짐 연출)
        TurnOffLights();             
        
        if (titleGroup != null) 
            titleGroup.SetActive(false); // UI 숨기기

        // 2. 깨지는 소리 재생
        if (audioSource != null && breakSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(breakSound);
        }
        
        // 3. 암흑 속에서 대기 (여운 남기기)
        yield return new WaitForSeconds(blackoutDuration);

        // 4. 게임 씬으로 이동
        SceneManager.LoadScene(gameSceneName);
    }

    // 조명을 끄고 전구 색을 검은색으로 바꾸는 함수
    void TurnOffLights()
    {
        // 조명 컴포넌트 끄기
        if(spotLight != null) spotLight.enabled = false;
        if(areaLight != null) areaLight.enabled = false;

        // 전구 모델의 빛나는 재질(Emission)을 검은색으로 변경
        if (lampRenderer != null)
        {
            lampRenderer.material.SetColor("_EmissiveColor", Color.black);
        }
    }
}