using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using Assets.Scripts.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class FirefighterUnit : AbstractUnit
    {
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            UnitActions[0] = new Tuple<string, int, string>("extinguish_fire", 2, "Extinguish all flames directly surrounding the Firefighter.");
            UnitType = UnitType.Firefighter;
            ActionPoints = (int)UnitType.Firefighter;
            ReachableFire = 1;
        }
    }
}
