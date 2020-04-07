using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects
{
    public class TileProperties
    {
        public bool IsMovable { get; set; }
        public int MovementCost { get; set; }
        public bool IsFlammable { get; set; }
        public bool IsGoal { get; set; }
        public int RoundsOnFire { get; set; }
    }
}
