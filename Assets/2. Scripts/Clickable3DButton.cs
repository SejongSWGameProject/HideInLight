using UnityEngine;
using UnityEngine.UI; // UI의 Toggle을 사용하기 위해 필요합니다.
using System.Collections; // 코루틴(애니메이션)을 사용하기 위해 필요합니다.

/// <summary>
/// 3D 오브젝트(자신)를 클릭했을 때,
/// 지정된 'movablePart'(부모)의 각도(Rotation)를 'Up'과 'Down' 각도 사이에서 애니메이션시키고
/// 'targetToggle'(UI)의 상태를 뒤집습니다.
/// (모델의 원래 각도를 '중심'으로 자동 인식합니다)
/// </summary>
public class Clickable3DButton : MonoBehaviour
{
    [Header("기본 연결")]
    [Tooltip("이 버튼으로 제어할 '보이지 않는' UI 토글")]
    public Toggle targetToggle; 

    [Tooltip("실제로 회전시킬 부모 오브젝트 (예: switch_handle)")]
    public Transform movablePart; 

    [Tooltip("레이캐스트가 감지할 레이어 마스크 (Clickable3D 레이어)")]
    public LayerMask clickableLayerMask;

    [Header("애니메이션 설정")]
    [Tooltip("버튼이 회전하는 애니메이션 시간 (초)")]
    public float animationDuration = 0.15f;

    // ▼▼▼ 'rotationOffset' 대신 'Up/Down' 오프셋으로 변경 ▼▼▼
    [Tooltip("버튼이 '올라온' 상태일 때 적용할 '상대적인' 회전 오프셋 (예: X축 -15도)")]
    public Vector3 upRotationOffset = new Vector3(-15, 0, 0); 

    [Tooltip("버튼이 '눌린' 상태일 때 적용할 '상대적인' 회전 오프셋 (예: X축 15도)")]
    public Vector3 downRotationOffset = new Vector3(15, 0, 0); 

    // 애니메이션 상태 변수
    private Quaternion baseRotation;      // 모델의 '원래(중심)' 각도
    private Quaternion upRotationQ;       // '올라온' 목표 각도
    private Quaternion downRotationQ;     // '눌린' 목표 각도
    private bool isAnimating = false;
    private bool hasRotationBeenSet = false; // 각도 설정 여부 확인

    void OnEnable()
    {
        if (movablePart == null)
        {
            movablePart = transform;
        }
        
        // '올라온' 각도와 '눌린' 각도를 Quaternion으로 변환하여 저장
        if (!hasRotationBeenSet) 
        {
            // 1. movablePart의 '현재 로컬 각도'를 '중심' 각도로 저장합니다.
            //    (이제 (0,0,0)으로 강제로 뒤집히지 않습니다!)
            baseRotation = movablePart.localRotation; 
            
            // 2. '올라온' 상태는 '중심' 각도 + 'Up 오프셋'입니다.
            upRotationQ = baseRotation * Quaternion.Euler(upRotationOffset);
            
            // 3. '눌린' 상태는 '중심' 각도 + 'Down 오프셋'입니다.
            downRotationQ = baseRotation * Quaternion.Euler(downRotationOffset);
            
            hasRotationBeenSet = true;
        }

        // 팝업을 켤 때마다 UI 토글 상태와 3D 버튼 각도를 동기화
        if (targetToggle != null)
        {
            if (targetToggle.isOn)
            {
                movablePart.localRotation = downRotationQ; // '눌린' 각도로 시작
            }
            else
            {
                movablePart.localRotation = upRotationQ; // '올라온' 각도로 시작
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

        if (!hasRotationBeenSet) return; 

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
    
    // 애니메이션 시작 함수
    private void StartAnimation(bool isNowOn)
    {
        // 목표 각도를 설정 (켜졌으면 Down, 꺼졌으면 Up)
        Quaternion targetRotation = isNowOn ? downRotationQ : upRotationQ;
        StopAllCoroutines();
        // 새 코루틴(애니메이션) 시작
        StartCoroutine(AnimateButtonRotation(targetRotation));
    }

    // (IEnumerator) 부드러운 회전 애니메이션 코루틴
    private IEnumerator AnimateButtonRotation(Quaternion targetRot)
    {
        isAnimating = true;
        float timer = 0;
        Quaternion startRot = movablePart.localRotation; // 현재 각도

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            // SmoothStep을 사용하여 부드러운 '가속/감속' 효과 적용
            float t = Mathf.SmoothStep(0.0f, 1.0f, timer / animationDuration);
            // Quaternion.Lerp를 사용하여 각도를 부드럽게 변경
            movablePart.localRotation = Quaternion.Lerp(startRot, targetRot, t); 
            yield return null; 
        }

        movablePart.localRotation = targetRot; // 정확한 목표 각도로 설정
        isAnimating = false;
    }
}
