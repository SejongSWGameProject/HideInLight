using UnityEngine;

public class WireConnector3D : MonoBehaviour
{
    [Header("Wire Settings")]
    public int correctSocketID;
    public Color wireColor = Color.blue;

    [Header("Wire Components")]
    [SerializeField] private Transform startPoint; // 전선 시작점 (고정 3D 모델)
    [SerializeField] private Transform endPoint; // 전선 끝점 (드래그 가능한 3D 모델)
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Interaction")]
    [SerializeField] private float dragSensitivity = 0.01f;
    [SerializeField] private LayerMask socketLayer;

    private Camera puzzleCamera;
    private WireConnectionPuzzle puzzleManager;
    private SocketConnector3D connectedSocket;
    private bool isDragging = false;
    private bool isConnected = false;
    private Vector3 originalEndPosition;
    private float dragDistance;
    private Material lineMaterial;

    void Awake()
    {
        originalEndPosition = endPoint.position;

        // LineRenderer 설정
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
    }

    void SetupLineRenderer()
    {
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // 재질 생성
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = wireColor;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = wireColor;
        lineRenderer.endColor = wireColor;

        UpdateLine();
    }

    public void Initialize(WireConnectionPuzzle puzzle, Camera cam)
    {
        puzzleManager = puzzle;
        puzzleCamera = cam;
        originalEndPosition = endPoint.position;

        // 끝점 색상 설정
        Renderer renderer = endPoint.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = wireColor;
        }
    }

    void Update()
    {
        if (isConnected) return;
        UpdateLine();
    }

    void OnMouseDown()
    {
        if (isConnected || puzzleCamera == null) return;

        isDragging = true;

        // 카메라에서 끝점까지의 거리 계산
        dragDistance = Vector3.Distance(puzzleCamera.transform.position, endPoint.position);
    }

    void OnMouseDrag()
    {
        if (!isDragging || isConnected) return;

        // 마우스 위치를 3D 공간으로 변환
        Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPosition = ray.GetPoint(dragDistance);

        Debug.Log("그리는중");

        // 끝점 위치 업데이트
        endPoint.position = Vector3.Lerp(endPoint.position, targetPosition, dragSensitivity * 100f * Time.deltaTime);
    }

    void OnMouseUp()
    {
        if (!isDragging || isConnected) return;

        isDragging = false;

        // 가까운 소켓 찾기
        SocketConnector3D nearestSocket = FindNearestSocket();

        if (nearestSocket != null && !nearestSocket.IsOccupied())
        {
            // 소켓에 연결
            endPoint.position = nearestSocket.GetConnectionPoint();
            connectedSocket = nearestSocket;
            puzzleManager.OnWireConnected(this, nearestSocket);
        }
        else
        {
            // 원위치
            ResetPosition();
        }
    }

    SocketConnector3D FindNearestSocket()
    {
        Collider[] colliders = Physics.OverlapSphere(endPoint.position, 0.3f, socketLayer);

        SocketConnector3D nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            SocketConnector3D socket = col.GetComponent<SocketConnector3D>();
            if (socket != null && !socket.IsOccupied())
            {
                float dist = Vector3.Distance(endPoint.position, socket.GetConnectionPoint());
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = socket;
                }
            }
        }

        return nearest;
    }

    void UpdateLine()
    {
        if (lineRenderer != null && startPoint != null && endPoint != null)
        {
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
        }
    }

    public void ResetPosition()
    {
        endPoint.position = originalEndPosition;
        connectedSocket = null;
        isConnected = false;
        isDragging = false;

        Renderer renderer = endPoint.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = wireColor;
        }

        UpdateLine();
    }

    public void SetConnected(bool connected)
    {
        isConnected = connected;
    }

    public bool IsCorrectlyConnected()
    {
        return isConnected;
    }

    public void SetColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineMaterial.color = color;
        }

        Renderer renderer = endPoint.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    public SocketConnector3D GetConnectedSocket()
    {
        return connectedSocket;
    }
}