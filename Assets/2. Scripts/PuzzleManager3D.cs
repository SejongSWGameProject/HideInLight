using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 1-카메라 3D 팝업 방식 (플레이어 잠금)
// '클릭' 감지 로직(Raycast)이 이 스크립트의 Update()에 포함되어 있습니다.
public class PuzzleManager3D : MonoBehaviour
{
    [Header("3D 프리팹")]
    [Tooltip("전선을 그릴 LineRenderer 프리팹")]
    public GameObject wirePrefab; 

    [Header("3D 오브젝트 부모")]
    [Tooltip("왼쪽 커넥터(포트) 오브젝트 4개")]
    public List<Connector3D> leftConnectors;
    [Tooltip("오른쪽 커넥터(포트) 오브젝트 4개")]
    public List<Connector3D> rightConnectors;
    [Tooltip("생성된 전선이 들어갈 부모 오브젝트")]
    public Transform wireContainer; 

    [Header("퍼즐 설정")]
    [Tooltip("ID(0~3)별 전선 색상")]
    public Color[] wireColors;
    [Tooltip("클릭(Raycast)을 감지할 레이어 마스크 (예: Clickable3D)")]
    public LayerMask connectorLayerMask;
    
    [Header("퍼즐 UI (선택)")]
    [Tooltip("퍼즐이 켜지거나 꺼질 부모 팝업 (예: WirePuzzle_PopupAnchor)")]
    public GameObject puzzlePanel; 

    [Header("플레이어 제어")]
    [Tooltip("플레이어의 '움직임' 스크립트 (예: Player Move)")]
    public MonoBehaviour playerMovementScript;
    [Tooltip("플레이어의 '카메라 조작' 스크립트 (예: MouseLook)")]
    public MonoBehaviour cameraLookScript;
    
    // ▼▼▼ [수정됨] 1. 손전등 스크립트 변수 추가 ▼▼▼
    [Tooltip("플레이어의 '라이트' 스크립트 (예: Flashlight)")]
    public MonoBehaviour flashlightScript;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    private Wire3D currentDrawingWire; // 현재 그리고 있는 전선
    private Connector3D startConnector; // 전선 그리기 시작 포트
    private int connectorCount;
    private Camera puzzleCamera; // Raycast에 사용할 플레이어 메인 카메라

    void Start()
    {
        // 1-카메라 방식이므로, Raycast용 카메라는 Camera.main
        puzzleCamera = Camera.main; 
        
        // 포트 개수 확인 및 설정
        SetupPuzzle();
    }

    // 퍼즐이 켜질 때 (F키로 SetActive(true)될 때)
    void OnEnable()
    {
        Debug.Log("PuzzleManager3D: OnEnable - 전선 퍼즐 팝업 시작 (플레이어 잠금)");
        
        // 마우스 커서를 보이게 함
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 플레이어 움직임과 시점 잠금
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraLookScript != null) cameraLookScript.enabled = false;
        
