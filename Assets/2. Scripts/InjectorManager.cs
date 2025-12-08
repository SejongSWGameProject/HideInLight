using UnityEngine;
using UnityEngine.AI;

public class InjectorManager : MonoBehaviour
{

    public void GetInjector()
    {
        EndingManager.Instance.hasInjector = true;

        foreach(MonsterAI m in MonsterAI.allMonsters)
        {
            InteractableObject monsterInteractor = m.GetComponent<InteractableObject>();
            monsterInteractor.canInteract = true;
        }

        InteractableObject injectorInteractor = this.GetComponent<InteractableObject>();
        injectorInteractor.canInteract = false;

        Destroy(gameObject);
    }


}
