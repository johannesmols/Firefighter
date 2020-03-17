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
    class ObstacleTile : AbstractGameTile
    {
        public ObstacleTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = false,
                IsFlammable = false,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Obstacle Tiles")]
        public static void CreateObstacleTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Obstacle Tile", "New Obstacle Tile", "asset",
                "Save grass tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<ObstacleTile>(), path);
            }
        }
#endif
    }
}
