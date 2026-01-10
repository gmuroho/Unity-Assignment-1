using UnityEngine;
using System.Collections;

public class TrimmableObject : MonoBehaviour
{
    [Header("Visual Feedback")]
    public float cutHeight = 0.2f;
    public Color cutColor = new Color(0.35f, 0.45f, 0.25f);
    public float shrinkSpeed = 5f;

    private bool isTrimmed = false;
    private Vector3 originalScale;
    private MeshRenderer meshRenderer;

    private bool startShrinking = false;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (startShrinking == true)
        {
            float dist = Vector3.Distance(transform.localScale, targetScale);
            if (dist > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * shrinkSpeed);
            }
            else
            {
                transform.localScale = targetScale;
                startShrinking = false;
            }
        }
    }

    public void Trim()
    {
        if (isTrimmed == false)
        {
            isTrimmed = true;

            if (meshRenderer != null)
            {
                meshRenderer.material.color = cutColor;
            }

            targetScale = new Vector3(originalScale.x, cutHeight, originalScale.z);
            startShrinking = true;
        }
    }
}