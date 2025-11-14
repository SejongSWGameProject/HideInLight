using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Wire Settings")]
    public int correctSocketID; // 이 전선이 연결되어야 할 소켓 ID
    public Color wireColor = Color.blue; // 전선 색상

    [Header("Components")]
    [SerializeField] private Image wireImage;
    [SerializeField] private LineRenderer lineRenderer; // 3D 선 (선택사항)

    [Header("Line Settings")]
    [SerializeField] private bool use3DLine = false; // 3D LineRenderer 사용 여부
    [SerializeField] private bool useCurvedLine = true; // 곡선 사용 여부
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float curvature = 0.3f; // 곡선 정도

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private WirePuzzle puzzleManager;
    private Socket connectedSocket;
    private bool isConnected = false;
    private WireLine uiLine; // UI 선 컴포넌트
    private CurvedWireLine curvedLine; // 곡선 컴포넌트

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPosition = rectTransform.anchoredPosition;

        if (wireImage != null)
            wireImage.color = wireColor;

        // UI 선 생성
        if (!use3DLine)
        {
            GameObject lineObj = new GameObject("WireLine");
            lineObj.transform.SetParent(transform.parent, false);
            lineObj.transform.SetSiblingIndex(0); // 전선 뒤에 그리기

            if (useCurvedLine)
            {
                // 곡선 사용
                curvedLine = lineObj.AddComponent<CurvedWireLine>();
                curvedLine.color = wireColor;
                curvedLine.SetLineWidth(lineWidth);
                curvedLine.SetCurvature(curvature);
                curvedLine.raycastTarget = false;
            }
            else
            {
                // 직선 사용
                uiLine = lineObj.AddComponent<WireLine>();
                uiLine.Initialize(rectTransform, wireColor, lineWidth);
            }
        }
        // 3D LineRenderer 설정
        else if (lineRenderer != null)
        {
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = wireColor;
            lineRenderer.endColor = wireColor;
            lineRenderer.positionCount = 0;
        }
    }

    public void Initialize(WirePuzzle puzzle)
    {
        puzzleManager = puzzle;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isConnected) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // 선 업데이트
        UpdateLine();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isConnected) return;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 드롭한 위치에 소켓이 있는지 확인
        Socket socket = GetSocketAtPosition(eventData.position);

        if (socket != null && !socket.IsOccupied())
        {
            // 소켓에 연결
            rectTransform.anchoredPosition = socket.GetRectTransform().anchoredPosition;
            connectedSocket = socket;
            puzzleManager.OnWireConnected(this, socket);
        }
        else
        {
            // 소켓이 없으면 원위치
            ResetPosition();
        }
    }

    Socket GetSocketAtPosition(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Socket socket = result.gameObject.GetComponent<Socket>();
            if (socket != null)
                return socket;
        }

        return null;
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        connectedSocket = null;
        isConnected = false;

        if (wireImage != null)
            wireImage.color = wireColor;

        // 선 숨기기
        HideLine();
    }

    void UpdateLine()
    {
        if (use3DLine && lineRenderer != null)
        {
            // 3D LineRenderer 업데이트
            lineRenderer.positionCount = 2;
            Vector3 startPos = new Vector3(originalPosition.x, originalPosition.y, 0);
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, rectTransform.position);
        }
        else if (useCurvedLine && curvedLine != null)
        {
            // 곡선 업데이트
            curvedLine.SetPositions(originalPosition, rectTransform.anchoredPosition);
        }
        else if (uiLine != null)
        {
            // 직선 업데이트
            uiLine.UpdateLine(rectTransform.anchoredPosition);
        }
    }

    void HideLine()
    {
        if (use3DLine && lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
        else if (useCurvedLine && curvedLine != null)
        {
            curvedLine.HideLine();
        }
        else if (uiLine != null)
        {
            uiLine.HideLine();
        }
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
        if (wireImage != null)
            wireImage.color = color;

        // 선 색상도 변경
        if (use3DLine && lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
        else if (useCurvedLine && curvedLine != null)
        {
            curvedLine.color = color;
            curvedLine.SetVerticesDirty();
        }
        else if (uiLine != null)
        {
            uiLine.SetColor(color);
        }
    }

    public Socket GetConnectedSocket()
    {
        return connectedSocket;
    }

    [Header("3D Model")]
    [SerializeField] private Transform wireEndModel; // 3D 모델 참조
}