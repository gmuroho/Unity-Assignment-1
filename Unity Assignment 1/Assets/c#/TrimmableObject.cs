using UnityEngine;

public class TrimmableObject : MonoBehaviour
{
    [Header("修剪参数")]
    public float minScaleY = 0.1f;
    public float trimStep = 0.1f;

    [Header("视觉效果")]
    [Tooltip("请确保这个 Prefab 的材质 Shader 是 Unlit 类型，彻底告别黑边")]
    public GameObject leafParticlePrefab;

    private float currentScaleY;

    void Start()
    {
        currentScaleY = transform.localScale.y;

        // 自动对齐：确保模型底部对齐父物体中心，防止缩放时草浮空
        foreach (Transform child in transform)
        {
            // 默认 Cube 高度为 1，将其向上偏移 0.5 使得底部位于 0 点
            child.localPosition = new Vector3(child.localPosition.x, 0.5f, child.localPosition.z);
        }
    }

    public void Trim()
    {
        if (currentScaleY > minScaleY)
        {
            currentScaleY -= trimStep;
            currentScaleY = Mathf.Max(currentScaleY, minScaleY);

            // 物理缩放：父物体变矮，由于子物体对齐了底部，所以只会向下缩短
            transform.localScale = new Vector3(transform.localScale.x, currentScaleY, transform.localScale.z);

            SpawnParticles();
        }
    }

    private void SpawnParticles()
    {
        if (leafParticlePrefab != null)
        {
            // 计算喷发位置：在当前草的顶端
            Vector3 spawnPosition = transform.position + Vector3.up * currentScaleY;

            // 生成粒子
            GameObject effect = Instantiate(leafParticlePrefab, spawnPosition, Quaternion.identity);

            // 关键：将粒子移出父级，防止它跟着草一起变矮变细，导致视觉上的“糊”
            effect.transform.SetParent(null);
            effect.transform.localScale = Vector3.one;

            // 1秒后自动销毁，节省内存
            Destroy(effect, 1.0f);
        }
    }
}