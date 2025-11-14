using UnityEngine;

public class SocketConnector3D : MonoBehaviour
{
    [Header("Socket Settings")]
    public int socketID;
    public Color socketColor = Color.gray;

    [Header("Socket Components")]
    [SerializeField] private Transform connectionPoint; // 연결 지점

    private bool isOccupied = false;
    private WireConnector3D connectedWire;
    private Renderer socketRenderer;
    private Color originalColor;

    void Awake()
    {
        if (connectionPoint == null)
            connectionPoint = transform;

        socketRenderer = GetComponent<Renderer>();
        if (socketRenderer != null)
        {
            originalColor = socketRenderer.material.color;
            socketRenderer.material.color = socketColor;
        }
    }

    public void Initialize()
    {
        isOccupied = false;
        connectedWire = null;

        if (socketRenderer != null)
            socketRenderer.material.color = socketColor;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool occupied, WireConnector3D wire)
    {
        isOccupied = occupied;
        connectedWire = wire;
    }

    public Vector3 GetConnectionPoint()
    {
        return connectionPoint.position;
    }

    public WireConnector3D GetConnectedWire()
    {
        return connectedWire;
    }

    // 마우스 오버 하이라이트
    void OnMouseEnter()
    {
        if (!isOccupied && socketRenderer != null)
        {
            socketRenderer.material.color = Color.Lerp(socketColor, Color.white, 0.5f);
        }
    }

    void OnMouseExit()
    {
        if (!isOccupied && socketRenderer != null)
        {
            socketRenderer.material.color = socketColor;
        }
    }
}