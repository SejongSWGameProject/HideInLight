using UnityEngine;

public class Transformer : MonoBehaviour
{
    public GameObject puzzlePanel; 

    public void Interact()
    {
        Debug.Log("변압기와 상호작용!");
        puzzlePanel.SetActive(true); 
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}