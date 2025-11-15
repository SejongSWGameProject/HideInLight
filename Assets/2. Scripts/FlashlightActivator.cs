using UnityEngine;

public class FlashlightActivator : MonoBehaviour
{
    [Header("Objects to Activate")]
    public GameObject flashlightModel; // A 오브젝트
    public GameObject flashlightScript;    // B 오브젝트

    private bool isActivated = false;

    void Start()
    {
        // 둘 다 시작 시 비활성화
        if (flashlightModel != null)
            flashlightModel.SetActive(false);

        if (flashlightScript != null)
            flashlightScript.SetActive(false);
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
        if (flashlightModel != null)
        {
            flashlightModel.SetActive(true);
            
        }

        if (flashlightScript != null)
        {
            flashlightScript.SetActive(true);

        }


        isActivated = true;
    }
}
