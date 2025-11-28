using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerMind : MonoBehaviour
{

    [Header("UI")]
    public RectTransform uiObjectA;             // 줄어드는 UI 오브젝트

    [Header("감소 속도")]
    public float IncreaseSpeed = 20f;           // 외부 라이트에 비출 때
    public float autoDecreaseSpeed = 5f;        // 어둠에 있을 시 감소
    private float initialSizeY;           // 시작 정신력 최대수치

    [Header("설정")]
    public float checkInterval = 0.2f;

    private List<Light> sceneLights = new List<Light>();
    private bool isHitByLight = false;

    private void Start()
    {
        initialSizeY = uiObjectA.sizeDelta.y; // 시작 길이 저장

        // 씬의 모든 Light 자동 검색 (외부 라이트)
        sceneLights.AddRange(FindObjectsOfType<Light>());

    }

    void CheckLightss()
    {
        isHitByLight = false;

        foreach (var light in sceneLights)
        {
            // Light가 꺼져 있으면 스킵
            if (!light.enabled || light.intensity <= 0f)
                continue;

            Vector3 dir = transform.position - light.transform.position;

            if (Physics.Raycast(light.transform.position, dir, out RaycastHit hit))
            {
                if (hit.transform == transform) // 장애물 없이 플레이어를 봄
                {
                    isHitByLight = true;
                    return;  // 하나라도 비추면 true
                }
            }
        }
    }

    void Update()
    {
        if (uiObjectA == null) return;

        CheckLightss();

        Vector2 size = uiObjectA.sizeDelta;
        Vector3 pos = uiObjectA.localPosition;


        // 빛을 받을 때 증가
        if (isHitByLight)
        {
            float c = size.y;
            size.y += IncreaseSpeed * Time.deltaTime;
            if (size.y > initialSizeY)
                size.y = initialSizeY;
            float d = size.y - c;
            pos.y += d / 2f;
        }

        // 어둠에 있을 시 지속 감소
        else
        {
            float a = size.y;
            size.y -= autoDecreaseSpeed * Time.deltaTime;
            if (size.y < 0f)
                size.y = 0;
            float b = a - size.y;
            pos.y -= b / 2f;
        }


        uiObjectA.sizeDelta = size;
        uiObjectA.localPosition = pos;
    }
}