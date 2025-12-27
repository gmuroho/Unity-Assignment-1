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
        // 调试：在 Scene 窗口画出一根 3 米长的红线，方便查看射线去向
        Debug.DrawRay(playerCamera.position, playerCamera.forward * 3f, Color.red, 1f);

        RaycastHit hit;
        // 射线从相机中心发出
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, 3f))
        {
            Debug.Log($"<color=cyan>[射线命中]</color> 打中了: {hit.collider.name}");

            // 关键：在父级或自身寻找脚本
            TrimmableObject trimmable = hit.collider.GetComponentInParent<TrimmableObject>();

            if (trimmable != null)
            {
                trimmable.Trim();
            }
            else
            {
                Debug.LogWarning("射线打中了物体，但该物体及其父级都没有 TrimmableObject 脚本！");
            }
        }
        else
        {
            Debug.Log("射线未打中任何物体，请靠近一点。");
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

        // 确保手中的工具不会挡住射线
        currentTool.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // 补全缺失的 DropTool 方法，修复 PlayerInteraction 的报错
    public void DropTool()
    {
        if (currentTool == null) return;

        // 恢复工具的物理特性
        Rigidbody rb = currentTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;
        }

        // 解除父子关系
        currentTool.transform.SetParent(null);

        // 恢复 Layer 为 Interactable 确保以后还能捡起来
        currentTool.layer = LayerMask.NameToLayer("Interactable");

        currentTool = null;
    }
}