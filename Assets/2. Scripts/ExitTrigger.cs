using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    public UISaveController uiSaver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiSaver.SaveUIState();
            //NextStage();
            Debug.Log("스테이지 클리어!");
            SceneManager.LoadScene("SecondStage");
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
