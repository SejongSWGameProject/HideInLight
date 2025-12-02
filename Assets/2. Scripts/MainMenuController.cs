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

    [Header("깜빡임 설정")]
    public float minDelay = 0.05f;     
    public float maxDelay = 0.5f;      

    private bool isGameStarted = false; 
    private Color originalEmissionColor; 

    void Start()
    {
        if (blackScreenPanel != null) 
            blackScreenPanel.SetActive(false);

        if (lampRenderer != null)
            originalEmissionColor = lampRenderer.material.GetColor("_EmissiveColor");

        StartCoroutine(AutoFlickerRoutine());
    }

    // ★★★ [수정됨] 타타ㅏ타닥(X) -> 타닥! 타닥!(O) ★★★
    IEnumerator AutoFlickerRoutine()
    {
        bool preventRapid = false; 

        while (!isGameStarted)
        {
            // 1. 켜짐 상태 (평소)
            SetFlickerState(true);
            
            // 평소에는 여유 있게 켜져 있음 (0.8초 ~ 3.0초)
            float onDuration = Random.value > 0.3f ? Random.Range(0.8f, 3.0f) : Random.Range(0.5f, 0.8f);
            yield return new WaitForSeconds(onDuration);

            if (isGameStarted) break;

            // 2. 꺼짐 상태 (평소)
            SetFlickerState(false);

            // 평소에 꺼질 때도 여유 있게 (0.2초 ~ 0.5초)
            float offDuration = Random.Range(0.2f, 0.5f); 
            yield return new WaitForSeconds(offDuration);
            
            // 3. 지직거림 (타닥! 타닥!)
            if (!preventRapid && Random.value < 0.12f) 
            {
                for (int i = 0; i < 2; i++) 
                {
                    SetFlickerState(true);
                    // [수정 포인트] 0.03초(너무 빠름) -> 0.08초~0.15초 (딱 적당히 빠름)
                    yield return new WaitForSeconds(Random.Range(0.08f, 0.15f));
                    
                    SetFlickerState(false);
                    // [수정 포인트] 여기도 똑같이 늘려줌
                    yield return new WaitForSeconds(Random.Range(0.08f, 0.15f));
                }
                preventRapid = true; 
            }
            else
            {
                preventRapid = false; 
            }
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