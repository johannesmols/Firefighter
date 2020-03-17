using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using Assets.Scripts.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    public bool DynamicFireSpread = true; // todo
    public Tilemap tilemap;
    public List<AbstractUnit> playerUnits;

    private List<Vector3Int> tilesInWorldSpace;

    public void Start()
    {
        if (tilemap != null)
        {
            tilesInWorldSpace = TilemapHelper.GetTileCoordinates(tilemap);

            InvokeRepeating("UpdateTiles", 0, 1.0f);
        }
    }

    /// <summary>
    /// Update the tilemap according to specific rules (e.g. firespread)
    /// </summary>
    private void UpdateTiles()
    {
        var tiles = new Dictionary<System.Type, List<Vector3Int>>
        {
            { typeof(FireTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is FireTile).ToList() },
            { typeof(GrassTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is GrassTile).ToList() },
            { typeof(RoadTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is RoadTile).ToList() },
            { typeof(ObstacleTile), tilesInWorldSpace.Where(t => tilemap.GetTile(t) is ObstacleTile).ToList() }
        };

        SpreadFire(tiles);
    }

    private void SpreadFire(Dictionary<System.Type, List<Vector3Int>> tiles)
    {
        foreach (var fireTile in tiles[typeof(FireTile)])
        {
            var neighbors = TilemapHelper.FindNeighbors(fireTile, tiles);
            var flammableNeighbors = new List<Vector3Int>();
            foreach (var neighborType in neighbors.Keys)
            {
                foreach (var neighbor in neighbors[neighborType])
                {
                    var tile = (AbstractGameTile) tilemap.GetTile(neighbor);
                    if (tile.TileProperties.IsFlammable)
                    {
                        tilemap.SetTile(neighbor, AssetDatabase.LoadAssetAtPath("Assets/PaletteTiles/FireTile.asset", typeof(FireTile)) as FireTile);
                    }
                }
            }
        }
    }

    
}
