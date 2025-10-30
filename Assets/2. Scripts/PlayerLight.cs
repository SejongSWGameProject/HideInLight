using UnityEngine;

public class FlashlightCtrl : MonoBehaviour
{
    public Light flashlight;          // Spot Light ����
    public Transform cameraTransform; // ī�޶� Transform

    [SerializeField] private float flashlightAngular = 30F;
    [SerializeField] private float flashlightRange = 20F;

    void Start()
    {
        //flashlight.range = flashlightRange;
        //flashlight.spotAngle = flashlightAngular;
        flashlight.enabled = false; // ������ �� ������ ����
    }

    private void LateUpdate()
    {
        // ī�޶� ��ġ�� ȸ���� ���� ������ �̵�
        flashlight.transform.position = cameraTransform.position;
        flashlight.transform.rotation = cameraTransform.rotation;
    }

    void Update()
    {
        // ���콺 ���� Ŭ�� �� ���
        if (Input.GetMouseButtonDown(0)) // 0 = ���� ��ư
        {
            flashlight.enabled = !flashlight.enabled; // ���������� ����, ���������� ��
        }

        
    }
}
