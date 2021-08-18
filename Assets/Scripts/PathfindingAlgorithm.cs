using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PathfindingAlgorithm : MonoBehaviour
{
   RequestPathfinding requestPathfinding;
   GridGenerator grid;

   void Awake()
   {
      requestPathfinding = GetComponent<RequestPathfinding>();
      grid = GetComponent<GridGenerator>();
   }
   
   public void StartFindPath(Vector3 startPosition, Vector3 targetPosition)
   {
      StartCoroutine(FindPath(startPosition, targetPosition));
   }
   
   IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
   {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      Vector3[] waypoints = new Vector3[0];
      bool pathSuccess = false;

      Cells startCells = grid.NodeFromWorldPoint(startPosition);
      Cells targetCells = grid.NodeFromWorldPoint(targetPosition);

      if (startCells.isWalkable && targetCells.isWalkable)
      {
         Heap<Cells> openSet = new Heap<Cells>(grid.MaxSize);
         HashSet<Cells> closedSet = new HashSet<Cells>();
         openSet.Add(startCells);

         while (openSet.Count > 0)
         {
            Cells currentCells = openSet.RemoveFirst();
            closedSet.Add(currentCells);

            if (currentCells == targetCells)
            {
               sw.Stop();
               Debug.Log("PATH FOUND WITHIN: " + sw.ElapsedMilliseconds + " MILLISECONDS");
               pathSuccess = true;
               break;
            }

            foreach (Cells neighbor in grid.GetNeighbors(currentCells))
            {
               if (!neighbor.isWalkable || closedSet.Contains(neighbor))
               {
                  continue;
               }

               int newMovementCostToNeighbor = currentCells.gCost + CalculateDistance(currentCells, neighbor);
               if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
               {
                  neighbor.gCost = newMovementCostToNeighbor;
                  neighbor.hCost = CalculateDistance(neighbor, targetCells);
                  neighbor.parent = currentCells;
               
                  if(!openSet.Contains(neighbor))
                     openSet.Add(neighbor);
                  else
                     openSet.UpdateItem(neighbor);
               }
            }
         }
      }

      yield return null;

      if (pathSuccess)
      {
         waypoints = RetracePath(startCells, targetCells);
      }
      
      requestPathfinding.FinishProcessingPath(waypoints, pathSuccess);
   }

   Vector3[] RetracePath(Cells startCells, Cells endCells)
   {
      List<Cells> path = new List<Cells>();
      Cells currentCells = endCells;

      while (currentCells != startCells)
      {
         path.Add(currentCells);
         currentCells = currentCells.parent;
      }

      Vector3[] waypoints = SimplifyPath(path);
      Array.Reverse(waypoints);

      return waypoints;
   }

   Vector3[] SimplifyPath(List<Cells> path)
   {
      List<Vector3> waypoints = new List<Vector3>();
      Vector2 directionOld = Vector2.zero;

      for (int i = 1; i < path.Count; i++)
      {
         Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
         if (directionNew != directionOld)
         {
            waypoints.Add(path[i].worldPosition);
         }
         directionOld = directionNew;
      }
      
      return waypoints.ToArray();
   }

   int CalculateDistance(Cells cellsA, Cells cellsB)
   {
      int distanceX = Mathf.Abs(cellsA.gridX - cellsB.gridX);
      int distanceY = Mathf.Abs(cellsA.gridY - cellsB.gridY);

      if (distanceX > distanceY) 
         return 14 * distanceY + 10 * (distanceX - distanceY);
      return 14 * distanceX + 10 * (distanceY - distanceX);
   }
}
