using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder INSTANCE = null;

    private static Grid grid;

    [SerializeField] private Tilemap walkable;
    [SerializeField] private Tilemap unWalkable;

    private Dictionary<Vector3Int,Node> nodesMap;


    // Use this for initialization
    class Node
    {
        public bool walkable;

        public Vector3Int gridPos;

        public Vector2 worldPos;

        //distance from start
        public int nodeToStartCost;

        //distance from destination
        public int nodeToEndCost;

        //H+G;
        public int totalCost
        {
            get { return nodeToEndCost + nodeToStartCost; }
        }

        public Node parent;

        public Node(Vector3Int position, bool walkable)
        {
            gridPos = position;
            this.walkable = walkable;
            worldPos = grid.CellToWorld(position);
            worldPos = new Vector3(worldPos.x + .16f, worldPos.y + .16f, 0);
        }
    }

    void Awake()
    {
        //Check if instance already exists
        if (INSTANCE == null)

            //if not, set instance to this
            INSTANCE = this;

        //If instance already exists and it's not this:
        else if (INSTANCE != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);


        grid = GetComponent<Grid>();
        nodesMap =new Dictionary<Vector3Int, Node>();
        //todo:find a way to make it work with uneaven tilemaps
        
        for (int x = -walkable.size.x/2; x < walkable.size.x/2 ; x++)
        {
            for (int y = -walkable.size.y/2; y < walkable.size.y /2; y++)
            {
                nodesMap.Add(new Vector3Int(x,y,0), new Node(new Vector3Int(x, y, 0), !unWalkable.HasTile(new Vector3Int(x, y, 0))));
            }
        }
    }


    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = nodesMap[grid.WorldToCell(startPos)];
        Node targetNode = nodesMap[grid.WorldToCell(targetPos)];

        if (targetNode.walkable == false)
            return null;
        //nodes already visited
        List<Node> visitedNodes = new List<Node>();

        //currently visitable nodes
        List<Node> visitableNodes = new List<Node>();
        visitableNodes.Add(startNode);
        while (visitableNodes.Any())
        {
            if (visitableNodes.Count > 2500)
            {
                return null;
            }

            Node currentNode = visitableNodes[0];
            for (int i = 0; i < visitableNodes.Count; i++)
            {
                //if(total cost decrease pick this node or if equal to the currenNode total cost
                if (visitableNodes[i].totalCost < currentNode.totalCost ||
                    visitableNodes[i].totalCost == currentNode.totalCost)
                    //if(have the same total cost just pick the closest to the end)
                    if (visitableNodes[i].nodeToEndCost < currentNode.nodeToEndCost)
                    {
                        //currenly looking neighbour node is set
                        currentNode = visitableNodes[i];
                    }
            }

            //remove from visiting option
            visitableNodes.Remove(currentNode);
            //have been visited
            visitedNodes.Add(currentNode);

            //found target
            if (currentNode.gridPos == targetNode.gridPos)
            {
                return RetracePath(startNode, currentNode);
            }

            //for every neighbour of the current node 
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || visitedNodes.Contains(neighbour))
                {
                    //next for loop iteration
                    continue;
                }

                //calculate costs
                int movementCostToNeighbour =
                    currentNode.nodeToStartCost + GetDistanceBetweenNodes(currentNode, neighbour);

                //if found faster path
                if (movementCostToNeighbour < neighbour.nodeToStartCost || !visitableNodes.Contains(neighbour))
                {
                    //recalculate costs
                    neighbour.nodeToStartCost = movementCostToNeighbour;
                    neighbour.nodeToEndCost = GetDistanceBetweenNodes(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!visitableNodes.Contains(neighbour))
                    {
                        visitableNodes.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private List<Vector3> RetracePath(Node startNode, Node EndNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = EndNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPos);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int disY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);
        if (disX > disY)
            return 14 * disY + 10 * (disX - disY);
        return 14 * disX + 10 * (disY - disX);
    }

    private List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;
                if (checkX > -walkable.size.x/2 && checkX <walkable.size.x/2 && checkY >-walkable.size.y/2&& checkY < walkable.size.y/2)
                {
                    Vector3Int position = new Vector3Int(checkX, checkY, 0);
                    neighbours.Add(nodesMap[position]);
                }
            }
        }

        return neighbours;
    }
}