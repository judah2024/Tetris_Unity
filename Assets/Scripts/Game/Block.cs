using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    public Board kBoard;
    [HideInInspector] public BlockData data;
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public Vector3Int[] cells;

    private int mRoatationIndex;
    private float mLockDelay = 1.25f;
    private Coroutine mMoveCoroutine;
    private Coroutine mLockCoroutine;

    /// <summary>
    /// 블록 정보를 토대로 지정된 위치에 블럭을 생성한다.
    /// </summary>
    /// <param name="data"> 새 블럭 정보 </param>
    /// <param name="position"> 시작 위치 좌표 </param>
    public void SpawnBlock(BlockData data, Vector3Int position)
    {
        this.data = data;
        this.position = position;
        mRoatationIndex = 0;

        cells = new Vector3Int[4];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)this.data.kCells[i];
        }
        
        // 이전 코루틴들을 모두 정지시킨다.
        StopAllCoroutines();
        // 자동 낙하를 시작한다.
        StartCoroutine(DropCoroutine());
    }

    /// <summary>
    /// 주어진 시간이 지난 후, 블럭을 고정시킨다.
    /// </summary>
    /// <param name="delay"> 고정 시간 </param>
    IEnumerator LockCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 연속이동을 정지시킨다.
        if (mMoveCoroutine != null)
        {
            StopCoroutine(mMoveCoroutine);
            mMoveCoroutine = null;
        }
        LockPiece();
    }

    /// <summary>
    /// 일정시간마다 블럭을 낙하시킨다.
    /// </summary>
    IEnumerator DropCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(mLockDelay / 2);
            kBoard.ClearPiece(this);

            if (TryMovePiece(Vector2Int.down))
            {
                if (mLockCoroutine != null)
                {
                    StopCoroutine(mLockCoroutine);
                    mLockCoroutine = null;
                }
            }
            else
            {
                // 낙하에 실패하면 블럭이 충돌했으므로 LockCoroutine을 시작한다.
                mLockCoroutine = StartCoroutine(LockCoroutine(mLockDelay));
            }
            kBoard.DrawPiece(this);
        }
    }

    /// <summary>
    /// 주어진 방향으로 연속이동한다.
    /// </summary>
    /// <param name="dir"> 이동 방향 </param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(Vector2Int dir)
    {
        float lockDelay = 1.0f;
        while (true)
        {
            kBoard.ClearPiece(this);

            if (dir.x != 0)
            {
                TryMovePiece(new Vector2Int(dir.x ,0));
            }

            if (dir.y != 0)
            {
                if (TryMovePiece(new Vector2Int(0 ,dir.y)))
                {
                    if (mLockCoroutine != null)
                    {
                        StopCoroutine(mLockCoroutine);
                        mLockCoroutine = null;
                    }
                }
                else
                {
                    mLockCoroutine = StartCoroutine(LockCoroutine(lockDelay));
                }
            }
            kBoard.DrawPiece(this);

            // TODO : 하드코딩 되어 있음
            yield return new WaitForSeconds(0.25f);
        }
    }

    /// <summary>
    /// 블럭을 고정시키고 라인 제거, 게임 종료 판정을 진행한다.
    /// </summary>
    void LockPiece()
    {
        kBoard.DrawPiece(this);
        kBoard.UpdateLines();
        
        if (kBoard.HasFullLines())
        {
            kBoard.ClearFullLines();
        }
            
        // TODO : 게임 종료 판정 후 진행
        kBoard.ProcessBasedOnGameState();
    }

    /// <summary>
    /// 해당 방향으로 블럭의 이동을 시도한다.
    /// </summary>
    /// <param name="dir"> 이동방향 </param>
    /// <returns> 이동이 가능하다면 이동하고 true를 반환한다. </returns>
    bool TryMovePiece(Vector2Int dir)
    {
        Vector3Int newPosition = position + (Vector3Int)dir;
        bool result = kBoard.IsValidPosition(newPosition);
        if (result)
        {
            position = newPosition;
        }

        return result;
    }

    /// <summary>
    /// 블럭을 충돌하지 않는 가장 아래 좌표로 이동시킨 후 LockPiece를 호출한다.
    /// </summary>
    void HardDrop()
    {
        kBoard.ClearPiece(this);
        while (TryMovePiece(Vector2Int.down))
        {
        }

        LockPiece();
    }

    /// <summary>
    /// Input System의 Move 키의 입력 콜백 함수
    /// </summary>
    /// <param name="value"> 이동 값 </param>
    void OnMove(InputValue value)
    {
        Vector2 inputDir = value.Get<Vector2>();
        Vector2Int dir = new Vector2Int(Mathf.RoundToInt(inputDir.x), Mathf.RoundToInt(inputDir.y));

        if (mMoveCoroutine != null)
        {
            StopCoroutine(mMoveCoroutine);
            mMoveCoroutine = null;
        }

        if (dir == Vector2Int.zero)
            return;

        if (dir == Vector2Int.up)
        {
            HardDrop();
        }
        else
        {
            mMoveCoroutine = StartCoroutine(MoveCoroutine(dir));
        }
    }

    /// <summary>
    /// 블럭을 회전한다.
    /// </summary>
    /// <param name="dir"> 1 = 시계방향, -1 = 반시계방향 </param>
    void Rotate(int dir)
    {
        kBoard.ClearPiece(this);
        
        DoRotation(dir);
        
        int newRotationIndex = (mRoatationIndex + dir + 4) % 4;
        // SRS 시스템의 WallKick이 가능한지 확인한다.
        if (CanWallKick(newRotationIndex))
        {
            mRoatationIndex = newRotationIndex;
        }
        else
        {
            // 회전이 불가능 하다면 이전으로 돌아간다.
            DoRotation(-dir);
        }
        kBoard.DrawPiece(this);
    }

    /// <summary>
    /// 회전이 가능한지 확인한다.
    /// </summary>
    /// <param name="newRotationIndex"> 회전을 실행한 회전 인덱스 </param>
    /// <returns> 회전이 성공하면 true를 반환한다. </returns>
    bool CanWallKick(int newRotationIndex)
    {
        // offset값의 배열을 참조
        Vector2Int[] wallKickOffset = data.kWallKickData[new Vector2Int(mRoatationIndex, newRotationIndex)];
        foreach (Vector2Int offset in wallKickOffset)
        {
            Vector3Int newPosition = position + (Vector3Int)offset;
            if (kBoard.IsValidPosition(newPosition))
            {
                // 회전이 가능하면 true를 반환
                position = newPosition;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 회전을 실제로 실행한다.
    /// </summary>
    /// <param name="dir"> 1 = 시계방향, -1 = 반시계방향 </param>
    void DoRotation(int dir)
    {
        // I, O는 회전 중심이 다르므로 회전 중심을 추가한다.
        Vector3 pivot;
        switch (data.kType)
        {
            case BlockData.Type.I: case BlockData.Type.O:
                pivot = new Vector3(0.5f, 0.5f);
                break;
            default:
                pivot = Vector3.zero;
                break;
        }

        // 중심을 기준으로 블럭을 회전
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = RotationData.Rotate(cells[i], pivot, dir);
        }
        
    }

    void OnRotateCW(InputValue value)
    {
        Rotate(1);
    }
    
    
    void OnRotateCCW(InputValue value)
    {
        Rotate(-1);
    }
}
