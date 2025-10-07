using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    
    private Transformer transformerParent;

    void Start()
    {
        
        transformerParent = GetComponentInParent<Transformer>();
    }

    
    public void OnButtonClicked()
    {
       
        if (transformerParent != null)
        {
            transformerParent.Interact();
        }
        else
        {
            Debug.LogError("버튼의 부모에게서 Transformer 스크립트를 찾을 수 없습니다!");
        }
    }
}