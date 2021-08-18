﻿using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float cellRadius;
    Cells[,] grid;

    float cellDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        cellDiameter = cellRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / cellDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / cellDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Cells[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft =
            transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = 
                    worldBottomLeft + Vector3.right * (x * cellDiameter + cellRadius) + Vector3.forward * (y * cellDiameter + cellRadius);
                bool isWalkable = !(Physics.CheckSphere(worldPoint, cellRadius, unwalkableMask));
                grid[x, y] = new Cells(isWalkable, worldPoint, x, y);
            }
        }
    }

    public Cells CellFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Cells> GetNeighbors(Cells cells)
    {
        List<Cells> neighbors = new List<Cells>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0) continue;

                int checkX = cells.gridX + x;
                int checkY = cells.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        
        if (grid != null && displayGridGizmos)
        {
            foreach (Cells n in grid) 
            {
                Gizmos.color = (n.isWalkable)?Color.white:Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellDiameter-.1f));
            }
        }
    }
}
