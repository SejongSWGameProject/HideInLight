using UnityEngine;
using UnityEngine.Events;
using System.Collections; // 딜레이(Coroutine) 사용

public class WirePuzzleController : MonoBehaviour
{
    [Header("오브젝트 연결")]
    public LineRenderer[] wireLines; // 전선 3가닥
    public WirePort[] topPorts;      // 위쪽 포트 3개
    public WirePort[] bottomPorts;   // 아래쪽 포트 10개

    [Header("정답 설정")]
    public int[] correctAnswers = new int[3] { 0, 4, 8 }; 

    [Header("이벤트")]
    public UnityEvent OnPuzzleSolved;    
    public UnityEvent OnPuzzleCancelled; 

    // 내부 변수
    private int[] currentConnections = new int[3] { -1, -1, -1 }; 
    private bool isDragging = false;       
    private int draggingWireIndex = -1;    

    void Update()
    {
        // ESC 키 (취소)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPuzzle();
        }

        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        // 드래그 중일 때 전선 끝 이동
        if (isDragging)
        {
            UpdateDraggingWire();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                WirePort port = hit.collider.GetComponent<WirePort>();

                if (port != null)
                {
                    if (port.type == WirePort.PortType.Top)
                    {
                        StartDragging(port.portID);
                    }
                    else if (port.type == WirePort.PortType.Bottom)
                    {
                        if (isDragging)
                        {
                            ConnectWire(draggingWireIndex, port.portID);
                        }
                    }
                }
                else 
                {
                    // 빈 곳 클릭 시 취소
                    if (isDragging)
                    {
                        ResetWire(draggingWireIndex);
                        isDragging = false;
                        draggingWireIndex = -1;
                    }
                }
            }
        }
    }

    // ★★★ [핵심 함수] 포트의 정확한 연결 지점(Spot)을 찾아주는 기능 ★★★
    Vector3 GetSnapPosition(WirePort port)
    {
        // 1순위: 직접 지정한 연결 포인트(SnapPoint)가 있다면 거기를 사용
        if (port.connectionPoint != null)
        {
            return port.connectionPoint.position;
        }

        // 2순위: 없다면 콜라이더의 정중앙 사용
        Collider col = port.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds.center;
        }
        
        // 3순위: 그것도 없다면 오브젝트 위치 그대로 사용
        return port.transform.position;
    }

    void StartDragging(int topIndex)
    {
        isDragging = true;
        draggingWireIndex = topIndex;

        LineRenderer lr = wireLines[topIndex];
        lr.enabled = true; 
        
        // ★★★ 전선 두께 강제 고정 & 점 개수 확보 ★★★
        lr.positionCount = 2;   // 점 2개 (에러 방지)
        lr.startWidth = 0.02f;  // 시작 두께 얇게
        lr.endWidth = 0.02f;    // 끝 두께 얇게
        
        // ★★★ SnapPoint 위치 가져오기 ★★★
        Vector3 startPos = GetSnapPosition(topPorts[topIndex]);
        
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, startPos); 
    }

    void UpdateDraggingWire()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 마우스 따라다니는 끝점 (살짝 띄워서 파묻힘 방지)
            Vector3 targetPos = hit.point - (Camera.main.transform.forward * 0.05f);
            wireLines[draggingWireIndex].SetPosition(1, targetPos);
        }
    }

    void ConnectWire(int topIndex, int bottomIndex)
    {
        // ★★★ 아래쪽 포트의 SnapPoint에 정확히 꽂기 ★★★
        Vector3 endPos = GetSnapPosition(bottomPorts[bottomIndex]);
        
        wireLines[topIndex].SetPosition(1, endPos);
        currentConnections[topIndex] = bottomIndex;
        
        isDragging = false;
        draggingWireIndex = -1;

        Debug.Log($"연결됨: 위{topIndex} -> 아래{bottomIndex}");
        
        CheckVictory();
    }

    void ResetWire(int index)
    {
        wireLines[index].enabled = false;
        currentConnections[index] = -1; 
    }

    // ▼▼▼ 틀렸을 때 리셋하는 로직 (기존 유지) ▼▼▼
    void CheckVictory()
    {
        bool isAllConnected = true;
        for(int i=0; i<3; i++)
        {
            if(currentConnections[i] == -1) { isAllConnected = false; break; }
        }

        if (!isAllConnected) return;

        bool isCorrect = true;
        for (int i = 0; i < 3; i++)
        {
            if (currentConnections[i] != correctAnswers[i]) { isCorrect = false; break; }
        }

        if (isCorrect)
        {
            Debug.Log("퍼즐 성공!");
            OnPuzzleSolved.Invoke();
            this.enabled = false;
        }
        else
        {
            Debug.Log("틀렸습니다! 리셋합니다.");
            StartCoroutine(ResetPuzzleRoutine());
        }
    }

    IEnumerator ResetPuzzleRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++) ResetWire(i);
    }

    // ★★★ [수정된 부분] ESC 시 모든 선 초기화 ★★★
    public void CancelPuzzle()
    {
        // 1. 드래그 중이던 선이 있다면 먼저 리셋
        if (isDragging)
        {
            ResetWire(draggingWireIndex);
            isDragging = false;
        }

        // 2. ESC로 나갈 때, 현재 연결된 모든 선을 강제로 끊고 초기화!
        for (int i = 0; i < 3; i++)
        {
            ResetWire(i);
        }
        
        // 3. 외부 이벤트 호출 (퍼즐 창 닫기)
        OnPuzzleCancelled.Invoke();
    }
}