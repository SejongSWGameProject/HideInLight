using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WirePuzzle : MonoBehaviour
{
    [Header("Puzzle Components")]
    [SerializeField] private List<Wire> wires = new List<Wire>();
    [SerializeField] private List<Socket> sockets = new List<Socket>();
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Visual Settings")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;

    private InteractableDevice deviceScript;
    private int correctConnections = 0;

    void Start()
    {
        deviceScript = FindObjectOfType<InteractableDevice>();

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);

        InitializePuzzle();
    }

    void OnEnable()
    {
        ResetPuzzle();
    }

    void InitializePuzzle()
    {
        // 각 전선에 퍼즐 참조 전달
        foreach (Wire wire in wires)
        {
            if (wire != null)
                wire.Initialize(this);
        }

        // 각 소켓 초기화
        foreach (Socket socket in sockets)
        {
            if (socket != null)
                socket.Initialize();
        }
    }

    public void OnWireConnected(Wire wire, Socket socket)
    {
        // 올바른 연결인지 확인
        if (wire.correctSocketID == socket.socketID)
        {
            wire.SetConnected(true);
            socket.SetOccupied(true, wire);
            wire.SetColor(correctColor);
            correctConnections++;

            if (feedbackText != null)
                feedbackText.text = $"올바른 연결! ({correctConnections}/{wires.Count})";

            // 모든 전선이 올바르게 연결되었는지 확인
            if (correctConnections >= wires.Count)
            {
                Invoke("OnPuzzleSolved", 1f);
            }
        }
        else
        {
            // 틀린 연결
            wire.SetColor(incorrectColor);
            socket.SetOccupied(true, wire);

            if (feedbackText != null)
                feedbackText.text = "잘못된 연결입니다!";

            // 1초 후 원위치
            Invoke("ResetWrongConnection", 1f);
        }
    }

    void ResetWrongConnection()
    {
        foreach (Wire wire in wires)
        {
            if (wire != null && !wire.IsCorrectlyConnected())
            {
                Socket occupiedSocket = wire.GetConnectedSocket();
                if (occupiedSocket != null)
                {
                    occupiedSocket.SetOccupied(false, null);
                }
                wire.ResetPosition();
            }
        }

        if (feedbackText != null && correctConnections < wires.Count)
            feedbackText.text = "전선을 올바른 소켓에 연결하세요";
    }

    void OnPuzzleSolved()
    {
        if (feedbackText != null)
            feedbackText.text = "퍼즐 해결!";

        if (deviceScript != null)
            deviceScript.OnPuzzleSolved();
    }

    void ClosePuzzle()
    {
        if (deviceScript != null)
            deviceScript.ClosePuzzle();
    }

    void ResetPuzzle()
    {
        correctConnections = 0;

        foreach (Wire wire in wires)
        {
            if (wire != null)
                wire.ResetPosition();
        }

        foreach (Socket socket in sockets)
        {
            if (socket != null)
                socket.SetOccupied(false, null);
        }

        if (feedbackText != null)
            feedbackText.text = "전선을 올바른 소켓에 연결하세요";
    }
}