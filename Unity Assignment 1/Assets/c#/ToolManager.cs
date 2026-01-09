using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [Header("配置")]
    public Transform toolMountPoint;
    public Vector3 holdOffset = new Vector3(0.4f, -0.4f, 0.7f);

    private GameObject currentTool;
    private Transform playerCamera;

    void Start()
    {
        if (toolMountPoint != null) playerCamera = toolMountPoint;
        else
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
                toolMountPoint = playerCamera;
            }
        }
    }

    void Update()
    {
        if (currentTool != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseEquippedTool();
            }
        }
    }

    private void UseEquippedTool()
    {
        
        Debug.DrawRay(playerCamera.position, playerCamera.forward * 3f, Color.red, 1f);

        RaycastHit hit;
        
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 3f))
        {
            Debug.Log($"<color=cyan>[射线]</color> 打中: {hit.collider.name}");

            
            TrimmableObject trimmable = hit.collider.GetComponentInParent<TrimmableObject>();

            if (trimmable != null)
            {
                trimmable.Trim();
            }
            else
            {
                Debug.LogWarning("射线打中了物体，但该物体及其父级都没有 TrimmableObject 脚本");
            }
        }
        else
        {
            Debug.Log("射线未打中任何物体请靠近。");
        }
    }

    public void PickupAndEquipTool(GameObject toolObject)
    {
        if (currentTool != null) return;
        currentTool = toolObject;

        Rigidbody rb = currentTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = false;
        }

        currentTool.transform.SetParent(toolMountPoint);
        currentTool.transform.localPosition = holdOffset;
        currentTool.transform.localRotation = Quaternion.identity;

        
        currentTool.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    
    public void DropTool()
    {
        if (currentTool == null) return;

        
        Rigidbody rb = currentTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;
        }

        
        currentTool.transform.SetParent(null);

        
        currentTool.layer = LayerMask.NameToLayer("Interactable");

        currentTool = null;
    }
}