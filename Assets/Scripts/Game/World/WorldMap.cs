using System.Collections.Generic;
using Game.Map;
using InGame.World.Ressources;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class WorldMap : MonoBehaviour
    {
        public static WorldMap INSTANCE;
        [SerializeField] private Vector2Int size;
        [SerializeField] public Transform[] spawnPoints;

        [SerializeField] public Tilemap[] WalkableTilemaps;
        [SerializeField] public Tilemap[] UnWalkableTilemaps;
        [SerializeField] private RessourceTileMap gasTilemap;
        [SerializeField] public RessourceTileMap mineralTilemap;
        [SerializeField] public Pathfinder pathFinder;

        private Grid worldGrid;

        public Grid WorldGrid => worldGrid;

        private Dictionary<Vector3Int, Node> nodesMap;

        public Dictionary<Vector3Int, Node> NodesMap => nodesMap;

        public Vector2Int Size => size;

        private void Awake()
        {
            if (INSTANCE == null)
                INSTANCE = this;
            else
            {
                Destroy(this.gameObject);
            }

            worldGrid = this.GetComponent<Grid>();
            nodesMap = new Dictionary<Vector3Int, Node>();
            for (int x = -Size.x; x < Size.x; x++)
            {
                for (int y = -Size.x; y < Size.y; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    UpdateNode(position);
                }
            }
            pathFinder = new Pathfinder(this);
        }

        public void AddTile()
        {
        }

        public void UpdateNode(Vector3Int position)
        {
            bool walkable = true;

            for (int i = 0; i < UnWalkableTilemaps.Length; i++)
            {
                if (UnWalkableTilemaps[i].HasTile(position))
                    walkable = false;
            }

            nodesMap.Add(position, new Node(position, walkable));
        }
    }
}