using UnityEngine;

/// <summary>
/// 专门解决剪刀旋转时手柄“飘走”或“位移”的问题。
/// 请将此脚本挂载在 Scissors_Controller 或 Models_Offsets 上。
/// </summary>
public class ScissorHandleFixer : MonoBehaviour
{
    [Header("成组物体")]
    public Transform leftPart;  // 包含左刀片和左手柄的整体
    public Transform rightPart; // 包含右刀片和右手柄的整体

    [Header("旋转设置")]
    [Range(0, 60)] public float maxOpenAngle = 30f;
    public float transitionSpeed = 10f;

    private float _currentAngle = 0f;
    private bool _isOpen = false;

    void Start()
    {
        // 关键检查 1: 检查是否存在非等比缩放
        CheckScale(leftPart);
        CheckScale(rightPart);

        // 关键操作: 记录并锁定初始相对位置
        if (leftPart) ValidateTransform(leftPart);
        if (rightPart) ValidateTransform(rightPart);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) _isOpen = !_isOpen;

        float target = _isOpen ? maxOpenAngle : 0;
        _currentAngle = Mathf.Lerp(_currentAngle, target, Time.deltaTime * transitionSpeed);

        ApplyRigidRotation();
    }

    void ApplyRigidRotation()
    {
        if (!leftPart || !rightPart) return;

        // 使用本地坐标系旋转，不改变 Position
        // 确保你的 Group 节点下，刀片和手柄是合在一起的 Mesh 或子物体
        leftPart.localRotation = Quaternion.Euler(0, 0, _currentAngle);
        rightPart.localRotation = Quaternion.Euler(0, 0, -_currentAngle);
    }

    void CheckScale(Transform t)
    {
        if (t == null) return;
        Vector3 s = t.localScale;
        // 如果缩放不是 (1,1,1) 或者不统一，旋转时远离中心的手柄会产生位移误差
        if (Mathf.Abs(s.x - s.y) > 0.01f || Mathf.Abs(s.y - s.z) > 0.01f)
        {
            Debug.LogWarning($"警告: {t.name} 存在非等比缩放 {s}。这会导致手柄旋转轨迹变成椭圆，产生位移！请在建模软件或Unity中将其重置为 (1,1,1)。");
        }
    }

    void ValidateTransform(Transform t)
    {
        // 强制确保子物体（手柄和刀片）没有被错误的动画脚本独立移动
        foreach (Transform child in t)
        {
            // 如果子物体本身有位移动画，会干扰整体旋转
            if (child.localPosition != Vector3.zero && child.name.Contains("Handle"))
            {
                Debug.Log($"提示: 手柄 {child.name} 相对于轴心有偏移，这是正常的，但请确保旋转过程中它的 localPosition 保持不变。");
            }
        }
    }
}