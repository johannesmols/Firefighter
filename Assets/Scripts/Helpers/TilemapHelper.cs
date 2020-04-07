using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Helpers
{
    public class TilemapHelper
    {
        // Lookup table for even and odd rows
        // https://www.redblobgames.com/grids/hexagons/#neighbors-offset
        public static readonly List<Tuple<int, int>> evenRowLookupTable = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(-1, 1),
            new Tuple<int, int>(0, 1),
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(0, -1),
            new Tuple<int, int>(-1, -1),
            new Tuple<int, int>(-1, 0)
        };

        public static readonly List<Tuple<int, int>> oddRowLookupTable = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(0, 1),
            new Tuple<int, int>(1, 1),
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(1, -1),
            new Tuple<int, int>(0, -1),
            new Tuple<int, int>(-1, 0)
        };

        /// <summary>
        /// Get a list of tile coordinates of all tiles in the tilemap
        /// </summary>
        /// <param name="tilemap">the tilemap</param>
        /// <returns>a list of coordinates of all tiles in the tilemap</returns>
        public static List<Vector3Int> GetTileCoordinates(Tilemap tilemap)
        {
            var tiles = new List<Vector3Int>();
            for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++)
            {
                for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++)
                {
                    var localPlace = new Vector3Int(n, p, 0);
                    var place = tilemap.CellToWorld(localPlace);
                    if (tilemap.HasTile(localPlace))
                    {
                        tiles.Add(tilemap.WorldToCell(place));
                    }
                }
            }

            return tiles;
        }

        public static Dictionary<System.Type, List<Vector3Int>> GetTileDictionary(Tilemap tilemap)
        {
            var tilesInWorldSpace = GetTileCoordinates(tilemap);
            return new Dictionary<System.Type, List<Vector3Int>>
            {
                { typeof(FireTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is FireTile).ToList() },
                { typeof(GrassTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is GrassTile).ToList() },
                { typeof(RoadTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is RoadTile).ToList() },
                { typeof(ObstacleTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is ObstacleTile).ToList() },
                { typeof(GoalTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is GoalTile).ToList() },
                { typeof(TrenchTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is TrenchTile).ToList() },
                { typeof(BurntTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is BurntTile).ToList() }
            };
        }

        public static bool IsEvenRow(Vector3Int coordinates)
        {
            return coordinates.y % 2 == 0;
        }

        // https://www.redblobgames.com/grids/hexagons/#conversions
        public static Vector3Int ConvertOffsetToCubeCoordinates(Vector3Int offset)
        {
            int x, y, z;
            if (IsEvenRow(offset))
            {
                x = offset.x - (offset.y + (offset.y & 1)) / 2;
                z = offset.y;
                y = -x - z;

                
            }
            else
            {
                x = offset.x - (offset.y - (offset.y & 1)) / 2;
                z = offset.y;
                y = -x - z;
            }

            return new Vector3Int(x, y, z);
        }

        // Does not consider non-movable tiles (e.g. obstacles)
        // https://www.redblobgames.com/grids/hexagons/#distances-cube
        public static int GetDistanceBetweenTiles(Vector3Int a, Vector3Int b)
        {
            a = ConvertOffsetToCubeCoordinates(a);
            b = ConvertOffsetToCubeCoordinates(b);
            return Mathf.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y), Math.Abs(a.z - b.z));
        }

        // https://www.redblobgames.com/grids/hexagons/#range-obstacles
        public static List<List<Tuple<Vector3Int, int>>> FindReachableTiles(Vector3Int startTile, int maxDistance, Tilemap tilemap)
        {
            var visited = new List<Vector3Int> { startTile };
            var fringes = new List<List<Tuple<Vector3Int, int>>> { new List<Tuple<Vector3Int, int>> { new Tuple<Vector3Int, int>(startTile, 0) } };

            for (int i = 1; i <= maxDistance; i++)
            {
                fringes.Add(new List<Tuple<Vector3Int, int>>());
                foreach (var hex in fringes[i - 1])
                {
                    var neighbors = FindNeighbors(hex.Item1, tilemap);
                    foreach (var neighbor in neighbors.SelectMany(n => n.Value))
                    {
                        var neighborTile = (AbstractGameTile) tilemap.GetTile(neighbor);
                        if (!visited.Contains(neighbor) && neighborTile.TileProperties.IsMovable)
                        {
                            visited.Add(neighbor);
                            fringes[i].Add(new Tuple<Vector3Int, int>(neighbor, hex.Item2 + neighborTile.TileProperties.MovementCost));
                        }
                    }
                }
            }

            return fringes;
        }

        public static List<List<Tuple<Vector3Int, int>>> FindReachableFireTiles(Vector3Int startTile, int maxDistance, Tilemap tilemap)
        {
            var visited = new List<Vector3Int> { startTile };
            var fringes = new List<List<Tuple<Vector3Int, int>>> { new List<Tuple<Vector3Int, int>> { new Tuple<Vector3Int, int>(startTile, 0) } };

            for (int i = 1; i <= maxDistance; i++)
            {
                fringes.Add(new List<Tuple<Vector3Int, int>>());
                foreach (var hex in fringes[i - 1])
                {
                    var neighbors = FindNeighbors(hex.Item1, tilemap);
                    foreach (var neighbor in neighbors.SelectMany(n => n.Value))
                    {
                        var neighborTile = (AbstractGameTile) tilemap.GetTile(neighbor);
                        if (!visited.Contains(neighbor) && (neighborTile.TileProperties.IsMovable || neighborTile is FireTile))
                        {
                            visited.Add(neighbor);
                            fringes[i].Add(new Tuple<Vector3Int, int>(neighbor, hex.Item2 + neighborTile.TileProperties.MovementCost));
                        }
                    }
                }
            }

            // Remove non-fire tiles
            foreach (var fringe in fringes)
            {
                fringe.RemoveAll(item => ((AbstractGameTile) tilemap.GetTile(item.Item1)) is FireTile == false);
            }

            return fringes;
        }

        /// <summary>
        /// Find all adjacent tiles to a tile
        /// </summary>
        /// <param name="tile">the tile in question</param>
        /// <param name="tiles">all tiles in the tilemap</param>
        /// <returns>a dictionary of neigboring tiles by type</returns>
        public static Dictionary<System.Type, List<Vector3Int>> FindNeighbors(Vector3Int tile, Tilemap tilemap)
        {
            var tiles = GetTileDictionary(tilemap);
            var neigbors = new Dictionary<System.Type, List<Vector3Int>>();
            foreach (var type in tiles.Keys)
            {
                var neighborsWithType = new List<Vector3Int>();
                var tilesWithType = tiles[type];
                foreach (var tileWithType in tilesWithType)
                {
                    int x = tileWithType.x - tile.x;
                    int y = tileWithType.y - tile.y;

                    if (IsEvenRow(tile))
                    {
                        if (evenRowLookupTable.Contains(new Tuple<int, int>(x, y)))
                        {
                            neighborsWithType.Add(tileWithType);
                        }
                    }
                    else
                    {
                        if (oddRowLookupTable.Contains(new Tuple<int, int>(x, y)))
                        {
                            neighborsWithType.Add(tileWithType);
                        }
                    }
                }

                neigbors.Add(type, neighborsWithType);
            }

            return neigbors;
        }
    }
}
