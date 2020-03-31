using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
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
        [HideInInspector]
        public Tuple<string, int>[] UnitActions = new Tuple<string, int>[9];

        public virtual void Start()
        {
            ObjectTransform = GetComponent<Transform>();

            // Assigning action points based on unit type
            ResetActionPoints();

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

        public void ResetActionPoints()
        {
            ActionPoints = (int) UnitType;
        }
    }
}
