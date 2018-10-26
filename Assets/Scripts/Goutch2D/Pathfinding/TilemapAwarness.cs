using UnityEngine;
using UnityEngine.Tilemaps;

namespace Goutch2D.Pathfinding
{
    public class TilemapAwarness:MonoBehaviour
    {
        [SerializeField] private Tilemap obstacles;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="myPosition"></param>
        /// <returns>True if there is an obstacle tile on the grid position relative to the local position</returns>
        public bool[,] GetSurrounding(int range,Vector2 myPosition)
        {
            bool[,] surrounding=new bool[range*2+1,range*2+1];
            Vector3Int StartPos=obstacles.WorldToCell(myPosition);
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {               
                    int cellPosX = StartPos.x + x;
                    int cellPosY = StartPos.y + y;
                    surrounding[x+range,y+range]=obstacles.GetTile(new Vector3Int(cellPosX,cellPosY,0));
                }
            }
            return surrounding;
        }
    }
}