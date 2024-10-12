using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "new BlockData", menuName = "Tetris/TetrominoData")]
public class BlockData : ScriptableObject
{
    // 고스트 : 블럭 지우고, 고스트 지우고, 블럭 충돌검사 후, 고스트 생성, 블럭생성
    
    public enum Type
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z,
    }

    public Type kType;
    public Tile kTile;
    public Vector2Int[] kCells;
    public Sprite kBlockSprite;
    public Dictionary<Vector2Int, Vector2Int[]> kWallKickData;
}
