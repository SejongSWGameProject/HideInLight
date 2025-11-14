using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WireConnectionPuzzle : MonoBehaviour
{
    [Header("Puzzle Components")]
    [SerializeField] private List<WireConnector3D> wires = new List<WireConnector3D>();
    [SerializeField] private List<SocketConnector3D> sockets = new List<SocketConnector3D>();
    [SerializeField] private Camera puzzleCamera;

    [Header("UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Visual Settings")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip solvedSound;

    private AudioSource audioSource;
    private InteractableDevice deviceScript;
    private int correctConnections = 0;

    void Start()
    {
        deviceScript = FindObjectOfType<InteractableDevice>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        InitializePuzzle();
    }

    void InitializePuzzle()
    {
        foreach (WireConnector3D wire in wires)
        {
            if (wire != null)
                wire.Initialize(this, puzzleCamera);
        }

        foreach (SocketConnector3D socket in sockets)
        {
            if (socket != null)
                socket.Initialize();
        }
    }

    public void OpenPuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(true);

        ResetPuzzle();

        if (feedbackText != null)
            feedbackText.text = "전선을 올바른 소켓에 연결하세요";

        // 마우스 표시
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 플레이어 움직임 비활성화
        if (deviceScript != null)
        {
            // 여기에 플레이어 컨트롤러 비활성화 코드
        }
    }

    public void ClosePuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        // 마우스 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 플레이어 움직임 활성화
        if (deviceScript != null)
        {
            // 여기에 플레이어 컨트롤러 활성화 코드
        }
    }

    public void OnWireConnected(WireConnector3D wire, SocketConnector3D socket)
    {
        if (wire.correctSocketID == socket.socketID)
        {
            // 올바른 연결
            wire.SetConnected(true);
            socket.SetOccupied(true, wire);
            wire.SetColor(correctColor);
            correctConnections++;

            PlaySound(correctSound);

            if (feedbackText != null)
                feedbackText.text = $"올바른 연결! ({correctConnections}/{wires.Count})";

            // 모두 연결되었는지 확인
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

            PlaySound(incorrectSound);

            if (feedbackText != null)
                feedbackText.text = "잘못된 연결입니다!";

            Invoke("ResetWrongConnection", 1f);
        }
    }

    void ResetWrongConnection()
    {
        foreach (WireConnector3D wire in wires)
        {
            if (wire != null && !wire.IsCorrectlyConnected())
            {
                SocketConnector3D socket = wire.GetConnectedSocket();
                if (socket != null)
                {
                    socket.SetOccupied(false, null);
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

        PlaySound(solvedSound);

        if (deviceScript != null)
        {
            deviceScript.OnPuzzleSolved();
        }

        Invoke("ClosePuzzle", 2f);
    }

    void ResetPuzzle()
    {
        correctConnections = 0;

        foreach (WireConnector3D wire in wires)
        {
            if (wire != null)
                wire.ResetPosition();
        }

        foreach (SocketConnector3D socket in sockets)
        {
            if (socket != null)
                socket.SetOccupied(false, null);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}