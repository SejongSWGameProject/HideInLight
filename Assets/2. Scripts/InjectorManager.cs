using UnityEngine;
using UnityEngine.AI;

public class InjectorManager : MonoBehaviour
{

    public NavMeshAgent agent;

    public void GetInjector()
    {
        EndingManager.Instance.hasInjector = true;

        InteractableObject monsterInteractor = agent.GetComponent<InteractableObject>();
        monsterInteractor.canInteract = true;

        InteractableObject injectorInteractor = this.GetComponent<InteractableObject>();
        injectorInteractor.canInteract = false;

        Destroy(gameObject);
    }


}
