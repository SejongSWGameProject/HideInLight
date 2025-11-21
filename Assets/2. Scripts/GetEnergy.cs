using UnityEngine;
using UnityEngine.UI;

public class GetEnergy : MonoBehaviour
{
    [Header("UI 증가 설정")]
    [SerializeField] private float increaseAmount = 30f; // Inspector에서 조절 가능

    private bool isPlayerInRange = false;
    public GameObject gaugeUI; // Inspector에서 드래그해서 연결

    void Start()
    {
        if (gaugeUI == null)
        {
            Debug.LogWarning("Gauge UI가 연결되지 않았습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("아이템 앞에 있음. F키로 습득 가능");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickupItem();
        }
    }

    public void PickupItem()
    {
        if (gaugeUI != null)
        {
            RectTransform rt = gaugeUI.GetComponent<RectTransform>();
            if (rt != null)
            {
                Vector2 size = rt.sizeDelta;
                Vector3 pos = rt.localPosition;

                float a = size.x;
                // Inspector에서 설정한 증가량 사용
                size.x += increaseAmount;

                // 최대 100까지만 증가
                if (size.x > 100f)
                    size.x = 100f;

                float b = size.x - a;
                // 오른쪽으로 이동: 증가한 크기의 절반
                pos.x += b / 2f;

                // 적용
                rt.sizeDelta = size;
                rt.localPosition = pos;
            }
        }

        // 아이템 제거
        Destroy(gameObject);
        Debug.Log($"아이템 습득! 오른쪽으로 {increaseAmount} 증가, 최대 100까지 적용됨");
    }
}