using UnityEngine;

public class GrassPhysicOptimizer : MonoBehaviour
{
    public PhysicsMaterial caodi_wuli;

    void Start()
    {
        Collider pengzhuangqi = GetComponent<Collider>();

        if (pengzhuangqi != null)
        {
            if (caodi_wuli != null)
            {
                pengzhuangqi.material = caodi_wuli;
            }
        }
    }
}