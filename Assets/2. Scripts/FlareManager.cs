using UnityEngine;

public class FlareManager : MonoBehaviour
{

    public void GetFlare()
    {
        EndingManager.Instance.hasFlare = true;
        InteractableObject interactor = this.GetComponent<InteractableObject>();
        interactor.canInteract = false;
        Destroy(gameObject);
    }
}
