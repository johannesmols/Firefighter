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

    private int currentlySelectedUnit = 0;

    public void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("No tilemap assigned to the Level Controller");
        }
    }

    public void Update()
    {
        playerUnits.RemoveAll(item => item == null); // remove destroyed units

        // Change unit selection
        if (Input.GetMouseButtonDown(1))
        {
            var clickedCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var unitsOnThatCell = playerUnits.Where(unit => unit.TilePosition == clickedCell && unit.ActionPoints > 0).ToList();
            if (unitsOnThatCell.Count > 0)
            {
                currentlySelectedUnit = playerUnits.IndexOf(unitsOnThatCell.First());
            }
        }
        // Movement
        else if (Input.GetMouseButtonDown(0))
        {
            if (playerUnits.Count > 0)
            {
                var clickedCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                var clickedTile = (AbstractGameTile) tilemap.GetTile(clickedCell);
                var currentUnit = playerUnits[currentlySelectedUnit];
                var reachableTiles = TilemapHelper.FindReachableTiles(currentUnit.TilePosition, currentUnit.ActionPoints, tilemap);

                int cellInFringe = -1;
                for (int fringe = 0; fringe < reachableTiles.Count; fringe++)
                {
                    if (reachableTiles[fringe].Contains(clickedCell))
                    {
                        cellInFringe = fringe;
                        break;
                    }
                }

                if (cellInFringe >= 0 && !playerUnits.Any(unit => unit.TilePosition == clickedCell))
                {
                    currentUnit.ActionPoints -= cellInFringe;
                    currentUnit.TilePosition = clickedCell;
                    currentUnit.ObjectTransform.position = new Vector3(tilemap.CellToWorld(clickedCell).x, 0, tilemap.CellToWorld(clickedCell).z);

                    // Select next unit, no AP remaining
                    if (currentUnit.ActionPoints == 0)
                    {
                        var nextUnit = playerUnits.Where(unit => unit.ActionPoints > 0).ToList();
                        if (nextUnit.Count > 0)
                        {
                            currentlySelectedUnit = playerUnits.IndexOf(nextUnit.First());
                        }
                        else
                        {
                            UpdateTiles();
                        }
                    }
                }
            }
        }
        // Skip to next turn
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
        var tiles = TilemapHelper.GetTileDictionary(tilemap);

        if (IsGameOver(tiles))
        {
            Debug.Log("Game Over!");
            return;
        }

        SpreadFire(tiles);
        ResetActionPoints();
    }

    private void SpreadFire(Dictionary<System.Type, List<Vector3Int>> tiles)
    {
        foreach (var fireTile in tiles[typeof(FireTile)])
        {
            var neighbors = TilemapHelper.FindNeighbors(fireTile, tilemap);
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

    private bool IsGameOver(Dictionary<System.Type, List<Vector3Int>> tiles) {
        if (tiles[typeof(GoalTile)].Count == 0) 
        {
            return true;
        }
        return false;
    }

    private void ResetActionPoints()
    {
        foreach (var playerUnit in playerUnits)
        {
            playerUnit.ResetActionPoints();
        }
    }
}
