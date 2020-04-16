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
            UnitActions[0] = new Tuple<string, int>("extinguish_fire", 4);
            UnitType = UnitType.FireTruck;
            ActionPoints = (int)UnitType.FireTruck;
            ReachableFire = 2;
        }
    }
}
