using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置 (如果面板找不到，直接看这里)")]
    public float moveSpeed = 8f;         // 稍微调高了速度
    public float lookSensitivity = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.2f;

    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 movementInput;
    private float rotationX = 0f;

    void Start()
    {
        // 1. 获取并强制初始化 Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>(); // 如果没有就强行加一个
            Debug.LogWarning("PlayerMovement: 自动添加了缺失的 Rigidbody。");
        }

        // --- 核心：在这里强制设置你找不到的参数 ---
        rb.linearDamping = 0f;                          // 阻力设为0，防止粘在原地
        rb.angularDamping = 0.05f;
        rb.useGravity = true;                  // 开启重力
        rb.isKinematic = false;                // 必须关闭 Kinematic 才能受力移动
        rb.freezeRotation = true;              // 强制锁定旋转，防止摔倒
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 防止穿模

        // 2. 获取相机
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null) playerCamera = Camera.main;

        // 3. 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (rb == null) return;

        // 获取输入
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(x, 0f, z).normalized;

        // 视角旋转
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (playerCamera != null)
        {
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        // 跳跃
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (rb == null || movementInput.magnitude < 0.01f)
        {
            // 如果没按键，摩擦力会自然停下，或者这里手动清除水平速度
            if (movementInput.magnitude < 0.01f && IsGrounded())
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            return;
        }

        // 计算移动方向
        Vector3 moveDir = transform.TransformDirection(movementInput);
        Vector3 targetVelocity = moveDir * moveSpeed;

        // 应用速度改变
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z));

        // 限制最大推力
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    bool IsGrounded()
    {
        // 射线起始点稍微上移，向下检测
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
    }
}