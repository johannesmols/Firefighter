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
            UnitActions[0] = new Tuple<string, int, string, int>("dig_trench", 2, "Turn the tile that the digger is standing on into a trench. Fire cannot spread through it.", 0);
            UnitType = UnitType.Digger;
            ActionPoints = (int) UnitType.Digger;
        }
    }
}
