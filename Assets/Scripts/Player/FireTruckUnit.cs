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
            UnitActions[0] = new Tuple<string, int, string, int>("extinguish_fire", 4, "Extinguish all flames surrounding the fire truck in a range of two tiles.", 2);
            UnitType = UnitType.FireTruck;
            ActionPoints = (int)UnitType.FireTruck;
            ReachableFire = 2;
        }
    }
}
