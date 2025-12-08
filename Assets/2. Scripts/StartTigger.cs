using UnityEngine;

public class StartTigger : MonoBehaviour
{
    [Header("Objects to Activate")]
    public GameObject flashlightModel; // A 오브젝트
    public GameObject flashlightScript;    // B 오브젝트
    public GameObject mindUI;
    public GameObject elecPowerUI;
    public GameObject batteryUI;
    public GameObject monster;

    void Start()
    {
        // 둘 다 시작 시 비활성화
        if (flashlightModel != null)
        {
            flashlightModel.SetActive(false);
        }

        if (flashlightScript != null)
        {
            flashlightScript.SetActive(false);
        }

        if (mindUI != null)
        {
            mindUI.SetActive(false);
        }
        if (elecPowerUI != null)
        {
            elecPowerUI.SetActive(false);
        }
        if (batteryUI != null)
        {
            batteryUI.SetActive(false);
        }
        if(monster != null)
        {
            monster.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ActivateObjects();
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        ActivateObjects();
        PlayerMind playerMind = other.GetComponent<PlayerMind>();
        playerMind.isStart = true;
    }

    public void ActivateObjects()
    {
        if (flashlightModel != null)
        {
            flashlightModel.SetActive(true);

        }

        if (flashlightScript != null)
        {
            flashlightScript.SetActive(true);

        }

        if (mindUI != null)
        {
            mindUI.SetActive(true);

        }
        if (elecPowerUI != null)
        {
            elecPowerUI.SetActive(true);
        }
        if (batteryUI != null)
        {
            batteryUI.SetActive(true);
        }
        if (monster != null)
        {
            monster.SetActive(true);
        }
        LampManager.Instance.powerInit();
        Destroy(this.gameObject);
    }

}
