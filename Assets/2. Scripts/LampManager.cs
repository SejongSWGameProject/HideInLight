using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class LampManager : MonoBehaviour
{
    public static LampManager Instance { get; private set; }

    [Header("전등 배열")]
    public List<LampController> lamps = new List<LampController>();

    [Header("플레이어")]
    public Transform player;

    [Header("크리쳐")]
    [SerializeField] private MonsterFollow monster;
    LampController targetLamp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Lights 배열 안전 체크
        if (lamps == null || lamps.Count == 0)
        {
            //Debug.Log("Lights 배열이 비어 있습니다!");
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

        targetLamp = SetMonsterTargetToNearestLight();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            targetLamp = SetMonsterTargetToNearestLight();
        }

    }

    public void RegisterLamp(LampController lamp)
    {
        if (!lamps.Contains(lamp))
        {
            lamps.Add(lamp);
        }
    }
    public void BreakLamp(LampController lamp)
    {
        if (System.Object.ReferenceEquals(lamp, targetLamp))
        {
            return;
        }

        if (lamp != null)
        {
            lamp.BreakLamp();
            lamps.Remove(lamp);
        }
            
    }
    public void BreakLamp()
    {
        if (targetLamp != null)
        {
            Debug.Log("램프매니저 BreakLamp()");
            targetLamp.BreakLamp();
            lamps.Remove(targetLamp);

            targetLamp = SetMonsterTargetToNearestLight();
        }
    }
    public LampController SetMonsterTargetToNearestLight()
    {
        //남은 lamp가 하나도 없을 때 예외처리 해줘야함
        if (monster == null || player == null || lamps == null || lamps.Count == 0)
            return null;

        LampController nearestLight = GetNearestLightToPlayer();
        if (nearestLight != null)
            monster.setTarget(nearestLight.transform);

        return nearestLight;
    }

    LampController GetNearestLightToPlayer()
    {
        if (lamps == null || lamps.Count == 0 || player == null)
            return null;

        float minDistance = float.MaxValue;
        LampController nearest = lamps[0];

        foreach (LampController l in lamps)
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