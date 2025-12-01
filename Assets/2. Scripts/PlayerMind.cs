using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMind : MonoBehaviour
{

    [Header("UI")]
    public RectTransform uiObjectA;             // 줄어드는 UI 오브젝트

    [Header("감소 속도")]
    public float IncreaseSpeed = 20f;           // 외부 라이트에 비출 때
    public float autoDecreaseSpeed = 5f;        // 어둠에 있을 시 감소
    private float initialSizeY;           // 시작 정신력 최대수치

    private bool isInDarkness = false;
    public PlayerLight playerLight;

    public GhostAI ghost;
    public LayerMask obstacleLayer;
    public GhostSpawner ghostSpawner;

    public float mindValue = 100.0f;

    [Header("Volume & Status")]
    public Volume postProcessVolume;

    private Vignette vignette;
    private DepthOfField depthOfField;

    [Header("Effect Settings")]
    public float effectStartThreshold = 50f; // 효과 시작 기준값

    [Header("Vignette Settings")]
    public float maxVignetteIntensity = 0.5f; // 최대 비네팅 강도

    [Header("Blur Settings (Manual Mode)")]
    // 평소 상태: 멀리까지 잘 보임
    public float clearFarStart = 50f;
    public float clearFarEnd = 100f;

    // 위급 상태: 바로 앞(2m)부터 흐려지기 시작함
    public float blurryFarStart = 2f;
    public float blurryFarEnd = 5f;

    public float minFocusDistance = 0.5f;

    void Start()
    {
        initialSizeY = uiObjectA.sizeDelta.y; // 시작 길이 저장
        // 컴포넌트 가져오기
        // Volume에서 Vignette와 Depth of Field 컴포넌트 가져오기
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out depthOfField);

            if (vignette == null)
            {
                Debug.LogWarning("Vignette가 Volume Profile에 없습니다!");
            }

            if (depthOfField == null)
            {
                Debug.LogWarning("Depth of Field가 Volume Profile에 없습니다!");
            }

            // 초기 상태 설정 (효과 비활성화)
            if (vignette != null)
            {
                Debug.Log("비활성화");

                vignette.intensity.value = 0f;
            }

            if (depthOfField != null)
            {
                Debug.Log("비활성화");
                depthOfField.farMaxBlur = 0f;
            }
        }
        StartCoroutine(CheckDarknessRoutine());
    }

    

    void Update()
    {
        if (uiObjectA == null) return;

        // 빛을 받을 때 증가
        if (!isInDarkness)
        {
            adjustMind(IncreaseSpeed);
        }

        // 어둠에 있을 시 지속 감소
        else
        {
            if (playerLight != null)
            {
                if (!playerLight.flashlight.enabled)
                {
                    adjustMind(-autoDecreaseSpeed);

                }
            }
        }

        SetUIByMind();

        //Debug.Log(mindValue);

        SetSightEffect();
    }
    public void SetSightEffect()
    {
        if (mindValue < 50f)
        {

            // 1. 현재 진행률을 0(정상) ~ 1(완전 미침) 사이 값으로 변환
            // mindValue가 50이면 t = 0, mindValue가 0이면 t = 1
            float t = 1f - (mindValue / 50f);

            // 2. 제곱 적용 (Ease In 효과)
            // t가 0.1일 때 -> 0.01 (거의 변화 없음)
            // t가 0.5일 때 -> 0.25 (아직 절반 안 됨)
            // t가 0.9일 때 -> 0.81 (급격히 증가)
            float curveT = t * t;

            // 3. 0(기본값)에서 최대값으로 보간
            // 기존 코드와 달리 '시작값 -> 목표값' 순서로 적어 헷갈림 방지
            float dofLerp = Mathf.Lerp(0f, 16f, curveT); // 0에서 16으로
            float vigLerp = Mathf.Lerp(0f, 1f, curveT);  // 0에서 1로

            // Vignette 강도 조절
            if (vignette != null)
            {
                vignette.intensity.value = vigLerp;
            }

            // Depth of Field 강도 조절 (HDRP)
            if (depthOfField != null)
            {
                depthOfField.farMaxBlur = dofLerp;
            }

            // 디버그로 수치 변화 확인해보세요 (초반엔 수치가 아주 천천히 오를 겁니다)
            Debug.Log($"Mind: {mindValue} | Linear(t): {t:F2} | Curved(t^2): {curveT:F2}");
        }
    }
    public void SetUIByMind()
    {
        Vector2 size = uiObjectA.sizeDelta;
        Vector3 pos = uiObjectA.localPosition;

        float originY = size.y;
        size.y = initialSizeY * (mindValue / 100.0f);
        if (size.y < 0f)
            size.y = 0;
        if (size.y > initialSizeY)
            size.y = initialSizeY;
        pos.y += (size.y-originY) / 2f;
        

        uiObjectA.sizeDelta = size;
        uiObjectA.localPosition = pos;
    }

    private void adjustMind(float speed)
    {
        mindValue += speed * Time.deltaTime;
        if(mindValue >= 100)
        {
            mindValue = 100f;
        }
        if(mindValue <= 0)
        {
            mindValue = 0f;
        }
    }

    private void DecreaseMind(float speed)
    {
        //Vector2 size = uiObjectA.sizeDelta;
        //Vector3 pos = uiObjectA.localPosition;

        //float a = size.y;
        //size.y -= speed * Time.deltaTime;
        //if (size.y < 0f)
        //    size.y = 0;
        //float b = a - size.y;
        //pos.y -= b / 2f;

        //uiObjectA.sizeDelta = size;
        //uiObjectA.localPosition = pos;
    }

    public float GetPlayerMind()
    {
        return mindValue;
    }
    public void SetPlayerMind(float value)
    {
        if (value >= 100) value = 100f;
        if (value <= 0) value = 0f;
        mindValue = value;
    }
    public void IncreasePlayerMind(float d)
    {
        mindValue += d;
        if (mindValue >= 100) mindValue = 100f;
        if (mindValue <= 0) mindValue = 0f;
    }

    public bool getIsInDarkness()
    {
        return isInDarkness;
    }

    public void setIsInDarkness(bool isin)
    {
        isInDarkness = isin;
    }

    IEnumerator CheckDarknessRoutine()
    {
        // 최적화: 1초 대기 오브젝트를 미리 만들어둠 (메모리 절약)
        WaitForSeconds wait = new WaitForSeconds(1.0f);

        while (true)
        {
            // 함수 실행
            CheckDarkness();

            // 1초 대기 (이 줄에서 코드가 멈췄다가 1초 뒤 재개됨)
            yield return wait;
        }
    }

    public void CheckDarkness()
    {
        // 1. 반경 40m 내의 전등 레이어만 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 40f, LayerMask.GetMask("Lamp"));

        float distSum = 0.0f;

        // 2. 배열을 한 번만 돌면서 검사와 계산을 동시에 수행
        foreach (Collider c in hitColliders)
        {
            // 전등 컴포넌트 가져오기
            LampController l = c.GetComponent<LampController>();

            // 예외 처리: 컴포넌트가 없거나, 전등이 꺼져있으면 패스
            if (l == null || !l.lamp.enabled) continue;

            Vector3 direction = c.transform.position - transform.position;
            float distance = direction.magnitude; // 레이캐스트용 실제 거리

            // 3. 벽 체크 (Raycast)
            // 장애물에 막히지 않았을 때만 계산 (Raycast가 false여야 벽이 없는 것)
            if (!Physics.Raycast(transform.position, direction.normalized, distance, obstacleLayer))
            {
                // 벽이 없다면 빛 계산
                float distSquare = direction.sqrMagnitude; // 거리 제곱 (최적화)

                // 0으로 나누기 방지 (혹시 모를 에러 방지)
                if (distSquare > 0.001f)
                {
                    distSum += (1000.0f / distSquare);
                    // 디버깅이 필요하면 주석 해제
                    //Debug.Log($"{c.name} : {distSquare}");
                }
            }
        }

        //Debug.Log("distSum:" + distSum);

        // 4. 최종 판정
        isInDarkness = (distSum < 1.0f);

        if (!isInDarkness)
        {
            ghostSpawner.KillAllGhosts();
        }
    }
}