using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles 
{
    public class BurntTile : AbstractGameTile
    {
            public BurntTile()
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
        [MenuItem("Assets/Create/Tiles/Burnt Tiles")]
        public static void CreateBurntTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Burnt Tile", "New Burnt Tile", "asset",
                "Save Burnt tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<BurntTile>(), path);
            }
        }
#endif
    }
}
