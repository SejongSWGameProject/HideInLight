using UnityEngine;
using UnityEngine.UI; // UI의 Toggle을 사용하기 위해 필요합니다.
using System.Collections; // 코루틴(애니메이션)을 사용하기 위해 필요합니다.

/// <summary>
/// 3D 오브젝트(자신)를 클릭했을 때,
/// 지정된 'movablePart'(부모)의 위치를 애니메이션시키고
/// 'targetToggle'(UI)의 상태를 뒤집습니다.
/// (버튼 사라짐/날아감 오류 최종 수정됨)
/// </summary>
public class Clickable3DButton : MonoBehaviour
{
    [Header("기본 연결")]
    [Tooltip("이 버튼으로 제어할 '보이지 않는' UI 토글")]
    public Toggle targetToggle; 

    [Tooltip("실제로 Y 위치를 이동시킬 부모 오브젝트 (예: switch_handle)")]
    public Transform movablePart; // ★★★ 이게 다시 필요합니다! ★★★

    [Tooltip("레이캐스트가 감지할 레이어 마스크 (Clickable3D 레이어)")]
    public LayerMask clickableLayerMask;

    [Header("애니메이션 설정")]
    [Tooltip("버튼이 눌리는 애니메이션 시간 (초)")]
    public float animationDuration = 0.15f;

    [Tooltip("버튼이 Y축(아래)으로 눌리는 거리")]
    public float yMoveDistance = 0.05f;

    // 애니메이션 상태 변수
    private Vector3 originalPosition; // 원래 '올라온' 위치
    private Vector3 downPosition;     // '눌린' 위치
    private bool isAnimating = false; // 현재 애니메이션 중인지
    private bool hasPositionBeenSet = false; // 위치가 한번이라도 설정되었는지 확인

    /// <summary>
    /// OnEnable은 팝업(PopupAnchor)이 켜질 때마다 호출됩니다.
    /// </summary>
    void OnEnable()
    {
        // (중요!) 'movablePart'(부모)가 연결 안됐으면, 자기 자신을 움직이도록 설정
        if (movablePart == null)
        {
            movablePart = transform;
        }
        
        // 팝업이 켜지는 이 시점에 '원래 위치'를 읽습니다.
        if (!hasPositionBeenSet) 
        {
            // 'movablePart'(부모)의 '원래 로컬 위치' (X, Y, Z)를 그대로 읽어옵니다.
            originalPosition = movablePart.localPosition;
            
            // '내려간' 위치는 '원래 위치'에서 Y축만 조절합니다.
            downPosition = originalPosition - new Vector3(0, yMoveDistance, 0);
            
            hasPositionBeenSet = true; // 위치 저장을 완료했음
        }

        // 팝업을 켤 때마다, '보이지 않는' UI 토글의 현재 상태와
        // 3D 버튼의 '눌림' 상태를 즉시 동기화합니다.
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


    // 마우스 버튼이 눌렸을 때 (매 프레임 호출)
    private void Update()
    {
        // 마우스 왼쪽 버튼(0번)이 눌렸고, 현재 애니메이션 중이 아닐 때만 처리
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
            // (중요!) 충돌한 물체가 바로 이 스크립트가 붙은 오브젝트(자식)인지 확인
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
        Vector3 startPos = movablePart.localPosition; // movablePart의 위치를 변경

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0.0f, 1.0f, timer / animationDuration);
            movablePart.localPosition = Vector3.Lerp(startPos, targetPos, t); // movablePart의 위치를 변경
            yield return null; 
        }

        movablePart.localPosition = targetPos; // movablePart의 위치를 변경
        isAnimating = false;
    }
}