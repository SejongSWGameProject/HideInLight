using UnityEngine;

/// <summary>
/// 3D 오브젝트(Collider 필요)를 마우스로 클릭했을 때,
/// 지정된 UI 패널(GameObject)을 활성화(SetActive(true))시키는 스크립트입니다.
/// </summary>
public class OpenPanelOnClick : MonoBehaviour
{
    [Header("연결")]
    [Tooltip("클릭했을 때 켜고 싶은 UI 패널 (예: SwitchPuzzle_Panel)")]
    public GameObject panelToOpen;

    /// <summary>
    /// 이 스크립트가 붙은 오브젝트의 Collider가 클릭되었을 때 호출됩니다.
    /// </summary>
    private void OnMouseDown()
    {
        // 1. 패널이 연결되어 있는지 확인
        if (panelToOpen != null)
        {
            // 2. 패널을 켠다 (활성화)
            panelToOpen.SetActive(true);
            Debug.Log(panelToOpen.name + " 패널을 열었습니다.");
        }
        else
        {
            Debug.LogWarning("OpenPanelOnClick: panelToOpen 변수가 연결되지 않았습니다.");
        }
    }
}