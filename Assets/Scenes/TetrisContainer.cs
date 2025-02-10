using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisContainer
{
    private int width = 10;
    private int height = 21;
    private int[,] container;

    public TetrisContainer(int _width, int _height)
    {
        width = _width;
        height = _height;
        container = new int[height, width];
    }

    public bool HasValue(int i, int j)
    {
        return container[i, j] > 0;
    }

    int GetFirstEmptyRow()
    {
        for (int i = 0; i < height; i++)
        {
            if (IsEmptyRow(i))
            {
                return i;
            }
        }
        return height;
    }

    bool IsEmptyRow(int row) // note: row must be a valid row
    {
        for (int j = 0; j < width; j++)
        {
            if (container[row, j] > 0)
            {
                return false;
            }
        }
        return true;
    }

    int GetFirstFullRow()
    {
        for (int i = 0; i < height; i++)
        {
            if (IsFullRow(i))
            {
                return i;
            }
        }
        return height;
    }

    bool IsFullRow(int row) // note: row must be a valid row
    {
        for (int j = 0; j < width; j++)
        {
            if (container[row, j] == 0)
            {
                return false;
            }
        }
        return true;
    }

    public List<int> GetFullRows()
    {
        List<int> res = new List<int>();
        int firstFullRow = GetFirstFullRow();
        if (firstFullRow >= height) {
            return res;
        }
        res.Add(firstFullRow);
        // 4 is the maximum number of rows from the first full row to the last full row
        for (int i = firstFullRow + 1; i < firstFullRow + 4 && i < height; i++) {
            if (IsFullRow(i)) {
                res.Add(i);
            }
        }
        return res;
    }

    void ClearRow(int row)
    {
        for (int j = 0; j < width; j++)
        {
            container[row, j] = 0;
        }
    }

    public (int, int) ClearRows(List<int> rows)
    {
        if (rows.Count == 0) {
            return (0, 0);
        }
        int firstEmptyRow = GetFirstEmptyRow();
        Dictionary<int, int> rowMap = new Dictionary<int, int>();
        for (int i = rows[0]; i < firstEmptyRow; i++) {
            rowMap[i] = 1;
        }
        for (int i = 0; i < rows.Count; i++) {
            rowMap[rows[i]] = 0;
        }
        for (int i = 1; i < rows.Count; i++) {
            for (int j = rows[i] + 1; j < firstEmptyRow; j++) {
                if (rowMap[j] > 0)
                    rowMap[j] += 1;
            }
        }
        for (int i = rows[0] + 1; i < firstEmptyRow; i++) {
            if (rowMap[i] != 0) {
                CopyRow(i, i - rowMap[i]);
            }
        }
        return (rows[0], firstEmptyRow);
    }

    void CopyRow(int src, int dest)
    {
        if (src == dest || dest < 0 || dest >= height) {
            return;
        }
        if (src < 0 || src >= height ) {
            ClearRow(dest);
            return;
        }
        for (int j = 0; j < width; j++) {
            container[dest, j] = container[src, j];
        }
    }

    public bool FallBlock(TetrisBlock target)
    {
        if (IsValid(target, true)) {
            Vector3 pos = target.GetPosition();
            Vector2[] block = target.GetBlock();
            for (int i = 0; i < block.Length; i++)
            {
                int y = (int)(pos.y + block[i].y);
                int x = (int)(pos.x + block[i].x);
                container[y, x] = 1;
            }
            return true;
        }
        return false;
    }

    public bool IsValid(TetrisBlock target, bool isStrict = false)
    {
        Vector3 pos = target.GetPosition();
        Vector2[] block = target.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(pos.y + block[i].y);
            int x = (int)(pos.x + block[i].x);
            if (y >= height)
            {
                if (isStrict)
                {
                    return false;
                }
                continue;
            }
            if (x < 0 || x >= width || y < 0 || container[y, x] > 0)
            {
                return false;
            }
        }
        return true;
    }

    bool IsGameOver(TetrisBlock target)
    {
        Vector3 pos = target.GetPosition();
        Vector2[] block = target.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(pos.y + block[i].y);
            int x = (int)(pos.x + block[i].x);
            if (y >= height)
            {
                return true;
            }
        }
        return false;
    }

}
