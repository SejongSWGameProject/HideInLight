using UnityEngine;

public class FlashlightCtrl : MonoBehaviour
{
    public Light flashlight;          // Spot Light 연결
    public Transform cameraTransform; // 카메라 Transform

    [SerializeField] private float flashlightAngular = 30F;
    [SerializeField] private float flashlightRange = 20F;

    void Start()
    {
        //flashlight.range = flashlightRange;
        //flashlight.spotAngle = flashlightAngular;
        flashlight.enabled = false; // 시작할 때 손전등 끄기
    }

    private void LateUpdate()
    {
        // 카메라 위치와 회전에 맞춰 손전등 이동
        flashlight.transform.position = cameraTransform.position;
        flashlight.transform.rotation = cameraTransform.rotation;
    }

    void Update()
    {
        // 마우스 왼쪽 클릭 시 토글
        if (Input.GetMouseButtonDown(0)) // 0 = 왼쪽 버튼
        {
            flashlight.enabled = !flashlight.enabled; // 켜져있으면 끄고, 꺼져있으면 켬
        }

        
    }
}
