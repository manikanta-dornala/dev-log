using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GameBoard : MonoBehaviour
{

    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;
    [SerializeField] private Pattern pattern;
    [SerializeField] private float updateInterval = 0.05f; // 50ms

    private void Start()
    {
        SetPattern(pattern);
    }

    private void SetPattern(Pattern pattern)
    {
        Debug.Log($"Setting pattern: {pattern.name}");
        Clear();
        Vector2Int center = pattern.GetCenter();
        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector2Int cell = pattern.cells[i] - center;
            AliveCells.Add((Vector3Int)cell);
            currentState.SetTile((Vector3Int)cell, aliveTile);
            // Debug.Log($"Set cell at {cell}");
        }
    }

    private void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }

    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }


    private IEnumerator Simulate()
    {
        while (enabled)
        {
            UpdateState();
            yield return new WaitForSeconds(updateInterval); ;
        }
    }


    HashSet<Vector3Int> AliveCells = new HashSet<Vector3Int>();
    private void UpdateState()
    {

        var cellsToCheck = new HashSet<Vector3Int>(AliveCells);
        foreach (var cell in AliveCells)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    cellsToCheck.Add(new Vector3Int(cell.x + dx, cell.y + dy, 0));
                }
            }
        }

        // Debug.Log($"Alive cells: {AliveCells.Count}");
        // Debug.Log($"Frontier cells: {cellsToCheck.Count}");
        // foreach (var cell in AliveCells)
        // {
        //     Debug.Log($"Alive cell at {cell}");
        // }

        foreach (var cell in cellsToCheck)
        {
            var isAlive = IsAlive(cell);
            var aliveNeighbors = GetNumAliveNeighbors(cell);
            // Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            // Any live cell with two or three live neighbours lives on to the next generation.
            // Any live cell with more than three live neighbours dies, as if by overpopulation.
            // Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            if (isAlive && aliveNeighbors < 2)
            {
                AliveCells.Remove(cell);
            }
            else if (isAlive && (aliveNeighbors == 2 || aliveNeighbors == 3))
            {
                nextState.SetTile(cell, aliveTile);
            }
            else if (isAlive && aliveNeighbors > 3)
            {
                AliveCells.Remove(cell);
            }
            else if (!isAlive && aliveNeighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                AliveCells.Add(cell);
            }

        }
        (nextState, currentState) = (currentState, nextState);
        nextState.ClearAllTiles();
    }

    private bool IsAlive(Vector3Int position)
    {
        return currentState.GetTile(position) == aliveTile;
    }


    private int GetNumAliveNeighbors(Vector3Int cell)
    {
        // Game of Life rules
        int aliveNeighbors = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (IsAlive(new Vector3Int(cell.x + dx, cell.y + dy, 0)))
                {
                    aliveNeighbors++;
                }
            }
        }
        return aliveNeighbors;
    }

}


