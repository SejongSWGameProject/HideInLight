using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 3D 오브젝트와 F키로 상호작용합니다.
/// - 플레이어가 근접(Trigger)했고, 퍼즐이 열려있지 않을 때만 UI를 표시하고 상호작용을 허용합니다.
/// </summary>
public class InteractableObject : MonoBehaviour
{
    [Header("상호작용 설정")]
    [Tooltip("상호작용에 사용할 키")]
    public KeyCode interactionKey = KeyCode.F; 
    
    // 이 변수는 더 이상 사용하지 않습니다. (Raycast 제거)
    // public float aimDistance = 2.5f; 

    [Header("연결")]
    [Tooltip("F키를 눌렀을 때 실행할 이벤트")]
    public UnityEvent OnInteract;

    [Tooltip("가까이 갔을 때 켤 상호작용 UI (예: 'F키' 텍스트)")]
    public GameObject interactionPromptUI;

    [Tooltip("퍼즐 UI의 활성 상태를 체크할 게임 오브젝트 (PopupAnchor)")]
    public GameObject puzzlePopupModel_Check;

    [Header("퍼즐 상태")]
    private bool isPuzzleSolved = false;
    private bool isPlayerNear = false; // 플레이어가 Trigger 안에 있는지 여부

    public bool canInteract = true;
    private string curTag;

    // 1. 시작할 때 UI 숨기기
    void Start()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
        curTag = this.transform.tag;
        //Debug.Log(curTag);
    }

    public void Interact()
    {
        if (canInteract)
        {
            OnInteract.Invoke();

        }
    }

    /// <summary>
    /// "PRESS F" 텍스트 UI의 활성화 상태를 안전하게 변경합니다.
    /// </summary>
    public void SetPromptUI()
    {
        if (interactionPromptUI != null)
        {
            if (canInteract)
            {
                interactionPromptUI.SetActive(true);
            }
            else
            {
                interactionPromptUI.SetActive(false);
            }
        }
    }

    public void SetPromptUI(bool show)
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(show);
        }
    }
    // (Puzzle Manager에서 호출) 퍼즐이 풀렸을 때 상호작용 비활성화
    public void OnPuzzleHasBeenSolved()
    {
        isPuzzleSolved = true;
        SetPromptUI(); 
    }

    public void CanInteract()
    {
        canInteract = true;
    }
    public void CannotInteract()
    {
        canInteract = false;
    }
} 
