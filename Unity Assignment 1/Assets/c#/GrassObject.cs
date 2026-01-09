using UnityEngine;

public class GrassObject : MonoBehaviour
{
    public void OnCut()
    {
        
        
        Object.FindFirstObjectByType<GameProgressManager>().OnGrassCut();
        Destroy(gameObject); 
    }
}