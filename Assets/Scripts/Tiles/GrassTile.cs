using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles
{
    public class GrassTile : Tile
    {
        [SerializeField] 
        private Sprite[] sprites;

        public const bool flammable = true;
        public const bool movable = true;

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