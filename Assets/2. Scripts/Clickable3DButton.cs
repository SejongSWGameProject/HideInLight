using UnityEngine;
using UnityEngine.UI; // UI의 Toggle을 사용하기 위해 필요합니다.
using System.Collections; // 코루틴(애니메이션)을 사용하기 위해 필요합니다.

/// <summary>
/// 3D 오브젝트(자신)를 클릭했을 때,
/// 지정된 'movablePart'(부모)의 위치를 'Move Offset'만큼 애니메이션시키고
/// 'targetToggle'(UI)의 상태를 뒤집습니다.
/// </summary>
public class Clickable3DButton : MonoBehaviour
{
    [Header("기본 연결")]
    [Tooltip("이 버튼으로 제어할 '보이지 않는' UI 토글")]
    public Toggle targetToggle; 

    [Tooltip("실제로 이동시킬 부모 오브젝트 (예: switch_handle)")]
    public Transform movablePart; 

    [Tooltip("레이캐스트가 감지할 레이어 마스크 (Clickable3D 레이어)")]
    public LayerMask clickableLayerMask;

    [Header("애니메이션 설정")]
    [Tooltip("버튼이 눌리는 애니메이션 시간 (초)")]
    public float animationDuration = 0.15f;

    // ▼▼▼ 'Y Move Distance' 대신 'Move Offset'으로 변경! ▼▼▼
    [Tooltip("버튼이 '눌렸을 때' 이동할 방향과 거리 (예: Z축으로 들어가면 0, 0, 0.05)")]
    public Vector3 moveOffset = new Vector3(0, -0.05f, 0); // Y축 대신 (0, 0, 0.05) 등으로 수정 가능

    // 애니메이션 상태 변수
    private Vector3 originalPosition; // 원래 '올라온' 위치
    private Vector3 downPosition;     // '눌린' 위치
    private bool isAnimating = false;
    private bool hasPositionBeenSet = false;

    void OnEnable()
    {
        if (movablePart == null)
        {
            movablePart = transform;
        }
        
        if (!hasPositionBeenSet) 
        {
            // 'movablePart'(부모)의 '원래 로컬 위치' (X, Y, Z)를 그대로 읽어옵니다.
            originalPosition = movablePart.localPosition;
            
            // ▼▼▼ '내려간' 위치는 '원래 위치'에서 'Move Offset'만큼 더한 값입니다. ▼▼▼
            // (Y축만 빼는 것이 아니라, 설정한 방향(Offset)으로 움직입니다.)
            downPosition = originalPosition + moveOffset; 
            
            hasPositionBeenSet = true;
        }

        // 팝업을 켤 때마다 UI 토글 상태와 3D 버튼 위치를 동기화
        if (targetToggle != null)
        {
            if (targetToggle.isOn)
            {
                movablePart.localPosition = downPosition; // '눌린' 상태로 시작
            }
            else
            {
                movablePart.localPosition = originalPosition; // '올라온' 상태로 시작
            }
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            HandleClickRaycast();
        }
    }

    private void HandleClickRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!hasPositionBeenSet) return; 

        if (Physics.Raycast(ray, out hit, 50f, clickableLayerMask))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                Debug.Log($"[클릭 성공 - Raycast] {gameObject.name}이 감지되었습니다!");

                if (targetToggle != null)
                {
                    targetToggle.isOn = !targetToggle.isOn;
                    StartAnimation(targetToggle.isOn);
                }
                else
                {
                    Debug.LogWarning(gameObject.name + "에 'targetToggle'이 연결되지 않았습니다.");
                }
            }
        }
    }
    
    // (이하 애니메이션 코루틴은 동일합니다)
    private void StartAnimation(bool isNowOn)
    {
        Vector3 targetPosition = isNowOn ? downPosition : originalPosition;
        StopAllCoroutines();
        StartCoroutine(AnimateButtonPosition(targetPosition));
    }

    private IEnumerator AnimateButtonPosition(Vector3 targetPos)
    {
        isAnimating = true;
        float timer = 0;
        Vector3 startPos = movablePart.localPosition; 

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0.0f, 1.0f, timer / animationDuration);
            movablePart.localPosition = Vector3.Lerp(startPos, targetPos, t); 
            yield return null; 
        }

        movablePart.localPosition = targetPos; 
        isAnimating = false;
    }
}