using UnityEngine;

public class FlareManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetFlare()
    {
        EndingManager.Instance.hasFlare = true;
        InteractableObject interactor = this.GetComponent<InteractableObject>();
        interactor.canInteract = false;
        Destroy(gameObject);
    }
}
