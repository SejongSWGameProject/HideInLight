using UnityEngine;
using UnityEngine.UI;

public class Socket : MonoBehaviour
{
    [Header("Socket Settings")]
    public int socketID; // 소켓 고유 ID
    public Color socketColor = Color.gray; // 소켓 색상

    [Header("Components")]
    [SerializeField] private Image socketImage;

    private RectTransform rectTransform;
    private bool isOccupied = false;
    private Wire connectedWire;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (socketImage != null)
            socketImage.color = socketColor;
    }

    public void Initialize()
    {
        isOccupied = false;
        connectedWire = null;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool occupied, Wire wire)
    {
        isOccupied = occupied;
        connectedWire = wire;

        // 소켓이 채워지면 색상 변경 (선택사항)
        if (socketImage != null)
        {
            socketImage.color = occupied ? Color.white : socketColor;
        }
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public Wire GetConnectedWire()
    {
        return connectedWire;
    }
}