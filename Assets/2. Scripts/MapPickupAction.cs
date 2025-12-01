using UnityEngine;
using System.Collections; // 이거 추가 필수

public class MapPickupAction : MonoBehaviour
{
    public void GetMap()
    {
        StartCoroutine(GetMapRoutine());
    }

    IEnumerator GetMapRoutine()
    {
        MapSystem mapSys = FindAnyObjectByType<MapSystem>();
        
        if (mapSys != null)
        {
            // 1. 획득 처리
            mapSys.GetMapItem(); 
            
            // 2. UI 끄기
            InteractableObject myInteract = GetComponent<InteractableObject>();
            if (myInteract != null) myInteract.SetPromptUI(false);

            // 3. ⭐ 0.1초 대기 (중요!)
            // 플레이어의 Raycast가 "물체가 사라졌음"을 인식할 찰나의 시간을 줍니다.
            // 또한 Collider를 먼저 꺼서 Raycast가 통과하게 만듭니다.
            GetComponent<Collider>().enabled = false; 
            GetComponent<Renderer>().enabled = false; // 눈에도 안 보이게

            yield return new WaitForSeconds(0.1f);

            // 4. 진짜 삭제
            Destroy(gameObject);
        }
    }
}