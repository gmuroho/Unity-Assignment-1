using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrassPhysicOptimizer : MonoBehaviour
{
    [Tooltip("0 摩擦力物理材质拖")]
    public PhysicsMaterial zeroFrictionMaterial;

    void Start()
    {
        
        
        Collider col = GetComponent<Collider>();
        if (col != null && zeroFrictionMaterial != null)
        {
            col.material = zeroFrictionMaterial;
        }

        
        
    }
}