using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Tiles
{
    public class TrenchTile : AbstractGameTile
    {
        public TrenchTile()
        {
            TileProperties = new TileProperties
            {
                IsMovable = true,
                MovementCost = 2,
                IsFlammable = false,
                IsGoal = false
            };
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Trench Tiles")]
        public static void CreateTrenchTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Trench Tile", "New Trench Tile", "asset",
                "Save grass tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<TrenchTile>(), path);
            }
        }
#endif
    }
}
