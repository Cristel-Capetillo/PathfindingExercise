using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
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

      Node startNode = grid.NodeFromWorldPoint(startPosition);
      Node targetNode = grid.NodeFromWorldPoint(targetPosition);

      if (startNode.isWalkable && targetNode.isWalkable)
      {
         Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
         HashSet<Node> closedSet = new HashSet<Node>();
         openSet.Add(startNode);

         while (openSet.Count > 0)
         {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
               sw.Stop();
               Debug.Log("PATH FOUND WITHIN: " + sw.ElapsedMilliseconds + " MILLISECONDS");
               pathSuccess = true;
               break;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
               if (!neighbor.isWalkable || closedSet.Contains(neighbor))
               {
                  continue;
               }

               int newMovementCostToNeighbor = currentNode.gCost + CalculateDistance(currentNode, neighbor);
               if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
               {
                  neighbor.gCost = newMovementCostToNeighbor;
                  neighbor.hCost = CalculateDistance(neighbor, targetNode);
                  neighbor.parent = currentNode;
               
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
         waypoints = RetracePath(startNode, targetNode);
      }
      
      requestPathfinding.FinishProcessingPath(waypoints, pathSuccess);
   }

   Vector3[] RetracePath(Node startNode, Node endNode)
   {
      List<Node> path = new List<Node>();
      Node currentNode = endNode;

      while (currentNode != startNode)
      {
         path.Add(currentNode);
         currentNode = currentNode.parent;
      }

      Vector3[] waypoints = SimplifyPath(path);
      Array.Reverse(waypoints);

      return waypoints;
   }

   Vector3[] SimplifyPath(List<Node> path)
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

   int CalculateDistance(Node nodeA, Node nodeB)
   {
      int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
      int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

      if (distanceX > distanceY) 
         return 14 * distanceY + 10 * (distanceX - distanceY);
      return 14 * distanceX + 10 * (distanceY - distanceX);
   }
}