        // ▼▼▼ [수정됨] 2. 손전등 끄기 ▼▼▼
        if (flashlightScript != null) flashlightScript.enabled = false;
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
    }

    // 퍼즐을 닫을 때 (ESC 또는 퍼즐 성공 시)
    public void ClosePuzzlePanel()
    {
        Debug.Log("PuzzleManager3D: ClosePuzzlePanel - 전선 퍼즐 팝업 닫기 (플레이어 잠금 해제)");

        // 마우스 커서를 숨김
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 플레이어 움직임과 시점 잠금 해제
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraLookScript != null) cameraLookScript.enabled = true;
        
        // ▼▼▼ [수정됨] 3. 손전등 다시 켜기 ▼▼▼
        if (flashlightScript != null) flashlightScript.enabled = true;
        // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

        // 퍼즐 팝업(자기 자신)을 끈다
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false); 
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 매 프레임 마우스 입력을 확인
    void Update()
    {
        // 1. 마우스 왼쪽 버튼을 '눌렀을 때' (그리기 시작)
        if (Input.GetMouseButtonDown(0))
        {
            Connector3D hitConnector = GetConnectorUnderMouse(); // 클릭된 포트 확인
            // 왼쪽 포트를 클릭했고, 이미 연결되지 않았다면
            if (hitConnector != null && hitConnector.IsLeft && !hitConnector.IsConnected)
            {
                StartDrawingWire(hitConnector); // 그리기 시작
            }
        }

        // 2. 마우스를 '드래그 중'일 때 (선 그리기)
        if (startConnector != null && currentDrawingWire != null)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            currentDrawingWire.UpdateEndPosition(mouseWorldPos); // 선 끝점만 마우스 따라다니기
        }

        // 3. 마우스 왼쪽 버튼을 '뗐을 때' (연결 시도)
        if (Input.GetMouseButtonUp(0))
        {
            Connector3D endConnector = null; 
            if (startConnector != null) // 그리고 있던 중이었다면
            {
                endConnector = GetConnectorUnderMouse(); // 마우스를 뗀 위치의 포트 확인
                DropWireOnConnector(endConnector); // 연결 시도
            }

            // 그리기 상태 초기화
            startConnector = null;
            if (currentDrawingWire != null) // 연결에 실패했거나 허공에 놓았다면
            {
                Destroy(currentDrawingWire.gameObject); // 그리던 선 파괴
            }
            currentDrawingWire = null;
        }

        // ESC 키로 퍼즐 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (startConnector != null) // 선을 그리던 중이었다면
            {
                 Destroy(currentDrawingWire.gameObject);
                 currentDrawingWire = null;
                 startConnector = null;
            }
            else // 일반 상태
            {
                ClosePuzzlePanel();
            }
        }
    }

    // 마우스 위치로 Raycast를 쏴서 3D 커넥터 찾기
    Connector3D GetConnectorUnderMouse()
    {
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // 'connectorLayerMask'에 설정된 레이어만 감지
        if (Physics.Raycast(ray, out hit, 50f, connectorLayerMask))
        {
            // Raycast에 맞은 오브젝트에서 Connector3D 컴포넌트(명찰) 찾기
            return hit.collider.GetComponent<Connector3D>();
        }
        return null; // 아무것도 맞지 않음
    }

    // 마우스 위치를 3D 월드 좌표로 변환 (Line Renderer 끝점용)
    Vector3 GetMouseWorldPosition()
    {
        // 팝업이 카메라 앞 Z=1.5에 있다고 가정
        // LineRenderer가 마우스를 따라다닐 때의 '가상 3D 좌표'
        float distance = 1.6f; 
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distance;
        return puzzleCamera.ScreenToWorldPoint(mousePos);
    }

    // 퍼즐 초기 설정
    void SetupPuzzle()
    {
        connectorCount = leftConnectors.Count;
        if (connectorCount == 0 || connectorCount != rightConnectors.Count)
        {
            Debug.LogError("PuzzleManager3D: 왼쪽/오른쪽 커넥터 개수가 다르거나 0입니다! Inspector를 확인하세요.");
            return;
        }

        List<int> rightSideIds = Enumerable.Range(0, connectorCount).ToList();
        Shuffle(rightSideIds); // 오른쪽 포트 ID 섞기

        for (int i = 0; i < connectorCount; i++)
        {
            // 왼쪽 커넥터 초기화 (ID: 0, 1, 2, 3)
            leftConnectors[i].Initialize(i, true);
            Renderer leftRenderer = leftConnectors[i].GetComponent<Renderer>();
            if (leftRenderer != null && i < wireColors.Length) 
                leftRenderer.material.color = wireColors[i];

            // 오른쪽 커넥터 초기화 (ID: 2, 0, 3, 1 - 섞인 순서)
            int randomId = rightSideIds[i];
            rightConnectors[i].Initialize(randomId, false);
            Renderer rightRenderer = rightConnectors[i].GetComponent<Renderer>();
            if (rightRenderer != null && randomId < wireColors.Length) 
                rightRenderer.material.color = wireColors[randomId];
        }
        Debug.Log("--- 3D 전선 퍼즐: " + connectorCount + "쌍의 커넥터 설정 완료 ---");
    }

    // 전선 그리기 시작
    void StartDrawingWire(Connector3D startConn)
    {
        Debug.Log($"StartDrawingWire: ID {startConn.ConnectorId}번 포트에서 그리기 시작.");
        startConnector = startConn;
        GameObject wireObj = Instantiate(wirePrefab, wireContainer);
        currentDrawingWire = wireObj.GetComponent<Wire3D>();
        
        Color wireColor = (startConn.ConnectorId < wireColors.Length) ? wireColors[startConn.ConnectorId] : Color.white;
        currentDrawingWire.SetProperties(startConnector.transform.position, GetMouseWorldPosition(), wireColor);
    }

    // 전선 놓기 (연결 시도)
    void DropWireOnConnector(Connector3D endConnector)
    {
        if (endConnector == null)
        {
            Debug.Log("<color=orange>연결 실패: 커넥터가 아닌 빈 공간에 놓았습니다.</color>");
            return;
        }
        
        Debug.Log($"연결 시도: 시작 ID({startConnector.ConnectorId}) -> 도착 ID({endConnector.ConnectorId})");

        // 성공 조건: 1.오른쪽인가? 2.비어있나? 3.ID가 맞나?
        bool isRightSide = !endConnector.IsLeft;
        bool isNotConnected = !endConnector.IsConnected;
        bool isIdMatched = startConnector.ConnectorId == endConnector.ConnectorId;

        if (isRightSide && isNotConnected && isIdMatched)
        {
            Debug.Log($"<color=green>연결 성공!</color> ID: {startConnector.ConnectorId}");
            startConnector.IsConnected = true;
            endConnector.IsConnected = true;
            
            Color wireColor = (startConnector.ConnectorId < wireColors.Length) ? wireColors[startConnector.ConnectorId] : Color.white;
            // 전선 끝점을 도착 포트에 고정
            currentDrawingWire.SetProperties(startConnector.transform.position, endConnector.transform.position, wireColor);
            
            currentDrawingWire = null; // 연결 성공! (파괴 방지)
            CheckForCompletion();
        }
        else
        {
            // 실패 원인 로그
            Debug.Log("<color=red>연결 실패 원인:</color>");
            if (!isRightSide) Debug.Log("- 도착점이 오른쪽 포트가 아닙니다.");
            if (!isNotConnected) Debug.Log("- 도착 포트가 이미 연결되어 있습니다.");
            if (!isIdMatched) Debug.Log("- 시작과 도착 포트의 ID(색상)가 다릅니다.");
            // (연결 실패, Update에서 그리던 선이 파괴됨)
        }
    }

    // 모든 전선이 연결되었는지 확인
    void CheckForCompletion()
    {
        if (leftConnectors.All(conn => conn.IsConnected))
        {
            Debug.Log("<color=cyan>---!!! 전선 퍼즐 클리어 !!!---</color>");
            // TODO: 퍼즐 성공 이벤트 (예: 문 열기)
            
            // 1초 뒤에 퍼즐 닫기
            Invoke("ClosePuzzlePanel", 1.0f);
        }
    }
    
    // 리스트 셔플 함수
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1) { n--; int k = rng.Next(n + 1); T value = list[k]; list[k] = list[n]; list[n] = value; }
    }
}