using UnityEngine;

public class FlashlightCtrl : MonoBehaviour
{
    public Light flashlight;          // Spot Light ����
    public Transform cameraTransform; // ī�޶� Transform

    void Start()
    {
        flashlight.enabled = false; // ������ �� ������ ����
    }

    void Update()
    {
        // ���콺 ���� Ŭ�� �� ���
        if (Input.GetMouseButtonDown(0)) // 0 = ���� ��ư
        {
            flashlight.enabled = !flashlight.enabled; // ���������� ����, ���������� ��
        }

        // ī�޶� ��ġ�� ȸ���� ���� ������ �̵�
        flashlight.transform.position = cameraTransform.position;
        flashlight.transform.rotation = cameraTransform.rotation;
    }
}
