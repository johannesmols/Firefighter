using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DiggerUnit : AbstractUnit
    {
        private Transform objectTransform;
        private Vector3Int tilePosition;

        public void Start()
        {
            ActionPoints = 2;

            objectTransform = GetComponent<Transform>();

            if (Tilemap != null)
            {
                tilePosition = Tilemap.WorldToCell(objectTransform.position);
                if (Tilemap.HasTile(tilePosition))
                {
                    var tile = (AbstractGameTile) Tilemap.GetTile(tilePosition);
                    if (tile.TileProperties.IsMovable && !(tile is FireTile))
                    {
                        objectTransform.position = new Vector3(Tilemap.CellToWorld(tilePosition).x, 0, Tilemap.CellToWorld(tilePosition).z);
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
                Debug.LogError("No tilemap defined for Digger unit");
            }
        }
    }
}
