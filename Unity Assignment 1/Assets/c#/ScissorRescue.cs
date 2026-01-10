using UnityEngine;

public class ScissorRescue : MonoBehaviour
{
    public Transform l_zhou;
    public Transform r_zhou;

    public AudioSource sound1;
    public AudioSource sound2;

    public float open_deg = 30f;
    public float ssspeed = 12f;
    public float dist = 3f;

    public Vector3 pppos = new Vector3(0f, -0.5f, 1.2f);
    public Vector3 rrrot = new Vector3(0f, 0f, 0f);

    private bool pick_ok = false;
    private bool cut_ing = false;
    private float cur_a = 30f;
    private float tar_a = 30f;

    void Start()
    {
        cur_a = open_deg;
        tar_a = open_deg;
    }

    void Update()
    {
        if (pick_ok == false)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit h;

                if (Physics.Raycast(r, out h, dist))
                {
                    if (h.transform == this.transform || h.transform.IsChildOf(this.transform))
                    {
                        pick_ok = true;
                        transform.SetParent(Camera.main.transform);
                        transform.localPosition = pppos;
                        transform.localRotation = Quaternion.Euler(rrrot);

                        if (GetComponent<Rigidbody>() != null)
                        {
                            GetComponent<Rigidbody>().isKinematic = true;
                        }

                        if (GetComponent<Collider>() != null)
                        {
                            GetComponent<Collider>().enabled = false;
                        }
                    }
                }
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (cut_ing == false)
            {
                cut_ing = true;
                tar_a = 0f;
                if (sound1 != null)
                {
                    sound1.Play();
                }
            }
        }

        cur_a = Mathf.Lerp(cur_a, tar_a, Time.deltaTime * ssspeed);

        if (l_zhou != null)
        {
            l_zhou.localRotation = Quaternion.Euler(0, cur_a, 0);
        }
        if (r_zhou != null)
        {
            r_zhou.localRotation = Quaternion.Euler(0, -cur_a, 0);
        }

        if (cut_ing == true)
        {
            if (cur_a < 1f)
            {
                tar_a = open_deg;
                if (sound2 != null)
                {
                    sound2.Play();
                }
                cut_ing = false;
            }
        }
    }
}