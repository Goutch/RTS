using UnityEngine;

namespace Game.Map
{
    public class Node
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
            worldPos = WorldMap.INSTANCE.WorldGrid.CellToWorld(position);
            worldPos = new Vector3(worldPos.x + .16f, worldPos.y + .16f, 0);
        }
    }
}