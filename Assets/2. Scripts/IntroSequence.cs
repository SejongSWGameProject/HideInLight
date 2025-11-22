using UnityEngine;
using System.Collections;
using UnityEngine.UI; // UI를 다루기 위해 추가

public class IntroSequence : MonoBehaviour
{
    [Header("3D 연출 오브젝트")]
    public Light spotLight;          // 아까 만든 Spot Light
    public GameObject title3DText;   // "Hide In Light" 3D 텍스트 (지금 화면에 있는거)
    
    [Header("기존 2D UI 연결")]
    public GameObject mainButtonsGroup; // Start, Option, Exit 버튼들을 묶어놓은 부모 오브젝트 (또는 Canvas)
    
    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip lightFlickerSound; // 치직 거리는 소리 (없으면 비워도 됨)
    public AudioClip bulbSmashSound;    // 쨍그랑 소리

    void Start()
    {
        // 1. 게임 시작하자마자 UI랑 타이틀은 안 보이게 숨김
        if(title3DText != null) title3DText.SetActive(false);
        if(mainButtonsGroup != null) mainButtonsGroup.SetActive(false);

        // 2. 연출 시작
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // [연출 1] 전등 깜빡이기 (치직... 치직...)
        float timer = 0f;
        while (timer < 2.0f) // 2초 동안
        {
            spotLight.enabled = !spotLight.enabled; // 껐다 켰다
            if(spotLight.enabled && lightFlickerSound != null) audioSource.PlayOneShot(lightFlickerSound);
            
            float randomTime = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(randomTime);
            timer += randomTime;
        }

        // [연출 2] 마지막에 잠깐 켜져서 긴장감 조성
        spotLight.enabled = true;
        yield return new WaitForSeconds(0.8f);

        // [연출 3] 쨍그랑! (암전)
        spotLight.enabled = false; 
        if(bulbSmashSound != null) audioSource.PlayOneShot(bulbSmashSound);

        yield return new WaitForSeconds(1.5f); // 어둠 속에서 대기

        // [연출 4] 타이틀 등장 (HIDE IN LIGHT)
        if(title3DText != null) title3DText.SetActive(true);
        
        yield return new WaitForSeconds(1.5f); // 타이틀 읽을 시간 줌

        // [연출 5] 드디어 기존 2D 버튼들 등장!
        if(mainButtonsGroup != null) mainButtonsGroup.SetActive(true);
    }
}