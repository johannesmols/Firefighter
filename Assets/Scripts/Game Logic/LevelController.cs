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
    public bool dynamicFireSpread = true; // todo
    public Tilemap tilemap;
    public List<AbstractUnit> playerUnits;
    public GameObject UnitSelector;
    public AudioController audioController;

    public int currentlySelectedUnit = 0;
    private bool isGameOver = false;
    private bool levelIsComplete = false;
    private System.Random random = new System.Random();

    public void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("No tilemap assigned to the Level Controller");
        }

        audioController.PlayMissionStartSound();
    }

    public void Update()
    {
        playerUnits.RemoveAll(item => item == null); // remove destroyed units

        // Change unit selection
        if (Input.GetMouseButtonDown(0))
        {
            var clickedCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var unitsOnThatCell = playerUnits.Where(unit => unit.TilePosition == clickedCell && unit.ActionPoints > 0).ToList();
            if (unitsOnThatCell.Count > 0)
            {
                currentlySelectedUnit = playerUnits.IndexOf(unitsOnThatCell.First());

                var currentUnit = playerUnits.Find(a => a == unitsOnThatCell.First());
                Debug.Log("Changed selection to " + currentUnit.name + ", " + currentUnit.ActionPoints + " AP left");
                audioController.PlayUnitChooseSound();
            }
        }
        // Movement
        else if (Input.GetMouseButtonDown(1))
        {
            if (playerUnits.Count > 0)
            {
                var clickedCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                var clickedTile = (AbstractGameTile) tilemap.GetTile(clickedCell);
                var currentUnit = playerUnits[currentlySelectedUnit];
                var reachableTiles = TilemapHelper.FindReachableTiles(currentUnit.TilePosition, currentUnit.ActionPoints, tilemap);
                var reachableFireTiles = TilemapHelper.FindReachableFireTiles(currentUnit.TilePosition, currentUnit.ActionPoints, tilemap);

                int cellInFringe = -1;
                int fireCellInFringe = -1;
                Tuple<Vector3Int, int> targetTile = null;
                Tuple<Vector3Int, int> targetFireTile = null;
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
                for (int fringe = 0; fringe < reachableFireTiles.Count; fringe++)
                {
                    
                    var reachableFireTile = reachableFireTiles[fringe].Where(tile => tile.Item1 == clickedCell).ToList();
                    if (reachableFireTile.Count > 0)
                    {
                        targetFireTile = reachableFireTile.First();
                        fireCellInFringe = fringe;
                        break;
                    }
                }

                if (cellInFringe >= 0 && !playerUnits.Any(unit => unit.TilePosition == clickedCell) && (currentUnit.ActionPoints >= targetTile?.Item2 || currentUnit.ActionPoints >= targetFireTile?.Item2))
                {
                        Debug.Log("Moved unit " + currentUnit.name + " from " + currentUnit.TilePosition + " to " + clickedCell + ", costing " + targetTile.Item2 + ". There are " + (currentUnit.ActionPoints - targetTile.Item2) + " AP left.");
                        currentUnit.ActionPoints -= targetTile.Item2;
                        currentUnit.TilePosition = clickedCell;
                        currentUnit.ObjectTransform.position = new Vector3(tilemap.CellToWorld(clickedCell).x, 0, tilemap.CellToWorld(clickedCell).z);

                        audioController.PlayUnitMoveSound();

                        // Select next unit, no AP remaining
                        ChooseNextUnitOrGoToNextRound();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        // Skip to next turn
        else if (Input.GetKeyDown("space"))
        {
            //currentlySelectedUnit = 0;
            UpdateTiles();
        }

        UnitSelector.transform.position = playerUnits[currentlySelectedUnit].transform.position;
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
            SpreadFire(tiles, dynamicFireSpread);
            ResetActionPoints();

            isGameOver = IsGameOver(TilemapHelper.GetTileDictionary(tilemap));
        }
    }

    private void ChooseNextUnitOrGoToNextRound()
    {
        var currentUnit = playerUnits[currentlySelectedUnit];
        if (currentUnit.ActionPoints == 0)
        {
            var nextUnit = playerUnits.Where(unit => unit.ActionPoints > 0).ToList();
            if (nextUnit.Count > 0)
            {
                currentlySelectedUnit = playerUnits.IndexOf(nextUnit.First());
                //audioController.PlayUnitChooseSound();
            }
            else
            {
                UpdateTiles();
            }
        }
    }

    private void SpreadFire(Dictionary<System.Type, List<Vector3Int>> tiles, bool dynamicSpread)
    {
        var newTilesOnFireCnt = 0;
        foreach (var fireTile in tiles[typeof(FireTile)])
        {
            var currentFireTile = (AbstractGameTile) tilemap.GetTile(fireTile);
            currentFireTile.TileProperties.RoundsOnFire++;
            if (currentFireTile.TileProperties.RoundsOnFire >= 1)
            {
                tilemap.SetTile(fireTile, Resources.Load("BurntTile", typeof(BurntTile)) as BurntTile);
            }

            if (dynamicSpread)
            {
                var reachableNeighbors = TilemapHelper.FindReachableTiles(fireTile, 1, tilemap);
                if (reachableNeighbors.Count >= 2)
                {
                    var possibleTilesToSpreadTo = reachableNeighbors[1];
                    var spreadToThisManyTiles = (possibleTilesToSpreadTo.Count - 1) / 2 + 1; // divide by two, but round up
                    var spreadTo = possibleTilesToSpreadTo.OrderBy(x => random.Next()).Take(spreadToThisManyTiles); // randomly choose elements from list
                    foreach (var tile in spreadTo)
                    {
                        var actualTile = (AbstractGameTile) tilemap.GetTile(tile.Item1);
                        if (actualTile.TileProperties.IsFlammable)
                        {
                            tilemap.SetTile(tile.Item1, Resources.Load("FireTile", typeof(FireTile)) as FireTile);
                            newTilesOnFireCnt++;
                        }
                    }
                }
            }
            else
            {
                var neighbors = TilemapHelper.FindNeighbors(fireTile, tilemap);
                foreach (var neighborType in neighbors.Keys)
                {
                    foreach (var neighbor in neighbors[neighborType])
                    {
                        var tile = (AbstractGameTile)tilemap.GetTile(neighbor);
                        if (tile.TileProperties.IsFlammable)
                        {
                            tilemap.SetTile(neighbor, Resources.Load("FireTile", typeof(FireTile)) as FireTile);
                            // replace goal tile with burntGoalTile  
                            newTilesOnFireCnt++;
                        }
                    }
                }
            }
        }

        if (newTilesOnFireCnt == 0)
        {
            levelIsComplete = true;
            Debug.Log("Level complete, no fire can spread anymore");
            audioController.PlayMissionSuccessSound();
        }
        else
        {
            audioController.PlayFireSpreadSound();
        }
    }

    private void ExecuteAction(Tuple<string, int> action, UnitType unitType, AbstractUnit unit)
    {
        if (action != null)
        {
            switch(action.Item1)
            {
                case "dig_trench":
                    {
                        if (unitType != UnitType.Digger)
                            return;

                        var standingOn = (AbstractGameTile)tilemap.GetTile(unit.TilePosition);
                        if (unit.ActionPoints >= action.Item2 && standingOn.TileProperties.IsMovable && !standingOn.TileProperties.IsGoal && standingOn.TileProperties.IsFlammable)
                        {
                            tilemap.SetTile(unit.TilePosition, Resources.Load("TrenchTile", typeof(TrenchTile)) as TrenchTile);
                            unit.ActionPoints -= action.Item2;

                            audioController.PlayDigTrenchSound();

                            Debug.Log("Placed trench on tile " + unit.TilePosition + ", costing " + action.Item2 + " APs. There are " + (unit.ActionPoints) + " AP left.");
                        }
                    }
                    break;
                case "extinguish_fire":
                    {
                        if (unitType != UnitType.FireTruck && unitType != UnitType.Firefighter)
                            return;

                        var extinguished = 0;
                        var fireTilesInRange = TilemapHelper.FindReachableFireTiles(unit.TilePosition, unit.ReachableFire, tilemap);
                        if (unit.ActionPoints >= action.Item2)
                        {
                            for (int i = 1; i < fireTilesInRange.Count; i++)
                            {
                                foreach (var tile in fireTilesInRange[i])
                                {
                                    tilemap.SetTile(tile.Item1, Resources.Load("BurntTile", typeof(BurntTile)) as BurntTile);
                                    extinguished++;
                                }
                            }

                            if (extinguished > 0)
                            {
                                unit.ActionPoints -= action.Item2;
                                audioController.PlayExtinguishFireSound();
                            }
                        }
                    }
                    break;
            }
    
        }

        ChooseNextUnitOrGoToNextRound();
    }

    private bool IsGameOver(Dictionary<System.Type, List<Vector3Int>> tiles) {
        if (tiles[typeof(GoalTile)].Count == 0) 
        {
            Debug.Log("Game over!");
            audioController.PlayMissionFailSound();
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
