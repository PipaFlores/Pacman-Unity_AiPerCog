using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8.0f;

    // Function to get powerpellet index according to their location on the grid
    public int GetPowerPelletIndex()
    {
        if (transform.position.x < 0 && transform.position.y > 0)
        {
            return 0;
        }
        else if (transform.position.x > 0 && transform.position.y > 0)
        {
            return 1;
        }
        else if (transform.position.x > 0 && transform.position.y < 0)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
  
        

    protected override void Eat() //
        {
            FindObjectOfType<GameManager>().PowerPelletEaten(this);
        }
}
