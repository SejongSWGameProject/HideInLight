using UnityEngine;

public class PlayerLockControl : MonoBehaviour
{
    [Header("제어할 스크립트 연결")]
    [Tooltip("플레이어 이동 스크립트 (예: PlayerMovement)")]
    public MonoBehaviour movementScript; 

    [Tooltip("카메라 회전 스크립트 (예: MouseLook, CameraController)")]
    public MonoBehaviour cameraScript;

    // 퍼즐 시작할 때 부를 함수
    public void FreezePlayer()
    {
        // 1. 이동/회전 스크립트 끄기
        if (movementScript != null) movementScript.enabled = false;
        if (cameraScript != null) cameraScript.enabled = false;

        // 2. 마우스 커서 보이게 하고 가두기 풀기 (퍼즐 클릭해야 하니까)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 퍼즐 끝날 때 부를 함수
    public void UnfreezePlayer()
    {
        // 1. 이동/회전 스크립트 다시 켜기
        if (movementScript != null) movementScript.enabled = true;
        if (cameraScript != null) cameraScript.enabled = true;

        // 2. 마우스 커서 다시 숨기고 잠그기 (게임 플레이로 복귀)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}