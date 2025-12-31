using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GrunnPlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("地面检测")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        // 获取 CharacterController 组件
        controller = GetComponent<CharacterController>();

        // 自动检查引用
        if (groundCheck == null)
        {
            Debug.LogError("错误：没有给 'Ground Check' 赋值！请在 Player 下创建一个空物体并拖入。");
        }
    }

    void Update()
    {
        // 1. 地面检测
        // 确保你的地面物体的 Layer 设置为了 "Ground" (或者你在面板里选中的层)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            // 微微给一点向下的力，确保角色贴地，但不要太大防止卡进地板
            velocity.y = -2f;
        }

        // 2. 获取输入 (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 计算移动方向 (相对于角色自身的朝向)
        Vector3 move = transform.right * x + transform.forward * z;

        // 3. 执行移动
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 4. 处理跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. 应用重力
        velocity.y += gravity * Time.deltaTime;

        // 6. 执行重力位移
        controller.Move(velocity * Time.deltaTime);
    }
}