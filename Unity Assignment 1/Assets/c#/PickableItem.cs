using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Tooltip("0:剪刀, 1:水壶, 2:铲子 (必须与ToolSystem里的数组顺序一致)")]
    public int toolID;
}