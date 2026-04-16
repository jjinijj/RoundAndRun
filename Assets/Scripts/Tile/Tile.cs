using UnityEngine;

public enum TileType { Empty, JumpObstacle_Short, JumpObstacle_Long, SlideObstacle, Item, ItemJump }
public class Tile : MonoBehaviour
{
    public TileType tileType;
    public float tileLength = 10f; // 타일 길이, 프리팹마다 맞게 조정
    public Item item = null;

    public void ResetTile()
    {
        // 나중에 아이템 재활성화 등 초기화 로직 여기에
        if(item != null)
        {
            item.ResetItem();
        }

    }
}