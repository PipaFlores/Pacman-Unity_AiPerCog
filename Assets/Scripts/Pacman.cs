using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public Movement movement { get; private set; }
    private void Awake(){
        this.movement = GetComponent<Movement>();
    }
    
    private void Update(){

        if (Input.GetKeyDown(KeyCode.UpArrow)){
            this.movement.SetDirection(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            this.movement.SetDirection(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            this.movement.SetDirection(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            this.movement.SetDirection(Vector2.right);
        }
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x); // Gets the angle of dir
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Sets rot to the angle
    }

    public void Start()
    {

    }
    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
    }
    
}
