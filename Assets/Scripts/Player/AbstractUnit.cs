using Assets.Scripts.Helpers;
using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Player
{
    public abstract class AbstractUnit : MonoBehaviour
    {
        public Tilemap Tilemap;
        public UnitType UnitType;

        [HideInInspector]
        public Vector3Int TilePosition;
        [HideInInspector]
        public int ActionPoints;
        [HideInInspector]
        public Transform ObjectTransform;
        
        public void Start()
        {
            // Assigning action points based on unit type
            switch (UnitType)
            {
                case UnitType.Digger:
                    ActionPoints = 2;
                    break;
            }

            ObjectTransform = GetComponent<Transform>();

            if (Tilemap != null)
            {
                TilePosition = Tilemap.WorldToCell(ObjectTransform.position);
                if (Tilemap.HasTile(TilePosition))
                {
                    var tile = (AbstractGameTile)Tilemap.GetTile(TilePosition);
                    if (tile.TileProperties.IsMovable && !(tile is FireTile))
                    {
                        ObjectTransform.position = new Vector3(Tilemap.CellToWorld(TilePosition).x, 0, Tilemap.CellToWorld(TilePosition).z);
                    }
                    else
                    {
                        Debug.LogError("Placed unit on an unmovable tile or fire tile, destroying it");
                        Destroy(gameObject);
                    }

                }
            }
            else
            {
                Debug.LogError("No tilemap defined for unit, destroying it");
                Destroy(gameObject);
            }
        }
    }
}
