using UnityEngine;

public class ScissorHandleFixer : MonoBehaviour
{
    public Transform leftObj;
    public Transform rightObj;

    [Range(0, 60)] public float openAngle = 30f;
    public float speed = 10f;

    private float curAngle = 0f;
    private bool isOpen = false;

    void Start()
    {
        if (leftObj != null)
        {
            Vector3 s1 = leftObj.localScale;
            if (Mathf.Abs(s1.x - s1.y) > 0.01f || Mathf.Abs(s1.y - s1.z) > 0.01f)
            {
                Debug.LogWarning(leftObj.name + " scale error! 缩放不对会位移!");
            }
        }

        if (rightObj != null)
        {
            Vector3 s2 = rightObj.localScale;
            if (Mathf.Abs(s2.x - s2.y) > 0.01f || Mathf.Abs(s2.y - s2.z) > 0.01f)
            {
                Debug.LogWarning(rightObj.name + " scale error! 缩放不对会位移!");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isOpen = !isOpen;
        }

        float mb_Angle = 0;
        if (isOpen)
        {
            mb_Angle = openAngle;
        }
        else
        {
            mb_Angle = 0;
        }

        curAngle = Mathf.Lerp(curAngle, mb_Angle, Time.deltaTime * speed);

        if (leftObj != null && rightObj != null)
        {
            leftObj.localRotation = Quaternion.Euler(0, 0, curAngle);
            rightObj.localRotation = Quaternion.Euler(0, 0, -curAngle);
        }
    }
}