using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载在 Models_Offsets 上的剪刀控制器
/// 适配当前 Z 轴旋转层级
/// </summary>
public class ScissorsPivotController : MonoBehaviour
{
    [Header("层级引用")]
    [Tooltip("拖入层级中的 group left")]
    public Transform groupLeft;
    [Tooltip("拖入层级中的 group right")]
    public Transform groupRight;

    [Header("开合角度（相对角度）")]
    [Range(0, 60)] public float openAngle = 30f;   // 张开时单侧偏离中线的角度
    [Range(-10, 10)] public float closeAngle = 0f; // 合拢时单侧的角度（通常为0或微调）

    [Header("动画设置")]
    public float cutSpeed = 0.1f;    // 剪下速度
    public float recoverSpeed = 0.2f; // 回弹速度

    private bool isCutting = false;

    void Start()
    {
        // 安全检查
        if (groupLeft == null || groupRight == null)
        {
            Debug.LogError("请在 Models_Offsets 的脚本槽位中手动拖入 group left 和 group right！");
            return;
        }

        // 初始化到张开状态
        SetScissorsAngle(openAngle);
    }

    void Update()
    {
        // 只有不处于动画中时才能再次触发
        if (Input.GetMouseButtonDown(0) && !isCutting)
        {
            StartCoroutine(PerformCut());
        }
    }

    IEnumerator PerformCut()
    {
        isCutting = true;

        // 1. 合拢阶段
        float elapsed = 0;
        while (elapsed < cutSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cutSpeed;
            // 使用 SmoothStep 让动作更有打击感
            float current = Mathf.Lerp(openAngle, closeAngle, t);
            SetScissorsAngle(current);
            yield return null;
        }
        SetScissorsAngle(closeAngle);

        // 2. 微小停顿
        yield return new WaitForSeconds(0.02f);

        // 3. 回弹阶段
        elapsed = 0;
        while (elapsed < recoverSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoverSpeed;
            float current = Mathf.Lerp(closeAngle, openAngle, t);
            SetScissorsAngle(current);
            yield return null;
        }
        SetScissorsAngle(openAngle);

        isCutting = false;
    }

    /// <summary>
    /// 核心旋转赋值逻辑
    /// </summary>
    void SetScissorsAngle(float angle)
    {
        // 根据你的图片：
        // Left 应该是正角度 (0, 0, angle)
        // Right 应该是负角度 (0, 0, -angle)
        groupLeft.localRotation = Quaternion.Euler(0, 0, angle);
        groupRight.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    // 在编辑器里实时预览
    void OnValidate()
    {
        if (!Application.isPlaying && groupLeft != null && groupRight != null)
        {
            SetScissorsAngle(openAngle);
        }
    }
}