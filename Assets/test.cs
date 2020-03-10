using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class test : MonoBehaviour
{
    public Tilemap tilemap;

    void Start()
    {
        var tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                Debug.Log(tile.GetType().ToString());
            }
        }
    }
}
