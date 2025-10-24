using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class LightManager : MonoBehaviour
{
    [Header("전등 배열")]
    public Light[] lights;

    [Header("플레이어")]
    public Transform player;

    [Header("크리쳐")]
    [SerializeField] private MonsterFollow monster;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Lights 배열 안전 체크
        if (lights == null || lights.Length == 0)
        {
            Debug.LogError("Lights 배열이 비어 있습니다!");
        }

        // Player 안전 체크
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else Debug.LogError("Player Transform이 할당되지 않았고, 씬에 Player 태그 오브젝트도 없습니다!");
        }

        // Monster 안전 체크
        if (monster == null)
        {
            monster = GameObject.FindAnyObjectByType<MonsterFollow>();
            if (monster == null)
                Debug.LogError("MonsterFollow 객체를 찾을 수 없습니다!");
        }
    }

    void Start()
    {
        if (monster == null) return;

        SetMonsterTargetToNearestLight();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetMonsterTargetToNearestLight();
        }
    }

    void SetMonsterTargetToNearestLight()
    {
        if (monster == null || player == null || lights == null || lights.Length == 0)
            return;

        Light nearestLight = GetNearestLightToPlayer();
        if (nearestLight != null)
            monster.setTarget(nearestLight.transform);
    }

    Light GetNearestLightToPlayer()
    {
        if (lights == null || lights.Length == 0 || player == null)
            return null;

        float minDistance = float.MaxValue;
        Light nearest = lights[0];

        foreach (Light l in lights)
        {
            float dist = Vector3.Distance(player.position, l.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = l;
            }
        }
        return nearest;
    }
}