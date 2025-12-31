using UnityEngine;

/// <summary>
/// 最终版草丛配置工具：支持无限层级递归，自动识别网格物体
/// 挂载在最外层的 grasspivotroot 上执行即可
/// </summary>
public class GrassSetupPro : MonoBehaviour
{
    [Header("设置")]
    public string targetLayer = "Grass";
    public bool addMeshCollider = true;
    public bool cleanupOldComponents = true;

    [ContextMenu("Execute Professional Setup")]
    public void Setup()
    {
        int layer = LayerMask.NameToLayer(targetLayer);
        if (layer == -1)
        {
            Debug.LogError($"[GrassSetup] 找不到层级: {targetLayer}。请点击编辑器右上角 Layers -> Add Layer 手动添加。");
            return;
        }

        int totalCount = 0;
        ProcessNode(transform, layer, ref totalCount);

        Debug.Log($"[GrassSetup] 配置完成！共处理了 {totalCount} 个草体。");
    }

    private void ProcessNode(Transform current, int layer, ref int count)
    {
        // 只有带有网格的物体才被认为是“草”
        bool isActualGrass = current.GetComponent<MeshFilter>() != null;

        if (isActualGrass)
        {
            // 1. 设置层级
            current.gameObject.layer = layer;

            // 2. 清理可能存在的旧组件（可选），确保状态干净
            if (cleanupOldComponents)
            {
                var oldCols = current.GetComponents<Collider>();
                foreach (var c in oldCols) if (!(c is MeshCollider)) DestroyImmediate(c);
            }

            // 3. 添加/获取 MeshCollider
            if (addMeshCollider && !current.GetComponent<MeshCollider>())
            {
                current.gameObject.AddComponent<MeshCollider>();
            }

            // 4. 添加割草逻辑脚本
            if (!current.GetComponent<TrimmableObject>())
            {
                current.gameObject.AddComponent<TrimmableObject>();
            }

            count++;
        }

        // 递归处理所有子物体
        for (int i = 0; i < current.childCount; i++)
        {
            ProcessNode(current.GetChild(i), layer, ref count);
        }
    }
}