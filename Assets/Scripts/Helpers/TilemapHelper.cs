﻿using System;
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
        // Lookup table for even and odd rows. Hex coordinate systems are a fucking mess
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
        /// <param name="_tilemap">the tilemap</param>
        /// <returns>a list of coordinates of all tiles in the tilemap</returns>
        public static List<Vector3Int> GetTileCoordinates(Tilemap _tilemap)
        {
            var tiles = new List<Vector3Int>();
            for (int n = _tilemap.cellBounds.xMin; n < _tilemap.cellBounds.xMax; n++)
            {
                for (int p = _tilemap.cellBounds.yMin; p < _tilemap.cellBounds.yMax; p++)
                {
                    var localPlace = new Vector3Int(n, p, 0);
                    var place = _tilemap.CellToWorld(localPlace);
                    if (_tilemap.HasTile(localPlace))
                    {
                        tiles.Add(_tilemap.WorldToCell(place));
                    }
                }
            }

            return tiles;
        }

        /// <summary>
        /// Find all adjacent tiles to a tile
        /// </summary>
        /// <param name="tile">the tile in question</param>
        /// <param name="tiles">all tiles in the tilemap</param>
        /// <returns>a dictionary of neigboring tiles by type</returns>
        public static Dictionary<System.Type, List<Vector3Int>> FindNeighbors(Vector3Int tile, Dictionary<System.Type, List<Vector3Int>> tiles)
        {
            var rowIsEven = tile.y % 2 == 0;
            var neigbors = new Dictionary<System.Type, List<Vector3Int>>();
            foreach (var type in tiles.Keys)
            {
                var neighborsWithType = new List<Vector3Int>();
                var tilesWithType = tiles[type];
                foreach (var tileWithType in tilesWithType)
                {
                    int x = tileWithType.x - tile.x;
                    int y = tileWithType.y - tile.y;

                    if (rowIsEven)
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
