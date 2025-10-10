using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    private Transformer transformerParent;

    void Start()
    {
        transformerParent = GetComponentInParent<Transformer>();
        if (transformerParent == null)
            Debug.LogWarning("PuzzleButton의 부모에서 Transformer를 찾지 못했습니다.");
    }

    // 간단한 방법: OnMouseDown 사용 (Collider 필요)
    void OnMouseDown()
    {
        if (transformerParent != null)
        {
            transformerParent.Interact();
        }
    }
}
