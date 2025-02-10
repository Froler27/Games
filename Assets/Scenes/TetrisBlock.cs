using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TetrisBlock
{
    enum BlockType
    {
        I,
        O,
        T,
        S,
        Z,
        J,
        L
    }
    
    private BlockType blockType;
    private int maxState;
    private int state;
    private Vector3 position;

    static Dictionary<BlockType, Dictionary<int, Vector2[]>> dict = new Dictionary<BlockType, Dictionary<int, Vector2[]>>() {
        {BlockType.I, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(-2, 0), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)}},
            {1, new Vector2[] {new Vector2(0, 2), new Vector2(0, 1), new Vector2(0, 0), new Vector2(0, -1)}}
        }},
        {BlockType.O, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)}}
        }},
        {BlockType.T, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1)}},
            {1, new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0)}},
            {2, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1)}},
            {3, new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0)}}
        }},
        {BlockType.S, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(1, 1), new Vector2(0, 1)}},
            {1, new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, -1)}}
        }},
        {BlockType.Z, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 1)}},
            {1, new Vector2[] {new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, 1), new Vector2(1, 0)}}
        }},
        {BlockType.J, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(-1, 1)}},
            {1, new Vector2[] {new Vector2(0, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 1)}},
            {2, new Vector2[] {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(1, 0), new Vector2(1, -1)}},
            {3, new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, -1)}}
        }},
        {BlockType.L, new Dictionary<int, Vector2[]> {
            {0, new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(1, 1)}},
            {1, new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, -1)}},
            {2, new Vector2[] {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(1, 0), new Vector2(-1, -1)}},
            {3, new Vector2[] {new Vector2(0, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(-1, 1)}}
        }}
    };

    // Start is called before the first frame update
    public TetrisBlock(Vector3 pos)
    {
        position = pos;
        blockType = (BlockType)Random.Range(0, 7);
        switch (blockType)
        {
            case BlockType.I:
                maxState = 2;
                break;
            case BlockType.O:
                maxState = 1;
                break;
            case BlockType.T:
                maxState = 4;
                break;
            case BlockType.S:
                maxState = 2;
                break;
            case BlockType.Z:
                maxState = 2;
                break;
            case BlockType.J:
                maxState = 4;
                break;
            case BlockType.L:
                maxState = 4;
                break;
        }
        state = 0;
    }

    public Vector2[] GetBlock()
    {
        return dict[blockType][state];
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }
    
    bool IsValid()
    {
        return true;
    }

    public void RotateClockwise()
    {
        state = (state + 1) % maxState;
    }

    public void RotateCounterClockwise()
    {
        state = (state - 1 + maxState) % maxState;
    }

    public void MoveLeft()
    {
        position += new Vector3(-1, 0, 0);
    }

    public void MoveRight()
    {
        position += new Vector3(1, 0, 0);
    }

    public void MoveDown()
    {
        position += new Vector3(0, -1, 0);
    }

    public void MoveUp()
    {
        position += new Vector3(0, 1, 0);
    }
}
