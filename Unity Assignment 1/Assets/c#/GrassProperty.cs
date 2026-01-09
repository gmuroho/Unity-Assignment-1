using UnityEngine;

public class GrassProperty : MonoBehaviour
{
    [Header("修剪状态")]
    public int health = 2;               
    public float heightReduction = 0.4f; 

    private Collider grassCollider;
    private Vector3 originalScale;

    
    [Header("可选：模型引用(若不使用缩放而想换模型时使用)")]
    public GameObject grassModel_High;
    public GameObject grassModel_Low;

    void Awake()
    {
        
        grassCollider = GetComponent<Collider>();
        originalScale = transform.localScale;
    }

    
    
    
    public void Cut()
    {
        if (health <= 0) return;

        health--;

        
        transform.localScale = new Vector3(
            transform.localScale.x,
            transform.localScale.y - (originalScale.y * heightReduction),
            transform.localScale.z
        );

        
        if (health == 1)
        {
            if (grassModel_High != null) grassModel_High.SetActive(false);
            if (grassModel_Low != null) grassModel_Low.SetActive(true);
        }

        
        if (health <= 0)
        {
            RemoveCollision();

            
            
            GameProgressManager manager = Object.FindFirstObjectByType<GameProgressManager>();
            if (manager != null)
            {
                manager.OnGrassCut();
            }
        }
    }

    private void RemoveCollision()
    {
        
        if (grassCollider != null)
        {
            grassCollider.enabled = false;
        }

        
        transform.localScale = new Vector3(transform.localScale.x, 0.05f, transform.localScale.z);

        
        if (grassModel_Low != null) grassModel_Low.SetActive(false);
    }
}