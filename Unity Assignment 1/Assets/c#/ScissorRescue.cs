using UnityEngine;

public class ScissorRescue : MonoBehaviour
{
    [Header("--- 核心组件 ---")]
    // 左右轴心点，确保两手同时控制开合
    public Transform leftPivot;
    public Transform rightPivot;

    [Header("--- 音效组件 ---")]
    public AudioSource cutSound;
    public AudioSource openSound;

    [Header("--- 设置 ---")]
    public float openAngle = 30f;
    public float animationSpeed = 12f;
    public float interactDistance = 3f;

    [Header("--- 视角偏移 (双手平举正中) ---")]
    // X=0 居中, Y=-0.5 略微靠下, Z=1.2 保证能看到整个大剪刀
    public Vector3 heldPosition = new Vector3(0f, -0.5f, 1.2f);
    public Vector3 heldRotation = new Vector3(0f, 0f, 0f);

    private bool isPickedUp = false;
    private bool isCutting = false;
    private float currentAngle = 30f;
    private float targetAngle = 30f;

    void Start()
    {
        currentAngle = openAngle;
        targetAngle = openAngle;
    }

    void Update()
    {
        if (!isPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                TryPickUp();
            }
            return;
        }

        // 剪动逻辑
        if (Input.GetMouseButtonDown(0) && !isCutting)
        {
            StartCut();
        }

        // 平滑开合动画
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * animationSpeed);

        // 双手控制：左右柄同步对称旋转
        if (leftPivot) leftPivot.localRotation = Quaternion.Euler(0, currentAngle, 0);
        if (rightPivot) rightPivot.localRotation = Quaternion.Euler(0, -currentAngle, 0);

        // 自动弹开
        if (isCutting && currentAngle < 1f)
        {
            targetAngle = openAngle;
            if (openSound) openSound.Play();
            isCutting = false;
        }
    }

    void TryPickUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 确保点击的是大剪刀本身或子物体
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                PickUp();
            }
        }
    }

    void PickUp()
    {
        isPickedUp = true;

        // 挂载到相机，成为第一人称视角物体
        transform.SetParent(Camera.main.transform);

        // 【关键修改】：将剪刀放在屏幕正中间，Z轴拉远以看到全貌
        transform.localPosition = heldPosition;
        transform.localRotation = Quaternion.Euler(heldRotation);

        // 物理清理
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Debug.Log("园艺大剪刀已双手就绪！");
    }

    void StartCut()
    {
        isCutting = true;
        targetAngle = 0f;
        if (cutSound) cutSound.Play();
    }
}