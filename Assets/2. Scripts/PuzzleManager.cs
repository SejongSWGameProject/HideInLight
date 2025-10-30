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

    // ▼▼▼ 이 퍼즐의 고유 ID (PlayerPrefs 키로 사용) ▼▼▼
    // 만약 나중에 다른 변압기 퍼즐을 또 만든다면, "TransformerPuzzle_02" 처럼 바꿔줘야 합니다.
    private string puzzleSaveKey = "IsTransformerPuzzleSolved_01";


    private List<Connector> leftConnectors = new List<Connector>();
    private Wire currentDrawingWire;
    private Connector startConnector;

    private Camera uiCamera; 

    void OnEnable()
    {
        // ▼▼▼ 1. "이미 풀었는지" 확인 (가장 먼저 실행) ▼▼▼
        // PlayerPrefs에서 이 퍼즐의 저장된 값을 가져옴 (없으면 기본값 0)
        if (PlayerPrefs.GetInt(puzzleSaveKey, 0) == 1)
        {
            Debug.Log($"이 퍼즐({puzzleSaveKey})은 이미 해결되었습니다. 패널을 열지 않습니다.");
            
            // 즉시 다시 닫아버림
            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
            return; // OnEnable의 나머지(ClearPuzzle, SetupPuzzle)를 실행하지 않음
        }
        // ▲▲▲ 1. 확인 끝 ▲▲▲


        // ▼▼▼ 캔버스 렌더 모드 확인 로직 (기존) ▼▼▼
        Canvas canvas = null;
        if (puzzlePanel != null)
        {
            canvas = puzzlePanel.GetComponentInParent<Canvas>();
        }
        else
        {
            canvas = GetComponentInParent<Canvas>(); 
        }

        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = null; 
            }
            else
            {
                uiCamera = canvas.worldCamera; 
                if (uiCamera == null) 
                {
                    uiCamera = Camera.main;
                    Debug.LogWarning("PuzzleManager: 캔버스에 worldCamera가 설정되지 않아 Camera.main을 사용합니다.");
                }
            }
        }
        else
        {
            Debug.LogError("PuzzleManager: 캔버스를 찾을 수 없습니다! UI가 정상 작동하지 않을 수 있습니다.");
            uiCamera = null; 
        }
        // ▲▲▲ 캔버스 로직 끝 ▲▲▲

        // 퍼즐을 켜기 전에, 이전에 있던 커넥터와 와이어를 모두 삭제합니다.
        ClearPuzzle();
        // 그리고 퍼즐을 새로 설치합니다.
        SetupPuzzle();
    }
    
    void OnDisable()
    {
        ClearPuzzle();
    }
    
    void ClearPuzzle()
    {
        // ... (이하 ClearPuzzle 내용은 동일) ...
        Debug.Log("--- PuzzleManager: ClearPuzzle (이전 퍼즐 삭제) ---");
        
        foreach (Transform child in leftConnectorsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in rightConnectorsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in wireContainer)
        {
            Destroy(child.gameObject);
        }
        
        leftConnectors.Clear();
        
        startConnector = null;
        if (currentDrawingWire != null)
        {
            Destroy(currentDrawingWire.gameObject);
            currentDrawingWire = null;
        }
    }


    void Update()
    {
        // ... (Update 내용은 동일) ...
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ESC 키는 퍼즐을 풀지 못했을 때만 작동해야 하므로
            // OnEnable에서 이미 클리어 여부를 체크해서 닫아주기 때문에
            // 여기서는 특별히 수정할 필요가 없습니다.
            // (만약의 경우를 대비해 ClosePuzzlePanel()을 호출하는 것도 좋음)
            ClosePuzzlePanel(false); // '풀지 않고' 닫기
        }
        
        if (startConnector != null && currentDrawingWire != null)
        {
            Vector3 startPos = startConnector.transform.position;
            Vector3 endPos; 

            if (uiCamera == null) 
            {
                endPos = Input.mousePosition;
            }
            else 
            {
                Vector3 screenMousePos = Input.mousePosition;
                Vector3 startScreenPos = uiCamera.WorldToScreenPoint(startPos);
                screenMousePos.z = startScreenPos.z;
                endPos = uiCamera.ScreenToWorldPoint(screenMousePos);
            }
            
            currentDrawingWire.SetProperties(startPos, endPos, wireColors[startConnector.ConnectorId]);
        }
    }

    /// <summary>
    /// UI의 'X' 버튼을 클릭했을 때 호출될 함수
    /// </summary>
    public void ClosePuzzlePanel()
    {
        // 'X' 버튼을 누른 것은 퍼즐을 풀지 않은 것이므로 false 전달
        ClosePuzzlePanel(false);
    }

    /// <summary>
    /// 퍼즐 패널을 닫는 내부 함수
    /// </summary>
    /// <param name="isCompleted">퍼즐을 '성공해서' 닫는 것인지 여부</param>
    private void ClosePuzzlePanel(bool isCompleted)
    {
        if (isCompleted)
        {
             Debug.Log("<color=cyan>---!!! 퍼즐 클리어 !!!---</color>");
             
             // ▼▼▼ 2. 퍼즐 해결 상태를 영구 저장 (추가) ▼▼▼
             PlayerPrefs.SetInt(puzzleSaveKey, 1);
             PlayerPrefs.Save(); // 확실하게 지금 저장
             // ▲▲▲ 2. 저장 끝 ▲▲▲
        }
        else
        {
            Debug.Log("퍼즐을 풀지 않고 닫습니다 (X 버튼 또는 ESC).");
        }

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    void SetupPuzzle()
    {
        // ... (SetupPuzzle 내용은 동일) ...
        Debug.Log("--- PuzzleManager: SetupPuzzle 시작 ---");
        List<int> rightSideIds = Enumerable.Range(0, connectorCount).ToList();
        Shuffle(rightSideIds);
        Debug.Log("오른쪽 커넥터 ID 순서 (랜덤): " + string.Join(", ", rightSideIds));

        for (int i = 0; i < connectorCount; i++)
        {
            GameObject leftObj = Instantiate(connectorPrefab, leftConnectorsParent);
            Connector leftConn = leftObj.GetComponent<Connector>();
            leftConn.Initialize(this, i, true);
            leftObj.GetComponent<Image>().color = wireColors[i];
            leftConnectors.Add(leftConn);

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
        // ... (StartDrawingWire 내용은 동일) ...
        Debug.Log($"StartDrawingWire: ID {startConn.ConnectorId}번 커넥터에서 그리기 시작.");
        startConnector = startConn;
        GameObject wireObj = Instantiate(wirePrefab, wireContainer);
        currentDrawingWire = wireObj.GetComponent<Wire>();
    }
    
    public void DropWireOnConnector(Connector endConnector)
    {
        // ... (DropWireOnConnector 내용은 동일) ...
        if (endConnector == null)
        {
            Debug.Log("<color=orange>연결 실패: 커넥터가 아닌 빈 공간에 놓았습니다.</color>");
            if(currentDrawingWire != null) Destroy(currentDrawingWire.gameObject);
        }
        else
        {
            if (startConnector == null)
            {
                Debug.LogWarning("DropWireOnConnector: startConnector가 null입니다.");
                if (currentDrawingWire != null) Destroy(currentDrawingWire.gameObject);
                currentDrawingWire = null;
                return;
            }

            Debug.Log($"연결 시도: 시작 ID({startConnector.ConnectorId}) -> 도착 ID({endConnector.ConnectorId})");

            bool isRightSide = !endConnector.IsLeft;
            bool isNotConnected = !endConnector.IsConnected;
            bool isIdMatched = startConnector.ConnectorId == endConnector.ConnectorId;

            if (isRightSide && isNotConnected && isIdMatched)
            {
                Debug.Log($"<color=green>연결 성공!</color> ID: {startConnector.ConnectorId}");
                startConnector.IsConnected = true;
                endConnector.IsConnected = true;
                currentDrawingWire.SetProperties(startConnector.transform.position, endConnector.transform.position, wireColors[startConnector.ConnectorId]);
                CheckForCompletion(); // ▼▼▼ 성공 시 여기 호출 ▼▼▼
            }
            else
            {
                Debug.Log("<color=red>연결 실패 원인:</color>");
                if (!isRightSide) Debug.Log("- 도착점이 오른쪽 커넥터가 아닙니다.");
                if (!isNotConnected) Debug.Log("- 도착점이 이미 다른 선과 연결되어 있습니다.");
                if (!isIdMatched) Debug.Log("- 시작점과 도착점의 ID(색상)가 다릅니다.");
                if(currentDrawingWire != null) Destroy(currentDrawingWire.gameObject);
            }
        }

        startConnector = null;
        currentDrawingWire = null;
    }

    public bool IsDrawingWire() => startConnector != null;

    void CheckForCompletion()
    {
        if (leftConnectors.Count > 0 && leftConnectors.All(conn => conn.IsConnected))
        {
            // ▼▼▼ 퍼즐을 '성공해서' 닫도록 true 값 전달 ▼▼▼
            ClosePuzzlePanel(true);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        // ... (Shuffle 내용은 동일) ...
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) { n--; int k = rng.Next(n + 1); T value = list[k]; list[k] = list[n]; list[n] = value; }
    }
}