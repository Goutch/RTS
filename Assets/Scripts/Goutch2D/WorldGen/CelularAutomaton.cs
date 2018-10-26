using System;

namespace Goutch2D.WorldGen
{
    public static class CelularAutomaton
    {
        private const int numberOfAdjToSurvive = 4;
        public static void GenerateBinaryMap(int[,] map, int smooth, int fillPourcent, bool edge,
            int seed)
        {
            Random rnd=new Random(seed);
            FillMapWithRandomNumbers(map, fillPourcent, edge, rnd);
            for (int i = 0; i < smooth; i++)
            {
                SmoothMap(map);
            }
        }
        public static void GenerateBinaryMap(int[,] map, int smooth, int fillPourcent, bool edge)
        {
            
            Random rnd=new Random();
            FillMapWithRandomNumbers(map, fillPourcent, edge,rnd);
            for (int i = 0; i < smooth; i++)
            {
                SmoothMap(map);
            }
        }
        static void FillMapWithRandomNumbers(int[,] map, int fillPourcent, bool edge,
            Random rnd)
        {
            int width = map.GetLength(0);
            int heigth = map.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == heigth - 1 && edge)
                        map[x, y] = 1;
                    else
                    {
                        if (map[x, y] == 0)
                            map[x, y] = (rnd.Next(0, 100) < fillPourcent) ? 1 : 0;
                    }
                }
            }
        }

        static void SmoothMap(int[,] map)
        {
            int width = map.GetLength(0);
            int heigth = map.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    int neibourWallTiles = GetSurrondingWallCount(map, x, y, width, heigth);
                    if (neibourWallTiles > numberOfAdjToSurvive)
                        map[x, y] = 1;
                    else if (neibourWallTiles < numberOfAdjToSurvive)
                        map[x, y] = 0;
                }
            }
        }

        //3x3 check
        static int GetSurrondingWallCount(int[,] map, int gridX, int gridY, int width, int heigth)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        //is in the map
                        if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < heigth)
                            wallCount += map[neighbourX, neighbourY];
                        //is outside the map
                        else wallCount++;
                    }
                }
            }

            return wallCount;
        }
    }
}