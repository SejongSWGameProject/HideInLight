using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ExitManager : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public bool isOpen = false;
    public AudioClip moveSound;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenExit();
        }
    }

    public void OpenExit()
    {
        Vector3 closedLeftDoorPos = leftDoor.position;
        Vector3 openedLeftDoorPos = new Vector3(closedLeftDoorPos.x+30,closedLeftDoorPos.y,closedLeftDoorPos.z);

        Vector3 closedRightDoorPos = rightDoor.position;
        Vector3 openedRightDoorPos = new Vector3(closedRightDoorPos.x - 30, closedRightDoorPos.y, closedRightDoorPos.z);

        leftDoor.position = openedLeftDoorPos;
        rightDoor.position = openedRightDoorPos;

        if (moveSound != null) audioSource.PlayOneShot(moveSound);

    }
}
