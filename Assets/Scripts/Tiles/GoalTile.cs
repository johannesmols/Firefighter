using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles 
{
    public class GoalTile : AbstractGameTile
    {
            public GoalTile()
            {
                TileProperties = new TileProperties
                {
                    IsMovable = true,
                    MovementCost = 2,
                    IsFlammable = true,
                    IsGoal = true
                };
            }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/Goal Tiles")]
        public static void CreateGoalTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Goal Tile", "New Goal Tile", "asset",
                "Save Goal tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(CreateInstance<GoalTile>(), path);
            }
        }
#endif
    }
}
