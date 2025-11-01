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

    // Start()는 한 번만 실행되지만, OnEnable()은 SetActive(true)가 될 때마다 실행됩니다.
    void OnEnable()
    {
        // 퍼즐을 켜기 전에, 이전에 있던 커넥터와 와이어를 모두 삭제합니다.
        ClearPuzzle();
        // 그리고 퍼즐을 새로 설치합니다.
        SetupPuzzle();
    }
    
    // (선택 사항) 비활성화될 때 미리 지우는 것도 좋습니다.
    void OnDisable()
    {
        ClearPuzzle();
    }
    
    /// <summary>
    /// 이전에 생성된 커넥터와 와이어를 모두 파괴하고 리스트를 비웁니다.
    /// </summary>
    void ClearPuzzle()
    {
        Debug.Log("--- PuzzleManager: ClearPuzzle (이전 퍼즐 삭제) ---");
        
        // 1. 왼쪽 커넥터 삭제
        foreach (Transform child in leftConnectorsParent)
        {
            Destroy(child.gameObject);
        }
        
        // 2. 오른쪽 커넥터 삭제
        foreach (Transform child in rightConnectorsParent)
        {
            Destroy(child.gameObject);
        }
        
        // 3. 와이어 삭제
        foreach (Transform child in wireContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 4. 리스트 비우기
        leftConnectors.Clear();
        
        // 5. 그리기 상태 초기화
        startConnector = null;
        if (currentDrawingWire != null)
        {
            Destroy(currentDrawingWire.gameObject);
            currentDrawingWire = null;
        }
    }


    void Update()
    {
        // ESC 키를 누르면 퍼즐 패널을 닫습니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        
        // --- 기존 Update 내용 ---
        if (startConnector != null && currentDrawingWire != null)
        {
            // 1. 마우스의 '픽셀 좌표(Input.mousePosition)'를 'UI 월드 좌표'로 변환합니다.
            Vector3 worldMousePosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                wireContainerRect,   // 좌표가 속할 기준 UI (와이어 컨테이너)
                Input.mousePosition, // 변환할 마우스 픽셀 위치
                (puzzleCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : puzzleCanvas.worldCamera, // 캔버스 모드에 맞는 카메라
                out worldMousePosition); // 변환된 월드 좌표

            // 2. SetProperties에는 변환된 '월드 좌표'를 넘겨줍니다.
            currentDrawingWire.SetProperties(
                startConnector.transform.position, // (월드 좌표 1)
                worldMousePosition,                // (월드 좌표 2 - 변환 완료)
                wireColors[startConnector.ConnectorId]);
        }
        // --- ▲▲▲ 여기까지 수정됨 ▲▲▲ ---
    }

    // ▼▼▼ 'X' 버튼 연결용 함수 (여기에 추가됨) ▼▼▼
    /// <summary>
    /// UI의 'X' 버튼을 클릭했을 때 호출될 함수
    /// </summary>
    public void ClosePuzzlePanel()
    {
        Debug.Log("'X' 버튼 클릭됨. 퍼즐 패널을 닫습니다.");
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
        else
        {
            // puzzlePanel이 연결 안됐을 경우, 이 스크립트가 붙은 게임오브젝트를 끔
            gameObject.SetActive(false);
        }
    }
    // ▲▲▲ 여기까지 추가됨 ▲▲▲


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
    
    public void DropWireOnConnector(Connector endConnector)
    {
        // 1. 빈 공간에 놓았는지 확인
        if (endConnector == null)
        {
            Debug.Log("<color=orange>연결 실패: 커넥터가 아닌 빈 공간에 놓았습니다.</color> (Wire 프리팹의 Raycast Target을 껐는지 확인하세요)");
            if(currentDrawingWire != null) Destroy(currentDrawingWire.gameObject); // 널 체크 추가
        }
        else // 2. 커넥터 위에 놓았을 때, 조건 확인
        {
            // ▼▼▼ 널 체크 추가: startConnector가 null일 수 있음 (OnDisable 등에서 초기화된 경우) ▼▼▼
            if (startConnector == null)
            {
                Debug.LogWarning("DropWireOnConnector: startConnector가 null입니다. (아마도 비활성화?)");
                if (currentDrawingWire != null) Destroy(currentDrawingWire.gameObject);
                currentDrawingWire = null;
                return;
            }
            // ▲▲▲ 널 체크 끝 ▲▲▲

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
                if(currentDrawingWire != null) Destroy(currentDrawingWire.gameObject); // 널 체크 추가
            }
        }

        // 그리기 상태 초기화
        startConnector = null;
        currentDrawingWire = null;
    }

    public bool IsDrawingWire() => startConnector != null;

    void CheckForCompletion()
    {
        // ▼▼▼ leftConnectors가 비어있지 않은지 확인 (ClearPuzzle 직후 호출 방지) ▼▼▼
        if (leftConnectors.Count > 0 && leftConnectors.All(conn => conn.IsConnected))
        {
            Debug.LogWarning("CheckForCompletion: leftConnectors 리스트가 비어있습니다.");
            return;
        }

        // 모든 leftConnectors가 IsConnected 상태인지 확인
        bool allConnected = leftConnectors.All(conn => conn.IsConnected);
        
        if (allConnected)
        {
            Debug.Log("<color=cyan>===== 퍼즐 성공! =====</color>");
            // (참고) 0.5초 뒤에 패널을 끄고 싶다면 Invoke("DelayClosePanel", 0.5f);
            ClosePuzzlePanel(); 
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}