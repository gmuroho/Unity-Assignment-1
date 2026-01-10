using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GrunnPlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jump_h = 2f;

    public Transform di_mian;
    public float check_dis = 0.4f;
    public LayerMask groundMask;

    private CharacterController ren_wu;
    private Vector3 move_v;
    private bool isGround;

    void Start()
    {
        ren_wu = GetComponent<CharacterController>();

        if (di_mian == null)
        {
            Debug.LogError("miss ground item");
        }
    }

    void Update()
    {
        
        if (di_mian == null) return;

        isGround = Physics.CheckSphere(di_mian.position, check_dis, groundMask);

        if (isGround && move_v.y < 0)
        {
            move_v.y = -2f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = transform.right * h + transform.forward * v;

        ren_wu.Move(dir * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            move_v.y = Mathf.Sqrt(jump_h * -2f * gravity);
        }

        move_v.y += gravity * Time.deltaTime;

        ren_wu.Move(move_v * Time.deltaTime);
    }
}