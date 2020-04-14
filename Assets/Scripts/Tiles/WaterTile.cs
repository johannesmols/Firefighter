using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Tiles
{
    class WaterTile : AbstractGameTile
    {
        public WaterTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = false,
                MovementCost = int.MaxValue,
                IsFlammable = false,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Water Tiles")]
        public static void CreateWaterTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Water Tile", "New Water Tile", "asset",
                "Save water tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<WaterTile>(), path);
            }
        }
#endif
    }
}
