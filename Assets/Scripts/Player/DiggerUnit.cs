using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
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
        public override void Start()
        {
            base.Start();
            UnitActions[0] = new Tuple<string, int>("dig_trench", 2);
            UnitType = UnitType.Digger;
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
