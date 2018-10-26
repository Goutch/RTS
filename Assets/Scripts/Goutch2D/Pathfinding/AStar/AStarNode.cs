using Game;
using UnityEngine;

namespace Goutch2D.Pathfinding
{
    public class AStarNode
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

        public AStarNode parent;

        public AStarNode(Vector3Int position, bool walkable)
        {
            gridPos = position;
            this.walkable = walkable;
            worldPos = WorldMap.INSTANCE.WorldGrid.CellToWorld(position);
            worldPos = new Vector3(worldPos.x + .5f, worldPos.y + .5f, 0);
        }
    }
}