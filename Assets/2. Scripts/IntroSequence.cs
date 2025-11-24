using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [Header("조명/전구/텍스트 연결")]
    public Light spotLight;         
    public MeshRenderer lampRenderer; 
    public GameObject titleGroup;   

    [Header("이동할 게임 씬 이름")]
    public string gameSceneName = "GameScene";

    [Header("연출 시간 설정")]
    [Tooltip("전등이 깜빡거리는 총 시간")]
    public float flickerDuration = 2.5f;

    [Tooltip("깜빡이는 속도 (최소~최대 랜덤)")]
    public float minFlickerSpeed = 0.1f; 
    public float maxFlickerSpeed = 0.5f; 

    [Tooltip("전등이 꺼지고 게임 넘어가기 전 암전 시간 (초)")]
    public float blackoutDuration = 2.0f; // 기본값 2초로 설정해둠

    private Color originalGlowColor;

    void Start()
    {
        if (lampRenderer != null)
        {
            originalGlowColor = lampRenderer.material.GetColor("_EmissiveColor");
        }
        
        SetLightState(true); 
        titleGroup.SetActive(true); 
    }

    public void StartGameSequence()
    {
        StartCoroutine(PlaySequenceAndLoad());
    }

    IEnumerator PlaySequenceAndLoad()
    {
        // 1. 공포스럽게 깜빡거림 (설정된 시간만큼 진행)
        float totalTimer = 0f;
        while (totalTimer < flickerDuration)
        {
            bool isLightOn = !spotLight.enabled;
            SetLightState(isLightOn);

            float waitTime = Random.Range(minFlickerSpeed, maxFlickerSpeed); 
            yield return new WaitForSeconds(waitTime);
            totalTimer += waitTime;
        }

        // 2. 퍽! 하고 완전히 꺼짐
        SetLightState(false);
        
        // 3. 설정한 시간(2초)만큼 어둠 속에서 대기
        // 여기가 님이 원하시던 부분입니다!
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