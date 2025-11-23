using System.Collections;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [Header("조명/전구/텍스트 연결")]
    public Light spotLight;         // 빛을 쏘는 조명
    public MeshRenderer lampRenderer; // 형광등 모델 (빛나는 껍데기)
    public GameObject titleGroup;   // 제목 묶음

    private Color originalGlowColor; // 원래 형광등의 밝은 색을 저장할 변수

    void Start()
    {
        // 1. 시작할 때 형광등의 원래 불빛 색깔(흰색)을 기억해둠
        if (lampRenderer != null)
        {
            originalGlowColor = lampRenderer.material.GetColor("_EmissiveColor");
        }

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 초기화: 제목 끄기, 불 켜기
        titleGroup.SetActive(false);
        SetLightState(true); 

        // 깜빡임 연출 (2.5초)
        float timer = 0f;
        while (timer < 2.5f)
        {
            // 현재 불이 켜져있으면 끄고, 꺼져있으면 켬 (반전)
            bool isLightOn = !spotLight.enabled;
            SetLightState(isLightOn);

            float waitTime = Random.Range(0.05f, 0.2f); // 더 빠르게 깜빡임
            yield return new WaitForSeconds(waitTime);
            timer += waitTime;
        }

        // 퍽! 하고 깨짐 (완전 암전)
        SetLightState(false);
        yield return new WaitForSeconds(1.5f);

        // 복구 및 제목 등장
        SetLightState(true);
        titleGroup.SetActive(true);
    }

    // 빛과 전구 색깔을 동시에 껐다 켜는 함수
    void SetLightState(bool isOn)
    {
        // 1. 조명 끄기/켜기
        spotLight.enabled = isOn;

        // 2. 형광등 전구 색깔 바꾸기 (켜지면 원래색, 꺼지면 검은색)
        if (lampRenderer != null)
        {
            Color targetColor = isOn ? originalGlowColor : Color.black;
            lampRenderer.material.SetColor("_EmissiveColor", targetColor);
        }
    }
}