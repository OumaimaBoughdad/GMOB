using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform TheFollowed;

    private GridMap gridMap;
    private List<Node> currentPath = null;

    [SerializeField]
    private float followerSpeed = 5f;

   [SerializeField] private Animator animator; 

    void Awake()
    {
        gridMap = GetComponent<GridMap>();
    }

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.P))
        {
            FindPath(transform.position, TheFollowed.position);
            if (currentPath != null)
            {
                Debug.Log("Path found!");
                StopCoroutine(FollowPath());
                StartCoroutine(FollowPath());
            }
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = gridMap.GetNode(startPos);
        Node targetNode = gridMap.GetNode(targetPos);

        if (startNode == null)
        {
            Debug.LogWarning("Start node is null");
            return;
        }

        if (targetNode == null)
        {
            Debug.LogWarning("Target node is null");
            return;
        }

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in gridMap.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
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

        currentPath = path;
        gridMap.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    IEnumerator FollowPath()
    {
        if (currentPath == null)
        {
            yield break;
        }

        for (int i = 0; i < currentPath.Count; i++)
        {
            Vector3 targetPosition = currentPath[i].worldPosition;
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * followerSpeed);

                // Calculate direction for animation (optional)
                Vector3 direction = (targetPosition - transform.position).normalized;
                float speed = direction.magnitude;

               // Update animation parameters (optional)
               animator.SetFloat("playerSpeed", speed);
               animator.SetFloat("Horizontal", direction.x);
               animator.SetFloat("Vertical", direction.y);

                yield return null;
            }
        }
    }
}
