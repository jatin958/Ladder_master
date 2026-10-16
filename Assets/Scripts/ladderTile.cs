using UnityEngine;

public class ladderTile : MonoBehaviour
{
    [SerializeField] private colorType colorType;
    public colorType getLadderColor()
    {
        return colorType;
    }
}
