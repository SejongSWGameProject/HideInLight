using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    [Header("연결할 프리팹")]
    public GameObject connectorPrefab;
    public GameObject wirePrefab;

    [Header("UI 부모 오브젝트")]
    public Transform leftConnectorsParent;
    public Transform rightConnectorsParent;
    public Transform wireContainer;
    
    [Header("UI 오브젝트")]
    public GameObject puzzlePanel; // 퍼즐 성공 시 닫을 패널

    [Header("퍼즐 설정")]
    public int connectorCount = 4;
    public Color[] wireColors;

    private List<Connector> leftConnectors = new List<Connector>();
    private Wire currentDrawingWire;
    private Connector startConnector;

    void Start()
    {
        SetupPuzzle();
    }

    void Update()
    {
        if (startConnector != null && currentDrawingWire != null)
        {
            currentDrawingWire.SetProperties(startConnector.transform.position, Input.mousePosition, wireColors[startConnector.ConnectorId]);
        }
    }

    void SetupPuzzle()
    {
        Debug.Log("--- PuzzleManager: SetupPuzzle 시작 ---");
        List<int> rightSideIds = Enumerable.Range(0, connectorCount).ToList();
        Shuffle(rightSideIds);
        Debug.Log("오른쪽 커넥터 ID 순서 (랜덤): " + string.Join(", ", rightSideIds));

        for (int i = 0; i < connectorCount; i++)
        {
            // 왼쪽 커넥터 생성
            GameObject leftObj = Instantiate(connectorPrefab, leftConnectorsParent);
            Connector leftConn = leftObj.GetComponent<Connector>();
            leftConn.Initialize(this, i, true);
            leftObj.GetComponent<Image>().color = wireColors[i];
            leftConnectors.Add(leftConn);

            // 오른쪽 커넥터 생성
            GameObject rightObj = Instantiate(connectorPrefab, rightConnectorsParent);
            Connector rightConn = rightObj.GetComponent<Connector>();
            int randomId = rightSideIds[i];
            rightConn.Initialize(this, randomId, false);
            rightObj.GetComponent<Image>().color = wireColors[randomId];
        }
        Debug.Log("--- PuzzleManager: " + connectorCount + "쌍의 커넥터 생성 완료 ---");
    }

    public void StartDrawingWire(Connector startConn)
    {
        Debug.Log($"StartDrawingWire: ID {startConn.ConnectorId}번 커넥터에서 그리기 시작.");
        startConnector = startConn;
        GameObject wireObj = Instantiate(wirePrefab, wireContainer);
        currentDrawingWire = wireObj.GetComponent<Wire>();
    }

    // ▼▼▼ 여기가 핵심 디버깅 부분입니다 ▼▼▼
    public void DropWireOnConnector(Connector endConnector)
    {
        // 1. 빈 공간에 놓았는지 확인
        if (endConnector == null)
        {
            Debug.Log("<color=orange>연결 실패: 커넥터가 아닌 빈 공간에 놓았습니다.</color> (Wire 프리팹의 Raycast Target을 껐는지 확인하세요)");
            Destroy(currentDrawingWire.gameObject);
        }
        else // 2. 커넥터 위에 놓았을 때, 조건 확인
        {
            Debug.Log($"연결 시도: 시작 ID({startConnector.ConnectorId}) -> 도착 ID({endConnector.ConnectorId})");

            // 성공 조건 체크
            bool isRightSide = !endConnector.IsLeft;
            bool isNotConnected = !endConnector.IsConnected;
            bool isIdMatched = startConnector.ConnectorId == endConnector.ConnectorId;

            if (isRightSide && isNotConnected && isIdMatched)
            {
                Debug.Log($"<color=green>연결 성공!</color> ID: {startConnector.ConnectorId}");
                startConnector.IsConnected = true;
                endConnector.IsConnected = true;
                currentDrawingWire.SetProperties(startConnector.transform.position, endConnector.transform.position, wireColors[startConnector.ConnectorId]);
                CheckForCompletion();
            }
            else
            {
                // 실패 원인을 하나씩 로그로 출력
                Debug.Log("<color=red>연결 실패 원인:</color>");
                if (!isRightSide) Debug.Log("- 도착점이 오른쪽 커넥터가 아닙니다.");
                if (!isNotConnected) Debug.Log("- 도착점이 이미 다른 선과 연결되어 있습니다.");
                if (!isIdMatched) Debug.Log("- 시작점과 도착점의 ID(색상)가 다릅니다.");
                Destroy(currentDrawingWire.gameObject);
            }
        }

        // 그리기 상태 초기화
        startConnector = null;
        currentDrawingWire = null;
    }

    public bool IsDrawingWire() => startConnector != null;

    void CheckForCompletion()
    {
        if (leftConnectors.All(conn => conn.IsConnected))
        {
            Debug.Log("<color=cyan>---!!! 퍼즐 클리어 !!!---</color>");
            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(false);
            }
        }
    }
    
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) { n--; int k = rng.Next(n + 1); T value = list[k]; list[k] = list[n]; list[n] = value; }
    }
}