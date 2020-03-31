using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
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
    private bool isGameOver = false;
    private bool levelIsComplete = false;

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

                var currentUnit = playerUnits.Find(a => a == unitsOnThatCell.First());
                Debug.Log("Changed selection to " + currentUnit.name + ", " + currentUnit.ActionPoints + " AP left");
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
                Tuple<Vector3Int, int> targetTile = null;
                for (int fringe = 0; fringe < reachableTiles.Count; fringe++)
                {
                    var reachableTile = reachableTiles[fringe].Where(tile => tile.Item1 == clickedCell).ToList();
                    if (reachableTile.Count > 0)
                    {
                        targetTile = reachableTile.First();
                        cellInFringe = fringe;
                        break;
                    }
                }

                if (cellInFringe >= 0 && !playerUnits.Any(unit => unit.TilePosition == clickedCell) && currentUnit.ActionPoints >= targetTile?.Item2)
                {
                    Debug.Log("Moved unit " + currentUnit.name + " from " + currentUnit.TilePosition + " to " + clickedCell + ", costing " + targetTile.Item2 + ". There are " + (currentUnit.ActionPoints - targetTile.Item2) + " AP left.");

                    currentUnit.ActionPoints -= targetTile.Item2;
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
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[1], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[2], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[3], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[4], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[5], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[6], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[7], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[8], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
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

        if (!levelIsComplete && !isGameOver)
        {
            Debug.Log("Starting next round");
            SpreadFire(tiles);
            ResetActionPoints();

            isGameOver = IsGameOver(TilemapHelper.GetTileDictionary(tilemap));
        }
    }

    private void SpreadFire(Dictionary<System.Type, List<Vector3Int>> tiles)
    {
        var newTilesOnFireCnt = 0;
        foreach (var fireTile in tiles[typeof(FireTile)])
        {
            tilemap.SetTile(fireTile, Resources.Load("BurntTile", typeof(BurntTile)) as BurntTile);
            var neighbors = TilemapHelper.FindNeighbors(fireTile, tilemap);
            var flammableNeighbors = new List<Vector3Int>();
            foreach (var neighborType in neighbors.Keys)
            {
                foreach (var neighbor in neighbors[neighborType])
                {
                    var tile = (AbstractGameTile) tilemap.GetTile(neighbor);
                    if (tile.TileProperties.IsFlammable)
                    {
                        tilemap.SetTile(neighbor, Resources.Load("FireTile", typeof(FireTile)) as FireTile);
                        // replace goal tile with burntGoalTile  
                        newTilesOnFireCnt++;
                    }
                }
            }
        }

        if (newTilesOnFireCnt == 0)
        {
            levelIsComplete = true;
            Debug.Log("Level complete, no fire can spread anymore");
        }
    }

    private void ExecuteAction(Tuple<string, int> action, UnitType unitType, AbstractUnit unit)
    {
        if (action != null)
        {
            switch(action.Item1)
            {
                case "dig_trench":
                    if (unitType != UnitType.Digger)
                        return;
                    var standingOn = (AbstractGameTile) tilemap.GetTile(unit.TilePosition);
                    if (unit.ActionPoints >= action.Item2 && standingOn.TileProperties.IsMovable && !standingOn.TileProperties.IsGoal && standingOn.TileProperties.IsFlammable)
                    {
                        tilemap.SetTile(unit.TilePosition, Resources.Load("TrenchTile", typeof(TrenchTile)) as TrenchTile);
                        unit.ActionPoints -= action.Item2;

                        Debug.Log("Placed trench on tile " + unit.TilePosition + ", costing " + action.Item2 + " APs. There are " + (unit.ActionPoints) + " AP left.");
                    }
                    break;
            }
        }
    }

    private bool IsGameOver(Dictionary<System.Type, List<Vector3Int>> tiles) {
        if (tiles[typeof(GoalTile)].Count == 0) 
        {
            Debug.Log("Game over!");
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
