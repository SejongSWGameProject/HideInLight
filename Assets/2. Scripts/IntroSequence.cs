using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [Header("오브젝트 연결")]
    public Light spotLight;           // 메인 조명 (스포트라이트)
    public Light areaLight;           // [추가됨] 은은하게 깔아둔 Area Light (혹은 Point Light)
    public MeshRenderer lampRenderer; // 형광등 모델
    public GameObject titleGroup;     // 제목과 버튼

    [Header("이동할 게임 씬 이름")]
    public string gameSceneName = "FirstScene";

    [Header("시간 설정")]
    public float flickerDuration = 2.0f;
    public float blackoutDuration = 3.0f; 

    [Header("효과음 (선택)")]
    public AudioSource audioSource;
    public AudioClip breakSound;      

    private Color originalGlowColor;

    void Start()
    {
        if (lampRenderer != null) 
            originalGlowColor = lampRenderer.material.GetColor("_EmissiveColor");
        
        // 시작할 때 다 켜두기
        SetLightState(true); 
        if(areaLight != null) areaLight.enabled = true; // Area Light도 켜기
        titleGroup.SetActive(true); 
    }

    public void StartGameSequence()
    {
        StartCoroutine(PlaySimpleSequence());
    }

    IEnumerator PlaySimpleSequence()
    {
        // 1. 깜빡거림 (Spot Light만 깜빡임)
        float totalTimer = 0f;
        while (totalTimer < flickerDuration)
        {
            bool isLightOn = !spotLight.enabled;
            SetLightState(isLightOn);
            
            // (선택사항) Area Light도 같이 깜빡이게 하려면 아래 주석 해제
            // if(areaLight != null) areaLight.enabled = isLightOn;

            float waitTime = Random.Range(0.05f, 0.2f); 
            yield return new WaitForSeconds(waitTime);
            totalTimer += waitTime;
        }

        // 2. 퍽! 하고 깨짐 (모든 빛 소멸)
        SetLightState(false);         // Spot Light 끄고
        if(areaLight != null) areaLight.enabled = false; // [핵심] Area Light도 끄기!
        titleGroup.SetActive(false);  // 글자도 끄기

        if (audioSource != null && breakSound != null)
        {
            audioSource.PlayOneShot(breakSound);
        }
        
        // 3. 완전한 암흑 상태로 대기
        yield return new WaitForSeconds(blackoutDuration);

        // 4. 게임 씬으로 이동
        SceneManager.LoadScene(gameSceneName);
    }

    void SetLightState(bool isOn)
    {
        spotLight.enabled = isOn;
        if (lampRenderer != null)
        {
            Color targetColor = isOn ? originalGlowColor : Color.black;
            lampRenderer.material.SetColor("_EmissiveColor", targetColor);
        }
    }
}