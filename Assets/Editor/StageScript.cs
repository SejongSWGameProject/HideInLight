using UnityEngine;
using UnityEditor;

public class AddCollidersAndRenderers : MonoBehaviour
{
    [MenuItem("Tools/Map Utility/Add MeshRenderer + MeshCollider to Children")]
    static void AddToChildren()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("먼저 부모 오브젝트를 선택하세요!");
            return;
        }

        GameObject parent = Selection.activeGameObject;
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>(true);

        int addedCount = 0;

        foreach (MeshFilter mf in meshFilters)
        {
            GameObject go = mf.gameObject;

            // MeshRenderer 추가
            if (go.GetComponent<MeshRenderer>() == null)
                go.AddComponent<MeshRenderer>();

            // MeshCollider 추가
            if (go.GetComponent<MeshCollider>() == null)
            {
                MeshCollider col = go.AddComponent<MeshCollider>();
                col.sharedMesh = mf.sharedMesh;
                col.convex = false;
            }

            addedCount++;
        }

        Debug.Log("오브젝트에 MeshRenderer + MeshCollider 추가 완료!");
    }
}
