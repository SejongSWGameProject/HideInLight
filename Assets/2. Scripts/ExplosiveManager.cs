using UnityEngine;

public class ExplosiveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ExitManager exitManager;

    // Update is called once per frame
    void Update()
    {
        if (EndingManager.Instance.hasFlare && exitManager.isOpen)
        {
            InteractableObject interactable = gameObject.GetComponent<InteractableObject>();
            interactable.canInteract = true;
        }
    }
}
