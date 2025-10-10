using UnityEngine;

public class ButtonClickTest : MonoBehaviour
{
    // 이 OnMouseDown 함수는 Collider가 붙어있는 오브젝트를
    // 마우스로 클릭했을 때 자동으로 호출됩니다.
    void OnMouseDown()
    {
        // 콘솔 창에 "클릭됨" 메시지를 출력합니다.
        Debug.Log("클릭됨");
    }
}