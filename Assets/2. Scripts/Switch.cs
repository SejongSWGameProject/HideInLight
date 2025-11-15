using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Switch : MonoBehaviour
{
    [Header("현재 상태")]
    public bool isOn = false; 

    [Header("회전 설정")]
    public GameObject handleObject; 
    public Vector3 offRotation; 
    public Vector3 onRotation;
    public float rotateSpeed = 15f; 

    [HideInInspector]
    public UnityEvent OnToggled; 

    private Coroutine moveCoroutine; 

    void Start()
    {
        if(handleObject != null)
        {
            handleObject.transform.localEulerAngles = isOn ? onRotation : offRotation;
        }
    }

    // OnMouseDown()은 여기서 삭제되었습니다!

    // 컨트롤러가 "너 눌렸어!" 하고 강제로 호출해줄 함수
    public void Toggle()
    {
        // 클릭 감지 로그 (컨트롤러가 대신 띄워줄 겁니다)
        Debug.Log("Toggle() 실행됨! 이름: " + this.gameObject.name);

        isOn = !isOn; 
        
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(RotateHandleCoroutine());
        
        if (OnToggled != null) OnToggled.Invoke();
    }

    IEnumerator RotateHandleCoroutine()
    {
        Quaternion targetRot = Quaternion.Euler(isOn ? onRotation : offRotation);
        Quaternion startRot = handleObject.transform.localRotation;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotateSpeed;
            handleObject.transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        
        handleObject.transform.localRotation = targetRot;
    }
}