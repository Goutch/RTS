using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Goutch2D.Pathfinding
{
    public class AStar
    {
        private const int MAX_OPEN_NODES = 2500;
        private Dictionary<Vector3Int,AStarNode> nodesMap;
        private Grid grid;

        private int maxX;

        private int maxY;
        // Use this for initialization
        public AStar(Dictionary<Vector3Int,AStarNode> nodesMap, Grid grid,int maxX,int maxY)
        {
            this.nodesMap = nodesMap;
            this.grid = grid;
            this.maxX = maxX;
            this.maxY = maxY;
        }


        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            AStarNode startNode = nodesMap[grid.WorldToCell(startPos)];
            AStarNode targetNode = nodesMap[grid.WorldToCell(targetPos)];

            if (targetNode.walkable == false)
                return null;
            //nodes already visited
            List<AStarNode> visitedNodes = new List<AStarNode>();
            //currently visitable nodes
            List<AStarNode> visitableNodes = new List<AStarNode>();
            visitableNodes.Add(startNode);


            while (visitableNodes.Any())
            {
                if (visitableNodes.Count > MAX_OPEN_NODES)
                {
                    return null;
                }

                AStarNode currentNode = visitableNodes[0];
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
                foreach (AStarNode neighbour in GetAdjNeighbours(currentNode))
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

        private List<Vector3> RetracePath(AStarNode startNode, AStarNode EndNode)
        {
            List<Vector3> path = new List<Vector3>();
            AStarNode currentNode = EndNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode.worldPos);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

        private int GetDistanceBetweenNodes(AStarNode nodeA, AStarNode nodeB)
        {
            int disX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
            int disY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);
            if (disX > disY)
                return 14 * disY + 10 * (disX - disY);
            return 14 * disX + 10 * (disY - disX);
        }

        private List<AStarNode> GetAdjNeighbours(AStarNode node)
        {
            List<AStarNode> neighbours = new List<AStarNode>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Mathf.Abs(x) == Mathf.Abs(y))
                        continue;

                    int checkX = node.gridPos.x + x;
                    int checkY = node.gridPos.y + y;
                    if (checkX > -maxX && checkX < maxX && checkY > -maxY &&
                        checkY < maxY)
                    {
                        Vector3Int position = new Vector3Int(checkX, checkY, 0);
                        neighbours.Add(nodesMap[position]);
                    }
                }
            }

            return neighbours;
        }

        private List<AStarNode> GetNeighbours(AStarNode node)
        {
            List<AStarNode> neighbours = new List<AStarNode>();
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
                    if (checkX > -maxX && checkX < maxX && checkY > -maxY &&
                        checkY < maxY)
                    {
                        Vector3Int position = new Vector3Int(checkX, checkY, 0);
                        neighbours.Add(nodesMap[position]);
                    }
                }
            }

            return neighbours;
        }
    }
}