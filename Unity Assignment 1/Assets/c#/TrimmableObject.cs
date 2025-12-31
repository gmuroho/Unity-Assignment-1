using UnityEngine;
using System.Collections;

public class TrimmableObject : MonoBehaviour
{
    [Header("Visual Feedback")]
    public float cutHeight = 0.2f;    // 割完后的缩放高度
    public Color cutColor = new Color(0.35f, 0.45f, 0.25f);
    public float shrinkSpeed = 5f;    // 缩放平滑度

    private bool isTrimmed = false;
    private Vector3 originalScale;
    private MeshRenderer meshRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 这里的方法名必须是 Trim，以修复 CS1061 报错
    public void Trim()
    {
        if (isTrimmed) return;
        isTrimmed = true;

        // 执行割草后的反馈
        StopAllCoroutines();
        StartCoroutine(TrimAnimation());
    }

    private IEnumerator TrimAnimation()
    {
        // 视觉变色
        if (meshRenderer != null)
        {
            meshRenderer.material.color = cutColor;
        }

        // 平滑缩放高度
        Vector3 targetScale = new Vector3(originalScale.x, cutHeight, originalScale.z);
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * shrinkSpeed);
            yield return null;
        }
        transform.localScale = targetScale;

        // 如果你有 GrassParticleAutoConfig，可以在这里实例化粒子效果
        // Instantiate(particlePrefab, transform.position, Quaternion.identity);
    }
}