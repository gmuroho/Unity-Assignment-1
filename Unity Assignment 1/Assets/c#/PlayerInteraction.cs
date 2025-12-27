using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("交互配置")]
    public LayerMask interactableMask;
    public float interactionDistance = 3f;

    [Header("持握位置微调")]
    public Vector3 holdOffset = new Vector3(0.5f, -0.5f, 1.0f);
    public Vector3 holdRotation = Vector3.zero;

    private ToolManager toolManager;
    private Transform playerCamera;
    private GameObject currentEquippedTool; // 记录当前拿在手里的物体

    void Start()
    {
        toolManager = GetComponent<ToolManager>();

        // 获取相机
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null) playerCamera = cam.transform;
    }

    void Update()
    {
        if (playerCamera == null) return;

        // --- 核心修复：强制跟随逻辑 ---
        // 如果手里有工具，每一帧都强制把它拉回到相机前方的固定位置
        if (currentEquippedTool != null)
        {
            currentEquippedTool.transform.position = playerCamera.TransformPoint(holdOffset);
            currentEquippedTool.transform.rotation = playerCamera.rotation * Quaternion.Euler(holdRotation);
        }

        RaycastHit hit;
        bool isHit = Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance, interactableMask);

        // 调试射线
        Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionDistance, isHit ? Color.green : Color.red);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (toolManager == null) return;

            // 情况 A：放下
            if (currentEquippedTool != null)
            {
                DropCurrentTool();
                return;
            }

            // 情况 B：拾取
            if (isHit && hit.collider != null)
            {
                PickupTarget(hit.collider.gameObject);
            }
        }
    }

    private void PickupTarget(GameObject obj)
    {
        GameObject target = GetInteractableRoot(obj);

        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.detectCollisions = false; // 拿起时关闭碰撞，防止把玩家弹飞
        }

        // 告知 ToolManager
        toolManager.PickupAndEquipTool(target);

        // 记录引用，触发 Update 中的强制跟随
        currentEquippedTool = target;

        // 设置父级（双重保险）
        target.transform.SetParent(playerCamera);
    }

    private void DropCurrentTool()
    {
        if (currentEquippedTool == null) return;

        Rigidbody rb = currentEquippedTool.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;
        }

        toolManager.DropTool();
        currentEquippedTool.transform.SetParent(null);
        currentEquippedTool = null;
    }

    private GameObject GetInteractableRoot(GameObject obj)
    {
        Transform current = obj.transform;
        GameObject lastValid = obj;

        while (current.parent != null)
        {
            if (((1 << current.parent.gameObject.layer) & interactableMask) != 0)
            {
                lastValid = current.parent.gameObject;
                current = current.parent;
            }
            else
            {
                break;
            }
        }
        return lastValid;
    }
}