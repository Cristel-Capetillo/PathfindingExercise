using UnityEngine;

public class Cells : IHeapItem<Cells>
{
    public bool isWalkable;
    public Vector3 worldPosition;
    
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;
    public int movementPenalty;

    public Cells parent;

    int heapIndex;

    public Cells(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _movPenalty)
    {
        isWalkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _movPenalty;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Cells cellsToCompare)
    {
        int compare = fCost.CompareTo(cellsToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(cellsToCompare.hCost);
        }

        return -compare;
    }
}
