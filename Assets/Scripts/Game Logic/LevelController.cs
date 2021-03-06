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
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    public bool dynamicFireSpread = true;
    public Tilemap tilemap;
    public List<AbstractUnit> playerUnits;
    public GameObject UnitSelector;
    public GameObject RangeDisplayHelper;
    public GameObject GrassProps;
    public GameObject BurnedProps;
    public AudioController audioController;
    public Camera mainCamera;

    [HideInInspector]
    public int currentlySelectedUnit = 0;
    private bool isGameOver = false;
    private bool levelIsComplete = false;
    private System.Random random = new System.Random();
    private bool calledRangeDisplayHelperOnce = false;

    private List<Tuple<Vector3Int, GameObject>> grassPropsList = new List<Tuple<Vector3Int, GameObject>>();
    private List<Tuple<Vector3Int, GameObject>> burnedPropsList = new List<Tuple<Vector3Int, GameObject>>();

    private readonly List<string> levelOrder = new List<string>() { "Tutorial", "Level 1", "Level 2", "Level 3" };

    public void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("No tilemap assigned to the Level Controller");
        }

        audioController.PlayMissionStartSound();

        mainCamera.transform.position = new Vector3(playerUnits[currentlySelectedUnit].transform.position.x, mainCamera.transform.position.y, playerUnits[currentlySelectedUnit].transform.position.z - 5f);

        CreateRandomTileProps();
    }

    public void Update()
    {
        playerUnits.RemoveAll(item => item == null); // remove destroyed units
        if (playerUnits.Count == 0)
        {
            currentlySelectedUnit = -1;
        }

        if (!calledRangeDisplayHelperOnce)
        {
            DisplayReachableTilesForCurrentUnit();
            calledRangeDisplayHelperOnce = true;
        }

        // Change unit selection
        if (Input.GetMouseButtonDown(0))
        {
            var clickedCell = GetTileClick();
            var unitsOnThatCell = playerUnits.Where(unit => unit.TilePosition == clickedCell && unit.ActionPoints > 0).ToList();
            if (unitsOnThatCell.Count > 0)
            {
                currentlySelectedUnit = playerUnits.IndexOf(unitsOnThatCell.First());

                var currentUnit = playerUnits.Find(a => a == unitsOnThatCell.First());

                DisplayReachableTilesForCurrentUnit();

                Debug.Log("Changed selection to " + currentUnit.name + ", " + currentUnit.ActionPoints + " AP left");
                audioController.PlayUnitChooseSound();
            }
        }
        // Movement
        else if (Input.GetMouseButtonDown(1))
        {
            if (playerUnits.Count > 0)
            {
                var clickedCell = GetTileClick();
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

                        DisplayReachableTilesForCurrentUnit();

                        // Select next unit, no AP remaining
                        ChooseNextUnitOrGoToNextRound();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ExecuteAction(playerUnits[currentlySelectedUnit].UnitActions[0], playerUnits[currentlySelectedUnit].UnitType, playerUnits[currentlySelectedUnit]);
        }
        // Skip to next turn
        else if (Input.GetKeyDown("space"))
        {
            //currentlySelectedUnit = 0;
            UpdateTiles();
        }

        if (currentlySelectedUnit != -1)
        {
            UnitSelector.transform.position = playerUnits[currentlySelectedUnit].transform.position;
        }
        else
        {
            Destroy(UnitSelector);
        }
    }

    /// <summary>
    /// Update the tilemap according to specific rules (e.g. firespread)
    /// </summary>
    public void UpdateTiles()
    {
        var tiles = TilemapHelper.GetTileDictionary(tilemap);

        if (!levelIsComplete && !isGameOver)
        {
            Debug.Log("Starting next round");
            StartCoroutine(SpreadFire(tiles, dynamicFireSpread));
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
                StartCoroutine(LerpCameraTo(mainCamera.transform.position, new Vector3(nextUnit.First().transform.position.x, mainCamera.transform.position.y, nextUnit.First().transform.position.z - 5f), 0.5f, 0.5f));
                DisplayReachableTilesForCurrentUnit();

                //audioController.PlayUnitChooseSound();
            }
            else
            {
                UpdateTiles();
            }
        }
    }

    private IEnumerator SpreadFire(Dictionary<System.Type, List<Vector3Int>> tiles, bool dynamicSpread)
    {
        // find fire epicenter
        var fireTiles = TilemapHelper.GetTileDictionary(tilemap)[typeof(FireTile)];
        if (fireTiles.Count > 0)
        {
            var average = fireTiles.Aggregate((acc, cur) => acc + cur) / fireTiles.Count;

            StartCoroutine(LerpCameraTo(mainCamera.transform.position, new Vector3(tilemap.CellToWorld(average).x, mainCamera.transform.position.y, tilemap.CellToWorld(average).z - 5f), 0.5f, 0.5f));
            yield return new WaitForSecondsRealtime(1f);
        }

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
                    var spreadToThisManyTiles = (int) Math.Ceiling(possibleTilesToSpreadTo.Count * 0.66f);
                    //var spreadToThisManyTiles = (possibleTilesToSpreadTo.Count - 1) / 2 + 1; // divide by two, but round up
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

            StartCoroutine(StartNextLevelDelayed(3));
        }
        else
        {
            audioController.PlayFireSpreadSound();
        }

        if (currentlySelectedUnit != -1)
        {
            StartCoroutine(LerpCameraTo(mainCamera.transform.position, new Vector3(playerUnits[currentlySelectedUnit].transform.position.x, mainCamera.transform.position.y, playerUnits[currentlySelectedUnit].transform.position.z - 5f), 0.5f, 0.5f));
            yield return new WaitForSecondsRealtime(1f);
        }
        
        DisplayReachableTilesForCurrentUnit();
        CreateRandomTileProps();
    }

    public void ExecuteAction(Tuple<string, int, string> action, UnitType unitType, AbstractUnit unit)
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
                            DisplayReachableTilesForCurrentUnit();

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
                                DisplayReachableTilesForCurrentUnit();
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

            StartCoroutine(RestartLevelDelayed(3));

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

    private void CreateRandomTileProps()
    {
        // remove objects that have changed tile type
        for (int i = grassPropsList.Count - 1; i >= 0; i--)
        {
            if (!((AbstractGameTile)tilemap.GetTile(grassPropsList[i].Item1) is GrassTile))
            {
                Destroy(grassPropsList[i].Item2);
                grassPropsList.RemoveAll(item => item.Item1 == grassPropsList[i].Item1);
            }
        }

        var tileDic = TilemapHelper.GetTileDictionary(tilemap);
        foreach (var tileType in tileDic.Keys)
        {
            var tilesWithType = tileDic[tileType];
            if (tileType == typeof(GrassTile))
            {
                foreach (var tile in tilesWithType)
                {
                    if (grassPropsList.Count == 0 || !grassPropsList.Any(i => i.Item1 == tile))
                    {
                        var randomProp = UnityEngine.Random.Range(1, 4); // random prop 1-4 (4 is excluded)
                        var newProp = Instantiate(Resources.Load("Forest/ForestProps" + randomProp), GrassProps.transform) as GameObject;
                        var cellPos = tilemap.CellToWorld(tile);
                        newProp.transform.position = new Vector3(cellPos.x, newProp.transform.position.y, cellPos.z);
                        newProp.transform.eulerAngles = new Vector3(newProp.transform.rotation.x, UnityEngine.Random.Range(0f, 360f), newProp.transform.rotation.z);

                        grassPropsList.Add(new Tuple<Vector3Int, GameObject>(tile, newProp));
                    }
                }
            }
            else if (tileType == typeof(BurntTile))
            {
                foreach (var tile in tilesWithType)
                {
                    if (burnedPropsList.Count == 0 || !burnedPropsList.Any(i => i.Item1 == tile))
                    {
                        var randomProp = UnityEngine.Random.Range(1, 3); // random prop 1-3 (3 is excluded)
                        var newProp = Instantiate(Resources.Load("Burned/BurnedProps" + randomProp), BurnedProps.transform) as GameObject;
                        var cellPos = tilemap.CellToWorld(tile);
                        newProp.transform.position = new Vector3(cellPos.x, newProp.transform.position.y, cellPos.z);
                        newProp.transform.eulerAngles = new Vector3(newProp.transform.rotation.x, UnityEngine.Random.Range(0f, 360f), newProp.transform.rotation.z);

                        burnedPropsList.Add(new Tuple<Vector3Int, GameObject>(tile, newProp));
                    }
                }
            }
        }
    }

    public void DisplayReachableTilesForCurrentUnit()
    {
        DestroyReachableTilesDisplayHelp();

        if (currentlySelectedUnit != -1)
        {
            var currentUnit = playerUnits[currentlySelectedUnit];
            var reachableTiles = TilemapHelper.FindReachableTiles(currentUnit.TilePosition, currentUnit.ActionPoints, tilemap);
            foreach (var fringe in reachableTiles)
            {
                foreach (var tile in fringe)
                {
                    if (tile.Item2 <= currentUnit.ActionPoints)
                    {
                        var newHelper = Instantiate(Resources.Load("RangeHexagon"), RangeDisplayHelper.transform) as GameObject;
                        var cellPos = tilemap.CellToWorld(tile.Item1);
                        newHelper.transform.position = new Vector3(cellPos.x, newHelper.transform.position.y, cellPos.z);
                    }
                }
            }
        }
    }

    public void DestroyReachableTilesDisplayHelp()
    {
        foreach (Transform child in RangeDisplayHelper.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private Vector3Int GetTileClick()
    {
        var plane = new Plane(Vector3.up, 0f);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distanceToPlane))
        {
            return tilemap.WorldToCell(ray.GetPoint(distanceToPlane));
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private IEnumerator RestartLevelDelayed(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator StartNextLevelDelayed(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        var currentScene = SceneManager.GetActiveScene();
        var levelIndex = levelOrder.IndexOf(currentScene.name) + 1;
        if (levelOrder.Count > levelIndex)
        {
            SceneManager.LoadScene(levelOrder[levelIndex]);
        }
        else
        {
            SceneManager.LoadScene("Startmenu");
        }
    }

    IEnumerator LerpCameraTo(Vector3 pos1, Vector3 pos2, float duration, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            mainCamera.transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        mainCamera.transform.position = pos2;
    }
}
