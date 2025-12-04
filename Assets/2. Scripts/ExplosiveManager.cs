using UnityEngine;

public class ExplosiveManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform player;
    public Transform monster;
    public float triggerDistance = 20.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractableObject interactable = GetComponent<InteractableObject>();
        if(Vector3.Distance(player.position, monster.position) < triggerDistance)
        {
            if (EndingManager.Instance.hasFlare)
            {
                interactable.canInteract = true;

            }
        }
    }
}
