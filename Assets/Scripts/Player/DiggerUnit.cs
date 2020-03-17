using Assets.Scripts.Helpers;
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

        public void Start()
        {
            ActionPoints = 2;

            objectTransform = GetComponent<Transform>();

            if (Tilemap != null)
            {
                TilePosition = Tilemap.WorldToCell(objectTransform.position);
                if (Tilemap.HasTile(TilePosition))
                {
                    var tile = (AbstractGameTile) Tilemap.GetTile(TilePosition);
                    if (tile.TileProperties.IsMovable && !(tile is FireTile))
                    {
                        objectTransform.position = new Vector3(Tilemap.CellToWorld(TilePosition).x, 0, Tilemap.CellToWorld(TilePosition).z);
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
                Debug.LogError("No tilemap defined for Digger unit, destroying it");
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            // Check if the unit was hit by fire
            var fireTiles = TilemapHelper.GetTileCoordinates(Tilemap).Where(t => Tilemap.GetTile(t) is FireTile).ToList();
            if (fireTiles.Contains(TilePosition))
            {
                Destroy(gameObject);
            }
        }
    }
}
