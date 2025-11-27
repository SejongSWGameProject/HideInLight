using UnityEngine;

public class MapPickupAction : MonoBehaviour
{
    public void GetMap()
    {
        MapSystem mapSys = FindAnyObjectByType<MapSystem>();
        
        if (mapSys != null)
        {
            mapSys.GetMapItem(); 
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("맵 시스템을 찾을 수 없습니다! Player에 MapSystem이 있나요?");
        }
    }
}