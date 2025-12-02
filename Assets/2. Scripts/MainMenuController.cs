using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("기본 연출 설정")]
    public Light horrorLight;          
    public GameObject blackScreenPanel; 
    public AudioSource sfxPlayer;      
    public AudioClip breakSound;       

    [Header("추가 깜빡임 대상")]
    public GameObject gameLogo;        
    public MeshRenderer lampRenderer;  

    [Header("발작(이벤트) 설정")]
    [Tooltip("이벤트 사이의 최소 대기 시간 (초)")]
    public float intervalMin = 4.0f;     
    [Tooltip("이벤트 사이의 최대 대기 시간 (초)")]
    public float intervalMax = 6.0f;      

    private bool isGameStarted = false; 
    private Color originalEmissionColor; 

    void Start()
    {
        if (blackScreenPanel != null) 
            blackScreenPanel.SetActive(false);

        if (lampRenderer != null)
            originalEmissionColor = lampRenderer.material.GetColor("_EmissiveColor");

        StartCoroutine(EventFlickerRoutine());
    }

    // ★★★ [수정됨] 5초마다 한 번씩 발작하는 코루틴 ★★★
    IEnumerator EventFlickerRoutine()
    {
        while (!isGameStarted)
        {
            // 1. 평소 상태: 불이 안정적으로 켜져 있음
            SetFlickerState(true);

            // 5초 정도 대기 (4초 ~ 6초 사이 랜덤)
            float waitTime = Random.Range(intervalMin, intervalMax);
            yield return new WaitForSeconds(waitTime);

            if (isGameStarted) break;

            // 2. 발작 이벤트 발생! (지직거림)
            // 영상처럼 불규칙하게 몇 번 따닥! 거립니다.
            int flickerCount = Random.Range(3, 6); // 3~5번 정도 반복
            
            for (int i = 0; i < flickerCount; i++)
            {
                // 꺼짐 (아주 짧게)
                SetFlickerState(false);
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

                // 켜짐 (아주 짧게)
                SetFlickerState(true);
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
            
            // 발작이 끝나면 다시 루프 처음으로 돌아가서 5초 대기
        }
    }

    public void OnStartButtonPressed()
    {
        if (isGameStarted) return; 
        isGameStarted = true;      

        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        // 시작하면 즉시 끔
        SetFlickerState(false); 

        if (sfxPlayer != null && breakSound != null)
        {
            sfxPlayer.PlayOneShot(breakSound);
        }

        if (blackScreenPanel != null) 
            blackScreenPanel.SetActive(true);

        yield return new WaitForSeconds(2.0f); 
        SceneManager.LoadScene("FirstStage"); 
    }

    void SetFlickerState(bool isOn)
    {
        if (horrorLight != null) horrorLight.enabled = isOn;
        if (gameLogo != null) gameLogo.SetActive(isOn);

        if (lampRenderer != null)
        {
            Color targetColor = isOn ? originalEmissionColor : Color.black;
            lampRenderer.material.SetColor("_EmissiveColor", targetColor);
        }
    }
}