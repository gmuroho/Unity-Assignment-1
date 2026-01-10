using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera mian_xiangji;
    public Transform shen_ti;

    public float mouse_v = 100f;
    private float x_zhuan = 0f;

    public float juli = 3f;
    public LayerMask whatIsLayer;
    public KeyCode key_E = KeyCode.E;

    public bool you_jiandao = false;
    public GameObject shou_shang_wu_ti;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mian_xiangji == null) mian_xiangji = GetComponentInChildren<Camera>();
        if (shen_ti == null) shen_ti = transform;

        Rigidbody rb = mian_xiangji.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (shou_shang_wu_ti != null) shou_shang_wu_ti.SetActive(false);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouse_v * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouse_v * Time.deltaTime;

        x_zhuan -= mouseY;
        x_zhuan = Mathf.Clamp(x_zhuan, -85f, 85f);

        mian_xiangji.transform.localRotation = Quaternion.Euler(x_zhuan, 0f, 0f);
        shen_ti.Rotate(Vector3.up * mouseX);

        Ray r = mian_xiangji.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit h;

        if (Physics.Raycast(r, out h, juli, whatIsLayer))
        {
            if (Input.GetKeyDown(key_E))
            {
                if (h.collider.name.ToLower().Contains("shears") || h.collider.CompareTag("Tool"))
                {
                    you_jiandao = true;
                    Destroy(h.collider.gameObject);
                    if (shou_shang_wu_ti != null) shou_shang_wu_ti.SetActive(true);
                }
            }
        }

        if (you_jiandao == true && Input.GetMouseButtonDown(0))
        {
            RaycastHit h2;
            if (Physics.Raycast(mian_xiangji.transform.position, mian_xiangji.transform.forward, out h2, 2.5f))
            {
                var component = h2.collider.GetComponent<TrimmableObject>();
                if (component != null)
                {
                    component.Trim();
                }
            }
        }
    }
}