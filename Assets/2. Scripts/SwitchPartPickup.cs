using UnityEngine;
using System.Collections;

public class SwitchPartPickup : MonoBehaviour
{
    public void PickupPart()
    {
        StartCoroutine(PickupRoutine());
    }

    IEnumerator PickupRoutine()
    {
        // 1. 매니저한테 +1 카운트 하라고 알림
        if (SwitchManager.Instance != null) // (이름 확인: SwitchPartManager면 그걸로 쓰세요)
        {
            SwitchManager.Instance.CollectPart();
        }

        // 2. UI 끄기
        InteractableObject myInteract = GetComponent<InteractableObject>();
        if (myInteract != null) myInteract.SetPromptUI(false);

        // 3. 물리 충돌 끄기 (이건 본체에 있을 테니 그대로 유지)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // ★★★ [수정된 부분] ★★★ 
        // 본체에 Renderer가 없으면 자식들(Children)을 뒤져서 싹 다 끕니다.
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in allRenderers)
        {
            r.enabled = false;
        }

        // 4. 잠깐 기다렸다가 삭제
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}