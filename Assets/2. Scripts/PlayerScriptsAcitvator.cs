using UnityEngine;

public class PlayerScriptsAcitvator : MonoBehaviour
{
    [Header("Objects to Activate")]
    public GameObject flashlightModel; // A 오브젝트
    public GameObject flashlightScript;    // B 오브젝트
    public GameObject mindUI;
    public GameObject elecPowerUI;
    
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

        isActivated = true;

        Destroy(this.gameObject);

        InteractableObject obj = GetComponent<InteractableObject>();
        obj.canInteract = false;

    }


}
