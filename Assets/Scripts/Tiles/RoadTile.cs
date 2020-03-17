using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles
{
    class RoadTile : AbstractGameTile
    {
        public RoadTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = true,
                IsFlammable = true,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Road Tile")]
        public static void CreateRoadTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "asset",
                "Save road tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<RoadTile>(), path);
            }
        }
#endif
    }
}
