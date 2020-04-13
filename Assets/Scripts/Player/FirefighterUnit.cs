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
            UnitActions[0] = new Tuple<string, int>("putout_fire", 2);
            UnitType = UnitType.Firefighter;
        }

        // Update is called once per frame
        public void Update()
        {
            var fireTiles = TilemapHelper.GetTileCoordinates(Tilemap).Where(t => Tilemap.GetTile(t) is FireTile).ToList();
            if(fireTiles.Contains(TilePosition))
            {
                Destroy(gameObject);
            }
        }
    }
}
