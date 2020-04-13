using Assets.Scripts.Helpers;
using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Player
{
    public class FireTruckUnit : AbstractUnit
    {
        public override void Start()
        {
            base.Start();
            UnitActions[0] = new Tuple<string, int>("extinguish_fire", 2);
            UnitType = UnitType.FireTruck;
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
