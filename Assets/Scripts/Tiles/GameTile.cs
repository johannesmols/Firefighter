using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Tiles
{
    public abstract class GameTile : Tile
    {
        public Sprite[] Sprites { get; }

        public TileProperties TileProperties { get; set; }
    }
}
