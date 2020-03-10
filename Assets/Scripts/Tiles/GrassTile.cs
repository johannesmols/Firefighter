using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles
{
    public class GrassTile : Tile
    {
        [SerializeField] private Sprite[] sprites;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tiles/GrassTiles")]
        public static void CreateGrassTile()
        {
            var path = EditorUtility.SaveFilePanelInProject("Save Grass Tile", "New Water Tile", "asset",
                "Save grass tile", "Assets");

            if (!string.IsNullOrWhiteSpace(path))
            {
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrassTile>(), path);
            }
        }
#endif
    }
}