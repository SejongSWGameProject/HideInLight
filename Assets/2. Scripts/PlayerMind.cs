using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
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
    private PlayerLight playerLight;

    public GhostAI ghost;
    public LayerMask obstacleLayer;
    public GhostSpawner ghostSpawner;

    public float mindValue = 100.0f;

    private void Start()
    {
        initialSizeY = uiObjectA.sizeDelta.y; // 시작 길이 저장
        playerLight = GetComponentInChildren<PlayerLight>();

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
            
            adjustMind(-autoDecreaseSpeed);

        }

        SetUIByMind();

        //Debug.Log(mindValue);

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