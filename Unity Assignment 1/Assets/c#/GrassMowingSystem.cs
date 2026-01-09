using UnityEngine;
using System.Collections;




public class GrassMower : MonoBehaviour
{
    [Header("ºÙ≤›…Ë÷√")]
    public float mowRadius = 0.5f;       
    public LayerMask grassLayer;         
    public Transform cutPoint;           

    
    public void PerformMow()
    {
        if (cutPoint == null) cutPoint = transform;

        
        Collider[] hitGrass = Physics.OverlapSphere(cutPoint.position, mowRadius, grassLayer);

        foreach (var grass in hitGrass)
        {
            MowableGrass target = grass.GetComponent<MowableGrass>();
            if (target != null)
            {
                
                Vector3 blastDirection = (grass.transform.position - cutPoint.position).normalized;
                blastDirection += Vector3.up * 0.5f; 
                target.OnMown(blastDirection);
            }
        }
    }
}




public class MowableGrass : MonoBehaviour
{
    [Header(" ”–ß")]
    public GameObject grassParticlePrefab; 
    public GameObject cutGrassModel;      

    [Header("≤›∑……¢")]
    public float flyForce = 5f;
    public float torqueForce = 10f;       

    private bool isCut = false;

    public void OnMown(Vector3 direction)
    {
        if (isCut) return;
        isCut = true;

        
        if (grassParticlePrefab != null)
        {
            Instantiate(grassParticlePrefab, transform.position, Quaternion.identity);
        }

        
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        
        rb.isKinematic = false;
        rb.useGravity = true;

        
        rb.AddForce(direction * flyForce, ForceMode.Impulse);
        
        rb.AddTorque(new Vector3(Random.value, Random.value, Random.value) * torqueForce, ForceMode.Impulse);

        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false; 
        }

        
        Destroy(gameObject, 3f);
    }
}