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
        public FireTruckUnit() {
            UnitProperties = new UnitProperties
            {
                reachableFire = 2
            };
        }
        public override void Start()
        {
            base.Start();
            UnitActions[0] = new Tuple<string, int>("extinguish_fire", 4);
            UnitProperties.reachableFire = 2;
            UnitType = UnitType.FireTruck;
        }
    }
}
