using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("视角控制 (必须赋值)")]
    public Camera playerCamera;      // 拖入你的 Main Camera
    public Transform playerBody;     // 拖入你的 Player 整个物体

    [Header("旋转参数")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("交互设置")]
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    [Header("道具模型")]
    public bool hasShears = false;
    public GameObject shearsInHand;

    void Start()
    {
        // 1. 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. 自动检查并强行赋值
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        if (playerBody == null) playerBody = transform;

        // 3. 预防性检查：如果相机上有刚体，会限制旋转，必须禁用
        Rigidbody camRb = playerCamera.GetComponent<Rigidbody>();
        if (camRb != null)
        {
            camRb.isKinematic = true;
            Debug.Log("警告：已自动禁用相机上的刚体以允许旋转。");
        }

        if (shearsInHand != null) shearsInHand.SetActive(false);
    }

    void Update()
    {
        // 处理上下低头和左右转身
        HandleRotation();

        // 处理捡东西和割草
        HandleInteraction();
    }

    void HandleRotation()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 计算并限制上下旋转角度
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f); // 允许近乎垂直看地

        // 【关键点】直接修改 LocalRotation，无视其他脚本影响
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 左右转动身体
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleInteraction()
    {
        // 屏幕中心发射射线
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            if (Input.GetKeyDown(interactKey))
            {
                // 检查是否是剪刀
                if (hit.collider.name.ToLower().Contains("shears") || hit.collider.CompareTag("Tool"))
                {
                    PickUpShears(hit.collider.gameObject);
                }
            }
        }

        // 割草逻辑
        if (hasShears && Input.GetMouseButtonDown(0))
        {
            PerformTrim();
        }
    }

    void PickUpShears(GameObject obj)
    {
        hasShears = true;
        Destroy(obj);
        if (shearsInHand != null) shearsInHand.SetActive(true);
        Debug.Log("成功获得剪刀！现在可以对着草点左键了。");
    }

    void PerformTrim()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2.5f))
        {
            // 尝试获取草上的脚本并触发割草
            var target = hit.collider.GetComponent<TrimmableObject>();
            if (target != null)
            {
                target.Trim();
            }
        }
    }
}