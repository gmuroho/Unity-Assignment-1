using UnityEngine;
using System.Collections;

// ==========================================
// 部分 1: 挂在剪刀上的检测逻辑
// ==========================================
public class GrassMower : MonoBehaviour
{
    [Header("--- 剪草设置 ---")]
    public float mowRadius = 0.5f;       // 剪开时的判定范围
    public LayerMask grassLayer;         // 草所在的层级
    public Transform cutPoint;           // 剪刀尖端位置（判定中心）

    // 这个方法由 ScissorRescue.cs 中的 StartCut 逻辑调用，或者在剪刀闭合动画达到 0 度时触发
    public void PerformMow()
    {
        if (cutPoint == null) cutPoint = transform;

        // 检测范围内的所有草
        Collider[] hitGrass = Physics.OverlapSphere(cutPoint.position, mowRadius, grassLayer);

        foreach (var grass in hitGrass)
        {
            MowableGrass target = grass.GetComponent<MowableGrass>();
            if (target != null)
            {
                // 计算飞散方向：从剪刀中心向外飞
                Vector3 blastDirection = (grass.transform.position - cutPoint.position).normalized;
                blastDirection += Vector3.up * 0.5f; // 给一点向上的力
                target.OnMown(blastDirection);
            }
        }
    }
}

// ==========================================
// 部分 2: 挂在草预制件上的表现逻辑
// ==========================================
public class MowableGrass : MonoBehaviour
{
    [Header("--- 视觉效果 ---")]
    public GameObject grassParticlePrefab; // 剪断时的绿色叶片粒子
    public GameObject cutGrassModel;      // 剪断后掉落的残骸模型（如果有）

    [Header("--- 物理飞散 ---")]
    public float flyForce = 5f;
    public float torqueForce = 10f;       // 旋转力度，让碎片在空中翻滚

    private bool isCut = false;

    public void OnMown(Vector3 direction)
    {
        if (isCut) return;
        isCut = true;

        // 1. 播放粒子效果
        if (grassParticlePrefab != null)
        {
            Instantiate(grassParticlePrefab, transform.position, Quaternion.identity);
        }

        // 2. 物理飞散处理
        // 如果草本身没有 Rigidbody，我们动态添加一个，或者激活它的残骸
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // 取消固定，启用物理
        rb.isKinematic = false;
        rb.useGravity = true;

        // 施加冲量让它飞出去
        rb.AddForce(direction * flyForce, ForceMode.Impulse);
        // 施加随机扭矩让它在空中乱转
        rb.AddTorque(new Vector3(Random.value, Random.value, Random.value) * torqueForce, ForceMode.Impulse);

        // 3. 碰撞体处理：防止草掉到地底下
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false; // 变成实体，可以撞地
        }

        // 4. 自动清理：3秒后消失
        Destroy(gameObject, 3f);
    }
}