using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public static class RotationData
{
    /// <summary>
    /// pivot을 기준으로 회전한다.
    /// </summary>
    /// <param name="position"> 회전 주체의 좌표 </param>
    /// <param name="pivot"> 회전 중심 좌표 </param>
    /// <param name="rotDir"> 1 = 시계 방향, -1 = 반시계 방향 </param>
    /// <returns> 회전돤 좌표 </returns>
    public static Vector3Int Rotate(Vector3 position, Vector3 pivot, int rotDir)
    {
        position -= pivot;
        return new Vector3Int(Mathf.RoundToInt(rotDir * position.y + pivot.x), Mathf.RoundToInt(-rotDir * position.x + pivot.y));
    }
    
    /* Begin WallKick Offset */
    public static readonly Dictionary<Vector2Int, Vector2Int[]> WallKickOffset_I = new ()
    {
        {new Vector2Int(0, 1), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int( -2, 0), new Vector2Int( +1, 0), new Vector2Int(-2,-1), new Vector2Int( +1,+2) }},
        {new Vector2Int(1, 0), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(+2, 0), new Vector2Int(-1, 0), new Vector2Int(+2,+1), new Vector2Int( -1,-2) }},
        {new Vector2Int(1, 2), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(+2, 0), new Vector2Int(-1,+2), new Vector2Int(+2,-1) }},
        {new Vector2Int(2, 1), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(+1, 0), new Vector2Int(-2, 0), new Vector2Int(+1,-2), new Vector2Int(-2,+1) }},
        {new Vector2Int(2, 3), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(+2, 0), new Vector2Int(-1, 0), new Vector2Int(+2,+1), new Vector2Int(-1,-2) }},
        {new Vector2Int(3, 2), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(+1, 0), new Vector2Int(-2,-1), new Vector2Int(+1,+2) }},
        {new Vector2Int(3, 0), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(+1, 0), new Vector2Int(-2, 0), new Vector2Int(+1,-2), new Vector2Int(-2,+1) }},
        {new Vector2Int(0, 3), new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(+2, 0), new Vector2Int(-1,+2), new Vector2Int(+2,-1) }},
    };
    
    public static readonly Dictionary<Vector2Int, Vector2Int[]> WallKickOffset_Other = new ()
    {
        {new Vector2Int(0, 1), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,+1), new Vector2Int( 0,-2), new Vector2Int(-1,-2) }},
        {new Vector2Int(1, 0), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(+1, 0), new Vector2Int(+1,-1), new Vector2Int( 0,+2), new Vector2Int(+1,+2) }},
        {new Vector2Int(1, 2), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(+1, 0), new Vector2Int(+1,-1), new Vector2Int( 0,+2), new Vector2Int(+1,+2) }},
        {new Vector2Int(2, 1), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,+1), new Vector2Int( 0,-2), new Vector2Int(-1,-2) }},
        {new Vector2Int(2, 3), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(+1, 0), new Vector2Int(+1,+1), new Vector2Int( 0,-2), new Vector2Int(+1,-2) }},
        {new Vector2Int(3, 2), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int( 0,+2), new Vector2Int(-1,+2) }},
        {new Vector2Int(3, 0), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int( 0,+2), new Vector2Int(-1,+2) }},
        {new Vector2Int(0, 3), new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(+1, 0), new Vector2Int(+1,+1), new Vector2Int( 0,-2), new Vector2Int(+1,-2) }},
    };
    /* End WallKick Offset */

    public static readonly Dictionary<BlockData.Type, Dictionary<Vector2Int, Vector2Int[]>> WallKickOffset = new ()
    {
        { BlockData.Type.I, WallKickOffset_I },
        { BlockData.Type.J, WallKickOffset_Other },
        { BlockData.Type.L, WallKickOffset_Other },
        { BlockData.Type.O, WallKickOffset_Other },
        { BlockData.Type.S, WallKickOffset_Other },
        { BlockData.Type.T, WallKickOffset_Other },
        { BlockData.Type.Z, WallKickOffset_Other },
    };
}
