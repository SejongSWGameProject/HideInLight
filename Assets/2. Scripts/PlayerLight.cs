using UnityEngine;

public class FlashlightCtrl : MonoBehaviour
{
    public Light flashlight;          // Spot Light ����
    public Transform cameraTransform; // ī�޶� Transform

    [Header("Initial Settings")]
    public float initialRange = 150f;  // Inspector���� �ʱⰪ ����
    public float initialAngle = 45f;   // Inspector���� �ʱⰪ ����

    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;         // ���콺 �� ����
    [Range(0f, 1f)] public float angleRangeFactor = 0.5f; // range ���� �� angle ���� ����

    private float range;
    private float angle;

    void Start()
    {
        if (flashlight == null) return;

        range = initialRange;  // Inspector �� �״�� �ʱⰪ���� ���
        angle = initialAngle;

        flashlight.range = range;
        flashlight.spotAngle = angle;

        flashlight.enabled = false;
    }


    void Update()
    {
        // ���콺 ���� Ŭ�� �� ���
        if (Input.GetMouseButtonDown(0)) // 0 = ���� ��ư
        {
            flashlight.enabled = !flashlight.enabled; // ���������� ����, ���������� ��
        }

        // ���콺 �� �Է� ó��
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // ���� ��ũ��: +, �Ʒ���: -
        if (scroll != 0)
        {
            // range ����/���� (�ּҰ� 50)
            if(angle != 15f)
                range = Mathf.Max(50f, range + scroll * scrollSpeed);

            // spotAngle ����/���� (�ּҰ� 15)
            if (range != 50f)
                angle = Mathf.Max(15f, angle - scroll * scrollSpeed * angleRangeFactor);

            flashlight.range = range;
            flashlight.spotAngle = angle;
        }

        RaycastHit hit;

        // ���� ���� �������� Raycast
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            if (flashlight.enabled)
            {
                // Ư�� �±� ����
                if (hit.collider.CompareTag("Monster"))
                {
                    Debug.Log("���� ���� �޾Ҵ�!");
                }
            }
        }
    }
}
