/*
 * 
 * using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    public Transform playerTransform;
    private GridMap gridMap;
    private List<Node> path = new List<Node>();

    void Start()
    {
        gridMap = FindObjectOfType<GridMap>();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            FindPath(transform.position, playerTransform.position);
            FollowPath();
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = gridMap.GetNode(startPos);
        Node targetNode = gridMap.GetNode(targetPos);

        if (startNode == null || targetNode == null || !startNode.walkable || !targetNode.walkable)
            return;

       // Heap<Node> openSet = new Heap<Node>(gridMap.gridSize.x * gridMap.gridSize.y);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in gridMap.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> newPath = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            newPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        newPath.Reverse();
        path = newPath;

        gridMap.path = path; // Update the path in the gridMap for visualization
    }

    void FollowPath()
    {
        if (path.Count > 0)
        {
            Node targetNode = path[0];
            Vector3 targetPosition = targetNode.worldPosition;
            targetPosition.z = transform.position.z; // Maintain the same z position

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2); // Move speed can be adjusted

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                path.RemoveAt(0);
            }
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
 * 
 * 
 * 
 */
