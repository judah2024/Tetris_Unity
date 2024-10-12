using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Vector3Int kSpawnPosition = new Vector3Int(4, 19, 0);
    public Vector2Int kBoardSize = new Vector2Int(10, 20);

    public Tilemap kTilemap;
    [FormerlySerializedAs("kPiece")] public Block kBlock;

    public BlockData[] kBlocks;

    private BlockData mNextBlockData;
    private List<int> mListFullLine;
    private int mFullLineCount;

    private void Awake()
    {
        mListFullLine = new List<int>(4);
        foreach (var blockData in kBlocks)
        {
            blockData.kWallKickData = RotationData.WallKickOffset[blockData.kType];
        }
        
        SetNextBlock();
        SpawnBlock();
    }

    /// <summary>
    /// 다음 블록을 설정하고 UI이벤트를 발생시킵니다.
    /// </summary>
    private void SetNextBlock()
    {
        mNextBlockData = kBlocks[Random.Range(0, kBlocks.Length)];
        GameEvents.NextBlockChanged(mNextBlockData.kBlockSprite);
    }
 
    /// <summary>
    /// 게임 종료 조건을 확인합니다.
    /// </summary>
    /// <returns> GameState가 Win 혹은 Lose 이면 true를 반환합니다.</returns>
    private bool IsEndGame()
    {
        // 승리가 패배보다 우선합니다.
        if (GameManager.Instance.IsWin())
            return true;
        
        for (int col = 0; col < kBoardSize.x; col++)
        {
            if (kTilemap.HasTile(new Vector3Int(col, kBoardSize.y - 1)) == true)
            {
                // 보드의 경계에 닿았으므로 Lose 이벤트를 발생시킵니다.
                GameEvents.GameStateChanged(GameState.Lose);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 게임을 종료하기 위해 모든 Coroutine을 종료하고 GameManger의 EndGame을 호출합니다.
    /// </summary>
    private void EndGame()
    {
        kBlock.StopAllCoroutines();
        GameManager.Instance.EndGame();
    }

    /// <summary>
    /// 완성된 라인을 제거한 후, 블록들을 아래로 내립니다.
    /// </summary>
    private void DropLines()
    {
        // 완성된 라인 번호를 오름차순으로 정렬
        mListFullLine.Sort();
        int gap = 0;
        for (int row = 0; row < kBoardSize.y; row++)
        {
            MoveLine(row, row - gap);
            
            // 완성된 라인이라면 gap을 늘려준다.
            if (gap < mListFullLine.Count && mListFullLine[gap] == row)
            {
                gap++;
            }
        }
    }

    /// <summary>
    /// 다음 블럭을 생성한다.
    /// </summary>
    public void SpawnBlock()
    {
        kBlock.SpawnBlock(mNextBlockData, kSpawnPosition);
        DrawPiece(kBlock);
        SetNextBlock();
    }

    /// <summary>
    /// 주어진 Piece를 타일맵에 그립니다.
    /// </summary>
    /// <param name="block"> 그릴 piece </param>
    public void DrawPiece(Block block)
    {
        Vector3Int[] cells = block.cells;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int position = cells[i] + block.position;
            kTilemap.SetTile(position, block.data.kTile);
        }
    }

    /// <summary>
    /// 주어진 Piece를 타일맵에서 지웁니다.
    /// </summary>
    /// <param name="block"> 지울 piece </param>
    public void ClearPiece(Block block)
    {
        Vector3Int[] cells = block.cells;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int position = cells[i] + block.position;
            kTilemap.SetTile(position, null);
        }
    }

    /// <summary>
    /// 주어진 좌표의 블럭에 충돌이 일어나는지 확인합니다.
    /// </summary>
    /// <param name="newPosition"> 움직이는 블럭의 좌표 </param>
    /// <returns> 충돌이 일어나지 않으면 true를 반환합니다.</returns>
    public bool IsValidPosition(Vector3Int newPosition)
    {
        Vector3Int[] cells = kBlock.cells;
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = newPosition + cells[i];

            if (!IsInBound(tilePosition))
                return false;

            if (kTilemap.HasTile(tilePosition))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 주어진 좌표가 경곔내인지 여부를 확인합니다.
    /// </summary>
    /// <param name="position"> Tilemap상의 좌표</param>
    /// <returns> 경계 내라면 true를 반환합니다. </returns>
    public bool IsInBound(Vector3Int position)
    {
        bool xCheck = 0 <= position.x && position.x < kBoardSize.x;
        bool yCheck = 0 <= position.y;
        return xCheck && yCheck;
    }

    /// <summary>
    /// 주어진 행이 완성된 라인인지 확인합니다.
    /// </summary>
    /// <param name="row"> 행의 번호 </param>
    /// <returns> 라인이 완성되었다면 true를 반환합니다. </returns>
    public bool IsLineFull(int row)
    {
        for (int col = 0; col < kBoardSize.x; col++)
        {
            if (kTilemap.HasTile(new Vector3Int(col, row)) == false)
            {
                return false;
            }
        }

        return true;
    }
   
    /// <summary>
    /// 게임상태에 따라 다음 행동을 제어합니다.
    /// </summary>
    public void ProcessBasedOnGameState()
    {
        if (IsEndGame())
        {
            EndGame();
        }
        else
        {
            SpawnBlock();
        }
    }

    /// <summary>
    /// 주어진 행의 블럭을 모두 지웁니다.
    /// </summary>
    /// <param name="row"></param>
    private void ClearLine(int row)
    {
        for (int col = 0; col < kBoardSize.x; col++)
        {
            kTilemap.SetTile(new Vector3Int(col, row), null);
        }
    }

    /// <summary>
    /// from행의 모든 블럭을 to행으로 옮김니다.
    /// </summary>
    /// <param name="from"> 옮길 행의 번호 </param>
    /// <param name="to"> 도착 행의 번호 </param>
    private void MoveLine(int from, int to)
    {
        if (from <= to)
            return;
        
        for (int col = 0; col < kBoardSize.x; col++)
        {
            TileBase tile = kTilemap.GetTile(new Vector3Int(col, from));
            // 이전 위치의 블럭을 지워준다.
            kTilemap.SetTile(new Vector3Int(col, from), null);
            kTilemap.SetTile(new Vector3Int(col, to), tile);
        }
    }

    /// <summary>
    /// 완성된 라인을 지운후 블럭들을 내린다.
    /// </summary>
    public void ClearFullLines()
    {
        // 완성된 라인이 없다면 함수를 리턴한다.
        if (HasFullLines() == false)
        {
            return;
        }

        foreach (int row in mListFullLine)
        {
            ClearLine(row);
        }

        DropLines();
        // 작업이 끝난 후 사용된 정보를 지운다.
        mListFullLine.Clear();
    }

    /// <summary>
    /// 다음 블럭을 생성하기 전에 완성된 라인을 제거한다.
    /// </summary>
    public void UpdateLines()
    {
        mListFullLine.Clear();
        int startRow = kBlock.position.y - 1;
        for (int i = 0; i < 4; i++)
        {
            int row = startRow + i;
            if (row < 0 || row >= kBoardSize.y)
                continue;

            if (IsLineFull(row))
            {
                mListFullLine.Add(row);
            }
        }

        // 제거된 라인이 있다면 점수를 증가시킨다.
        if (mListFullLine.Count > 0)
        {
            int lines = mListFullLine.Count;
            GameEvents.LinesCleared(lines);
            GameEvents.ScoreChanged(lines * lines * 100);
        }
    }

    /// <summary>
    /// 완성된 라인이 있는지 확인한다.
    /// </summary>
    /// <returns> 완성된 라인이 있다면 true를 반환한다.</returns>
    public bool HasFullLines() => mListFullLine.Count > 0;
}
