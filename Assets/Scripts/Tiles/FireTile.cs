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
    public class FireTile : AbstractGameTile
    {
        public FireTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = false,
                IsFlammable = false,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Fire Tiles")]
        public static void CreateFireTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Fire Tile", "New Fire Tile", "asset",
                "Save grass tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<FireTile>(), path);
            }
        }
#endif
    }
}
