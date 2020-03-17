using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Player
{
    public class AbstractUnit : MonoBehaviour
    {
        public Tilemap Tilemap;
        public UnitType UnitType;

        protected Vector3Int TilePosition;
        protected int ActionPoints;
    }
}
