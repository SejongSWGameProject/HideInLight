using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlopeStabilizer : MonoBehaviour
{
    private Rigidbody rb;
    public float stickToGroundForce = 20f;
    public float maxSlopeAngle = 60f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 플레이어 바로 아래로 Ray 쏴서 지면 감지
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < maxSlopeAngle)
            {
                // 살짝 눌러주는 힘 (항상 유지)
                rb.AddForce(-hit.normal * stickToGroundForce, ForceMode.Acceleration);
            }
        }
    }
}
