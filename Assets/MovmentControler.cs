using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovmentControler : MonoBehaviour
{
    private Vector2 movementInput;

    private Vector3 direction;

    //public Tilemap fogOfWar;

    bool hasMoved;

    void Update()
    {
        if (movementInput.x == 0)
        {
            hasMoved = false;
        }
        else if (movementInput.x != 0 && !hasMoved)
        {
            hasMoved = true;
            GetMovementDirection();
        }
    }

    public void GetMovementDirection()
    {
        if (movementInput.x < 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(-0.5f, 0.5f);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(-0.5f, -0.5f);
            }
            else
            {
                direction = new Vector3(-1, 0, 0);
            }

            transform.position += direction;
            //UpdateFogOfWar();
        }

        else if (movementInput.x > 0)
        {
            if (movementInput.y > 0)
            {
                direction = new Vector3(0.5f, 0.5f);
            }
            else if (movementInput.y < 0)
            {
                direction = new Vector3(-0.5f, 0.5f);
            }
            else
            {
                direction = new Vector3(1, 0, 0);
            }

            transform.position += direction;
            //UpdateFogOfWar();
        }
    }
}
