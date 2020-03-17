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
    private int currentlySelectedUnit = 0;

    public void Start()
    {
        if (tilemap != null)
        {
            tilesInWorldSpace = TilemapHelper.GetTileCoordinates(tilemap);
        }
        else
        {
            Debug.LogError("No tilemap assigned to the Level Controller");
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var clickedCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var currentUnit = playerUnits[currentlySelectedUnit];
            var distance = TilemapHelper.GetDistanceBetweenTiles(currentUnit.TilePosition, clickedCell);
            if (distance <= currentUnit.ActionPoints)
            {
                currentUnit.ActionPoints -= distance;
                currentUnit.TilePosition = clickedCell;
                currentUnit.ObjectTransform.position = new Vector3(tilemap.CellToWorld(clickedCell).x, 0, tilemap.CellToWorld(clickedCell).z);

                if (currentUnit.ActionPoints == 0)
                {
                    Debug.Log("Action points for unit " + currentUnit + " exhausted, moving on to the next");

                    if (currentlySelectedUnit < playerUnits.Count - 1)
                    {
                        currentlySelectedUnit++;
                    }
                    else
                    {
                        currentlySelectedUnit = 0;
                        UpdateTiles();
                    }
                }
            }
        }
        else if (Input.GetKeyDown("space"))
        {
            currentlySelectedUnit = 0;
            UpdateTiles();
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
        ResetActionPoints();
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

    private void ResetActionPoints()
    {
        foreach (var playerUnit in playerUnits)
        {
            playerUnit.ResetActionPoints();
        }
    }
}
