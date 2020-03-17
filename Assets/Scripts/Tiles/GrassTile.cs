using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles
{
    public class GrassTile : GameTile
    {
        public GrassTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = true,
                IsFlammable = true,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Grass Tiles")]
        public static void CreateGrassTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Grass Tile", "New Grass Tile", "asset",
                "Save grass tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<GrassTile>(), path);
            }
        }
#endif
    }
}