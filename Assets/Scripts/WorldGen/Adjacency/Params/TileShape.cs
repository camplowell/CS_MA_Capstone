using UnityEngine;

[CreateAssetMenu(fileName = "TileShape", menuName = "Capstone/TileShape", order = 1)]
public class TileShape : ScriptableObject
{
    public int northWest = 0;
    public int northEast = 0;
    public int southWest = 0;
    public int southEast = 0;
}
