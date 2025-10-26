using UnityEngine;
using UnityEngine.EventSystems;

public class Connector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PuzzleManager puzzleManager;
    public int ConnectorId { get; private set; }
    public bool IsLeft { get; private set; }
    public bool IsConnected { get; set; }

    public void Initialize(PuzzleManager manager, int id, bool isLeft)
    {
        puzzleManager = manager;
        ConnectorId = id;
        IsLeft = isLeft;
        IsConnected = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ▼▼▼ 진짜 원인을 찾기 위한 디버그 로그! ▼▼▼
        Debug.Log($"드래그 시작: {gameObject.name}, IsLeft = {IsLeft}");

        if (IsLeft && !IsConnected && !puzzleManager.IsDrawingWire())
        {
            puzzleManager.StartDrawingWire(this);
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (puzzleManager.IsDrawingWire())
        {
            Connector endConnector = null;
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                endConnector = eventData.pointerCurrentRaycast.gameObject.GetComponent<Connector>();
            }
            puzzleManager.DropWireOnConnector(endConnector);
        }
    }
}