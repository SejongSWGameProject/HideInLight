using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    [Header("연출 매니저 연결")]
    public IntroSequence introManager; // 아까 만든 연출 스크립트를 여기에 연결

    [Header("마우스 효과")]
    public Color hoverColor = Color.red;
    private Color originalColor;
    private Material myMaterial;
    private bool isClicked = false; // 중복 클릭 방지

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            myMaterial = rend.material;
            originalColor = myMaterial.GetColor("_EmissiveColor");
        }
    }

    void OnMouseEnter()
    {
        if (!isClicked && myMaterial != null) // 클릭된 후에는 색 안 변함
        {
            myMaterial.SetColor("_EmissiveColor", hoverColor * 10f);
        }
    }

    void OnMouseExit()
    {
        if (!isClicked && myMaterial != null)
        {
            myMaterial.SetColor("_EmissiveColor", originalColor);
        }
    }

    void OnMouseDown()
    {
        if (isClicked) return; // 이미 눌렀으면 무시

        isClicked = true; // 눌렀다고 표시
        
        // 연출 담당자에게 "게임 시작해!"라고 명령
        if (introManager != null)
        {
            introManager.StartGameSequence();
        }
    }
}