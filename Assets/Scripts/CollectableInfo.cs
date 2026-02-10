using UnityEngine;

public class CollectableInfo : MonoBehaviour
{
    public CollectableType type;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
}
