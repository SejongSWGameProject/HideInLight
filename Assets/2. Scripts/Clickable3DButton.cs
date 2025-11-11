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
    private Quaternion upRotationQ;         // '올라온' 목표 각도
    private Quaternion downRotationQ;       // '눌린' 목표 각도
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

        // ▼▼▼ [디버그 1] ▼▼▼
        if (targetToggle == null)
        {
            Debug.LogError($"[Clickable3DButton] '{gameObject.name}': 'Target Toggle'이 비어있습니다! 클릭이 작동하지 않습니다.", this);
        }
        else
        {
            Debug.Log($"[Clickable3DButton] '{gameObject.name}': OnEnable. 'Target Toggle'({targetToggle.name}) 연결됨.", this);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimating)
        {
            // ▼▼▼ [디버그 2] ▼▼▼
            // Debug.Log("[Clickable3DButton] 마우스 왼쪽 버튼 클릭 감지! Raycast를 시작합니다."); // 로그가 너무 많이 찍혀서 주석 처리
            HandleClickRaycast();
        }
    }

    private void HandleClickRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!hasRotationBeenSet)
        {
            // ▼▼▼ [디버그 3] ▼▼▼
            Debug.LogWarning("[Clickable3DButton] 각도(Rotation)가 아직 설정되지 않아 Raycast를 무시합니다.");
            return;
        }

        if (Physics.Raycast(ray, out hit, 50f, clickableLayerMask))
        {
            // ▼▼▼ [디버그 4] ▼▼▼
            // Debug.Log($"[Clickable3DButton] Raycast가 '{hit.collider.gameObject.name}' 오브젝트에 맞았습니다!"); // 로그가 너무 많이 찍혀서 주석 처리

            if (hit.collider.gameObject == this.gameObject)
            {
                // ▼▼▼ [디버그 5 - 성공!] ▼▼▼
                Debug.Log($"<color=green>[클릭 성공 - Raycast] {gameObject.name}이 감지되었습니다!</color>");

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
            else
            {
                // ▼▼▼ [디버그 6] ▼▼▼
                // Debug.LogWarning($"[Clickable3DButton] Raycast가 이 오브젝트({this.gameObject.name})가 아닌 다른 오브젝트({hit.collider.gameObject.name})에 맞았습니다. (LayerMask 확인 필요)"); // 로그가 너무 많이 찍혀서 주석 처리
            }
        }
        else
        {
            // ▼▼▼ [디버그 7] ▼▼▼
            // Debug.Log("<color=orange>[Clickable3DButton] Raycast가 아무것에도 맞지 않았습니다. (Collider 또는 LayerMask 문제)</color>"); // 로그가 너무 많이 찍혀서 주석 처리
        }
    }

    // 애니메이션 시작 함수
    private void StartAnimation(bool isNowOn)
    {
        // 목표 각도를 설정 (켜졌으면 Down, 꺼졌으면 Up)
        Quaternion targetRotation = isNowOn ? downRotationQ : upRotationQ;
        StopAllCoroutines();

        // ▼▼▼ [오류 수정] ▼▼▼
        // 코루틴을 시작하기 직전에, 이 게임 오브젝트가 여전히 활성화 상태인지 확인합니다.
        // (targetToggle.isOn 변경으로 인해 이 오브젝트가 비활성화되었을 수 있습니다.)
        if (gameObject.activeInHierarchy)
        {
            // 활성화 상태일 때만 코루틴을 시작합니다.
            StartCoroutine(AnimateButtonRotation(targetRotation));
        }
        else
        {
            // 이미 비활성화되었다면, 코루틴을 시작하는 대신 최종 각도로 즉시 설정합니다.
            // (애니메이션은 실행되지 않지만, 다음 활성화 시 올바른 상태를 보장합니다.)
            movablePart.localRotation = targetRotation;
            Debug.LogWarning($"[Clickable3DButton] '{gameObject.name}'이(가) 코루틴을 시작하기 전에 비활성화되었습니다. 애니메이션을 건너뛰고 최종 각도로 설정합니다.", this);
        }
        // ▲▲▲ [오류 수정 끝] ▲▲▲
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