using System;
using System.Collections.Generic;
using UnityEngine;

public class Storage<T> where T : class
{
    private Vector2Int _size;
    private T[,] _cells;

    public Vector2Int Size => _size;

    public bool IsInitialized => _cells != null;

    public void Initialize(Vector2Int size)
    {
        _size = size;
        _cells = new T[_size.x, _size.y];
    }

    public bool IsInside(Vector2Int position)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        return position.x >= 0 && position.x < _size.x
                               && position.y >= 0 && position.y < _size.y;
    }

    public bool TryGet(Vector2Int position, out T value)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        if (!IsInside(position))
        {
            value = null;
            return false;
        }

        value = _cells[position.x, position.y];
        return value != null;
    }

    public bool IsEmpty(Vector2Int position)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        if (!IsInside(position))
        {
            return false;
        }

        return _cells[position.x, position.y] == null;
    }

    public bool TrySet(Vector2Int position, T value)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        if (value == null)
        {
            return false;
        }

        if (!IsInside(position))
        {
            return false;
        }

        if (_cells[position.x, position.y] != null)
        {
            return false;
        }

        _cells[position.x, position.y] = value;
        return true;
    }

    public bool TryClear(Vector2Int position, out T removed)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        if (!IsInside(position))
        {
            removed = null;
            return false;
        }

        removed = _cells[position.x, position.y];
        
        if (removed == null)
        {
            return false;
        }

        _cells[position.x, position.y] = null;
        return true;
    }

    public bool TryFindFirstEmpty(out Vector2Int position)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (_cells[x, y] == null)
                {
                    position = new Vector2Int(x, y);
                    return true;
                }
            }
        }

        position = default;
        return false;
    }

    public IReadOnlyList<(Vector2Int position, T value)> GetAllNonEmpty()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("GridStorage2D is not initialized.");
        }

        List<(Vector2Int position, T value)> result = new List<(Vector2Int position, T value)>();

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                T value = _cells[x, y];
                if (value != null)
                {
                    result.Add((new Vector2Int(x, y), value));
                }
            }
        }

        return result;
    }
}