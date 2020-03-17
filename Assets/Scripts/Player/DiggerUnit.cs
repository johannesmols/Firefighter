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
