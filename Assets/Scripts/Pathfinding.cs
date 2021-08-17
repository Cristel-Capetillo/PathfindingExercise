using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
{
   GridGenerator grid;
   public Transform seeker, target;

   void Awake()
   {
      grid = GetComponent<GridGenerator>();
   }

   private void Update()
   {
      if (Input.GetButtonDown("Jump"))
      {
         FindPath(seeker.position, target.position);
      }
   }

   void FindPath(Vector3 startPosition, Vector3 targetPosition)
   {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      
      Node startNode = grid.NodeFromWorldPoint(startPosition);
      Node targetNode = grid.NodeFromWorldPoint(targetPosition);

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
            RetracePath(startNode, targetNode);
            return;
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
            }
         }
      }
   }

   void RetracePath(Node startNode, Node endNode)
   {
      List<Node> path = new List<Node>();
      Node currentNode = endNode;

      while (currentNode != startNode)
      {
         path.Add(currentNode);
         currentNode = currentNode.parent;
      }
      path.Reverse();

      grid.path = path;
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
