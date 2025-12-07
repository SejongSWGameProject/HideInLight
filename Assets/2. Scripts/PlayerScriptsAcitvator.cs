using UnityEngine;

public class PlayerScriptsAcitvator : MonoBehaviour
{
    [Header("Objects to Activate")]
    public GameObject flashlightModel; // A 오브젝트
    public GameObject flashlightScript;    // B 오브젝트
    public GameObject mindUI;
    public GameObject elecPowerUI;
    public GameObject batteryUI;
    public GameObject howtouseUI;
    
    private bool isActivated = false;

    void Start()
    {
        // 둘 다 시작 시 비활성화
        if (flashlightModel != null)
            flashlightModel.SetActive(false);

        if (flashlightScript != null)
            flashlightScript.SetActive(false);

        if(mindUI != null)
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
        if (howtouseUI != null)
        {
            howtouseUI.SetActive(false);
        }
    }

    void Update()
    {
        
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
        if (howtouseUI != null)
        {
            howtouseUI.SetActive(true);
        }

        isActivated = true;

        Destroy(this.gameObject);

        InteractableObject obj = GetComponent<InteractableObject>();
        obj.canInteract = false;

    }


}
