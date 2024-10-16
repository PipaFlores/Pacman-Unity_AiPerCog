using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public Movement movement { get; private set; }
    // public AnimatedSprite deathSequence;
    public SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    public bool pacmanAttack = false;

    public string inputDirection; // This is used to store the input direction of the player
    
    
    private void Awake(){
        this.movement = GetComponent<Movement>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
    
    private void Update(){

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
            this.movement.SetDirection(Vector2.up);
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                this.inputDirection = "up";
            } 
            else{
                this.inputDirection = "w";
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
            this.movement.SetDirection(Vector2.down);
            if (Input.GetKeyDown(KeyCode.DownArrow)){
                this.inputDirection = "down";
            } 
            else{
                this.inputDirection = "s";
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
            this.movement.SetDirection(Vector2.left);
            if (Input.GetKeyDown(KeyCode.LeftArrow)){
                this.inputDirection = "left";
            } 
            else{
                this.inputDirection = "a";
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
            this.movement.SetDirection(Vector2.right);
            if (Input.GetKeyDown(KeyCode.RightArrow)){
                this.inputDirection = "right";
            } 
            else{
                this.inputDirection = "d";
            }
        }
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x); // Gets the angle of dir
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Sets rot to the angle
    }

    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        // deathSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }
    
}
