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
    public List<LampController> allLamps = new List<LampController>();

    [Header("플레이어")]
    public Transform player;

    [Header("크리쳐")]
    [SerializeField] private MonsterAI monster;
    LampController targetLamp;

    [Header("전등 스위치 배열")]
    public List<LampSwitch> switches = new List<LampSwitch>();
    private LampSwitch nearSwitch;

    [Header("총 전력 UI")]
    public RectTransform elecPowerUI;             // 줄어드는 UI 오브젝트
    private float initHeight;
    private float initPower = 1000f;
    private float curPower = 1000f;
    private float decreaseSpeed = 1f;
    private int activeLampCount;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // Lights 배열 안전 체크
        if (lamps == null || lamps.Count == 0)
        {
            Debug.Log("Lights 배열이 비어 있습니다!");
        }

        if (switches == null || switches.Count == 0)
        {
            Debug.Log("LightSwitch 배열이 비어 있습니다!");
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
            monster = GameObject.FindAnyObjectByType<MonsterAI>();
            if (monster == null)
                Debug.Log("MonsterFollow 객체를 찾을 수 없습니다!");
        }

    }

    void Start()
    {
        initHeight = elecPowerUI.sizeDelta.y; // 시작 길이 저장
        Debug.Log(initHeight);
        if (monster == null) return;

        targetLamp = SetMonsterTargetToRandomLamp();
        Debug.Log("켜져있는 전등 개수: "+lamps.Count);
        UpdateActiveLampCount();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    targetLamp = SetMonsterTargetToRandomLamp();
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    targetLamp = SetMonsterTargetToRandomLamp();
        //}
        if (lamps.Count == 0)
        {
            Debug.Log("모든 전등 깨짐");
            monster.setTarget(player.transform);
        }
        
        if(elecPowerUI.gameObject.activeInHierarchy)
        {
            curPower -= decreaseSpeed * Time.deltaTime;
            if (curPower <= 0) {
                curPower = 0;
                DisableAllLamps();
            }
            UpdateElectricPowerUI();
            //Debug.Log(curPower);
        }

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    curPower -= 100f;
        //}
    }

    public void UpdateActiveLampCount()
    {
        activeLampCount = lamps.Count;
        decreaseSpeed = 1f + (activeLampCount / 75f);
        Debug.Log("activeLampCount:" + activeLampCount);
    }

    public void DisableAllLamps()
    {
        foreach(LampController l in allLamps)
        {
            if (l != null)
            {
                l.TurnOff();
                l.isBroken = true;
            }
        }
        allLamps.Clear();
        lamps.Clear();
    }

    public void UpdateElectricPowerUI()
    {
        Vector2 size = elecPowerUI.sizeDelta;
        Vector3 pos = elecPowerUI.localPosition;

        float originY = size.y;
        size.y = initHeight * (curPower / initPower);
        
        pos.y += (size.y - originY) / 2f;


        elecPowerUI.sizeDelta = size;
        elecPowerUI.localPosition = pos;
    }

    public void RegisterLamp(LampController lamp)
    {
        if (!lamps.Contains(lamp))
        {
            lamps.Add(lamp);
        }
    }

    public void RegisterLamp(LampController lamp, bool insertArranged)
    {
        if (insertArranged)
        {
            if (!allLamps.Contains(lamp))
            {
                allLamps.Add(lamp);
            }
        }
        else
        {
            if (!lamps.Contains(lamp))
            {
                lamps.Add(lamp);
            }
        }
    }

    public void QuitLamp(LampController lamp)
    {
        if (lamps.Contains(lamp))
        {
            lamps.Remove(lamp);
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

            targetLamp = SetMonsterTargetToRandomLamp();
        }
    }

    public LampController SetMonsterTargetToRandomLamp()
    {
        //남은 lamp가 하나도 없을 때 예외처리 해줘야함
        if (monster == null || player == null || lamps == null || lamps.Count == 0)
            return null;

        LampController randomLamp = GetRandomLamp();
        if (randomLamp != null)
            monster.setTarget(randomLamp.transform);
        targetLamp = randomLamp;
        Debug.Log("랜덤 전등 타겟설정");
        return randomLamp;
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

    

    LampController GetRandomLamp()
    {
        if (lamps.Count > 0)
        {
            // 0부터 (리스트 크기 - 1) 사이의 랜덤한 정수 인덱스를 가져옵니다.
            int randomIndex = Random.Range(0, lamps.Count);

            // 해당 인덱스의 LampController를 가져옵니다.
            LampController randomLamp = lamps[randomIndex];

            return randomLamp;
        }
        else
        {
            Debug.LogWarning("램프 리스트가 비어있습니다!");
            return null;
        }

    }

    public void RegisterLampSwitch(LampSwitch ls)
    {
        if (!switches.Contains(ls))
        {
            switches.Add(ls);
        }
    }

    public LampSwitch GetNearSwitch()
    {
        if (switches == null || switches.Count == 0 || player == null)
            return null;

        float minDistance = float.MaxValue;
        LampSwitch nearest = switches[0];

        foreach (LampSwitch ls in switches)
        {
            float dist = Vector3.Distance(player.position, ls.transform.position);
            Debug.Log(ls.name + dist);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = ls;
            }
        }
        return nearest;
    }

    public void SortLampListByDistance()
    {
        if (player == null || allLamps == null || allLamps.Count == 0)
        {
            Debug.LogWarning("플레이어 Transform 또는 램프 리스트가 설정되지 않았습니다.");
            return;
        }

        Vector3 playerPos = player.position;

        // 람다식을 사용하여 리스트를 정렬합니다.
        // (a, b) => ... 부분은 두 요소 a와 b를 비교하는 로직입니다.
        allLamps.Sort((a, b) =>
        {
            // 1. 플레이어와 각 램프 사이의 벡터를 구합니다.
            Vector3 diffA = a.transform.position - playerPos;
            Vector3 diffB = b.transform.position - playerPos;

            // 2. Vector3.sqrMagnitude (제곱 거리)를 사용하여 비교합니다.
            //    제곱 거리는 Vector3.Distance()보다 훨씬 빠릅니다. (제곱근 연산 생략)
            float distSqA = diffA.sqrMagnitude;
            float distSqB = diffB.sqrMagnitude;

            // 3. CompareTo를 사용하여 오름차순 정렬합니다 (가까운 것이 먼저).
            //    a의 거리가 b의 거리보다 작으면(가까우면) 음수를 반환하여 a가 b보다 앞으로 옵니다.
            return distSqA.CompareTo(distSqB);
        });

        Debug.Log("LampController 리스트가 플레이어와의 거리를 기준으로 정렬되었습니다 (가까운 순).");
    }
}