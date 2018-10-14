using System;
using System.Collections.Generic;
using Boo.Lang;
using Game;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InGame.World.Ressources
{
    public class RessourceTileMap : MonoBehaviour
    {
        [SerializeField] private RessourceData dataPrefab;
        private RessourceType type;
        private Tilemap tilemap;
        private Dictionary<Vector3Int, RessourceData> ressources;
        [SerializeField] private Tile FullTile;
        [SerializeField] private Tile UsedTile;
        [SerializeField] private Tile AlmostEmptyTile;
        
        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            type = dataPrefab.Type;
            for (int x = -WorldMap.INSTANCE.Size.x; x < WorldMap.INSTANCE.Size.x; x++)
            {
                for (int y = -WorldMap.INSTANCE.Size.x; y < WorldMap.INSTANCE.Size.y; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(position))
                    {
                        ressources.Add(position, Instantiate(dataPrefab));
                        ressources[position].Init(this);
                    }
                }
            }
        }
        public void OnRessourceDepleded(Vector3Int position)
        {
            tilemap.SetTile(position, null);
            ressources.Remove(position);
            WorldMap.INSTANCE.UpdateNode(position);
        }

        public int Harvest(Vector3Int worldGridPos,int ammount)
        {
           return ressources[worldGridPos].Harvest(ammount);
        }

        public void TileUsed(Vector3Int position)
        {
            tilemap.SetTile(position,UsedTile);
        }

        public void TileAlmostEmpty(Vector3Int position)
        {
            tilemap.SetTile(position,AlmostEmptyTile);
        }
    }
}