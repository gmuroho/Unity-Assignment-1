using UnityEngine;

public class ScissorsDebugger : MonoBehaviour
{
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private Vector3 initialLocalScale;

    void Awake()
    {
        // 记录编辑模式下的正确位置
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
        initialLocalScale = transform.localScale;

        Debug.Log($"[剪刀调试] 初始本地坐标已记录: {initialLocalPos}");
    }

    void Update()
    {
        // 1. 检查是否被意外隐藏
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr && !mr.enabled)
        {
            Debug.LogWarning("[剪刀调试] 警告：MeshRenderer 被禁用了！正在重新开启...");
            mr.enabled = true;
        }

        // 2. 检查坐标是否发生剧变
        if (Vector3.Distance(transform.localPosition, initialLocalPos) > 0.01f)
        {
            Debug.LogError($"[剪刀调试] 检测到坐标漂移！当前位置: {transform.localPosition}，正在强制还原...");
            // 强制还原到你缩放好的位置
            transform.localPosition = initialLocalPos;
            transform.localRotation = initialLocalRot;
            transform.localScale = initialLocalScale;
        }

        // 3. 检查层级
        if (gameObject.layer != LayerMask.NameToLayer("Default"))
        {
            // 如果你设置了特殊的 Layer，请确保相机能看到它
            // Debug.Log("[剪刀调试] 当前 Layer 为: " + LayerMask.LayerToName(gameObject.layer));
        }
    }
}