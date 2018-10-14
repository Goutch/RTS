using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Map;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder
{
    private WorldMap worldMap;

    // Use this for initialization
    public Pathfinder(WorldMap map)
    {
        worldMap = map;
    }


    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = worldMap.NodesMap[worldMap.WorldGrid.WorldToCell(startPos)];
        Node targetNode = worldMap.NodesMap[worldMap.WorldGrid.WorldToCell(targetPos)];

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
            foreach (Node neighbour in GetAdjNeighbours(currentNode))
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

    private List<Node> GetAdjNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(Mathf.Abs(x)==Mathf.Abs(y))
                    continue;

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;
                if (checkX > -worldMap.Size.x && checkX < worldMap.Size.x && checkY > -worldMap.Size.y  &&
                    checkY < worldMap.Size.y )
                {
                    Vector3Int position = new Vector3Int(checkX, checkY, 0);
                    neighbours.Add(worldMap.NodesMap[position]);
                }
            }
        }

        return neighbours;
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
                if (checkX > -worldMap.Size.x && checkX < worldMap.Size.x && checkY > -worldMap.Size.y  &&
                    checkY < worldMap.Size.y )
                {
                    Vector3Int position = new Vector3Int(checkX, checkY, 0);
                    neighbours.Add(worldMap.NodesMap[position]);
                }
            }
        }

        return neighbours;
    }
}