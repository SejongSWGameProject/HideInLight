using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 3D 오브젝트와 F키로 상호작용합니다.
/// </summary>
public class InteractableObject : MonoBehaviour
{
    [Header("상호작용 설정")]
    [Tooltip("상호작용에 사용할 키")]
    public KeyCode interactionKey = KeyCode.F; 
    
    [Header("연결")]
    [Tooltip("F키를 눌렀을 때 실행할 이벤트")]
    public UnityEvent OnInteract;

    [Tooltip("가까이 갔을 때 켤 상호작용 UI (예: 'F키' 텍스트)")]
    public GameObject interactionPromptUI;

    [Tooltip("퍼즐 UI의 활성 상태를 체크할 게임 오브젝트 (PopupAnchor)")]
    public GameObject puzzlePopupModel_Check;

    [Header("퍼즐 상태")]
    private bool isPuzzleSolved = false;
    
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
        // 파라미터 없는 버전도 아래 로직을 타도록 수정
        if (interactionPromptUI != null)
        {
            // 퍼즐이 켜져 있으면 무조건 끔
            if (puzzlePopupModel_Check != null && puzzlePopupModel_Check.activeInHierarchy)
            {
                interactionPromptUI.SetActive(false);
                return;
            }

            // 그게 아니면 interact 가능 여부에 따라 결정
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

    // ★★★ [여기가 핵심 수정 부분] ★★★
    public void SetPromptUI(bool show)
    {
        // 1. 안전장치: 연결된 퍼즐 창(puzzlePopupModel_Check)이 켜져 있다면?
        //    -> 들어온 명령(show=true)을 무시하고 강제로 끕니다(false).
        if (puzzlePopupModel_Check != null && puzzlePopupModel_Check.activeInHierarchy)
        {
            show = false;
        }

        // 2. 결정된 값으로 UI 설정
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(show);
        }
    }

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