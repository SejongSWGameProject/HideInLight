using UnityEngine;

public class FlashlightActivator : MonoBehaviour
{
    [Header("Objects to Activate")]
    public GameObject flashlight; // A 오브젝트
    public GameObject objectB;    // B 오브젝트

    private bool isActivated = false;

    void Start()
    {
        // 둘 다 시작 시 비활성화
        if (flashlight != null)
            flashlight.SetActive(false);

        if (objectB != null)
            objectB.SetActive(false);
    }

    void Update()
    {
        // 스페이스바 한 번만 입력 처리
        if (!isActivated && Input.GetKeyDown(KeyCode.Space))
        {
            ActivateObjects();
        }
    }

    void ActivateObjects()
    {
        if (flashlight != null)
        {
            flashlight.SetActive(true);
            
        }

        if (objectB != null)
        {
            objectB.SetActive(true);

        }


        isActivated = true;
    }
}
