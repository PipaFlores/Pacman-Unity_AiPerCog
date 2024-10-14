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

        if (Input.GetKeyDown(KeyCode.UpArrow)){
            this.movement.SetDirection(Vector2.up);
            this.inputDirection = "up";
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            this.movement.SetDirection(Vector2.down);
            this.inputDirection = "down";
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            this.movement.SetDirection(Vector2.left);
            this.inputDirection = "left";
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            this.movement.SetDirection(Vector2.right);
            this.inputDirection = "right";
        }
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x); // Gets the angle of dir
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Sets rot to the angle
    }

    // public void DeathSequence()
    // {
    //     enabled = false;
    //     spriteRenderer.enabled = false;
    //     collider.enabled = false;
    //     movement.enabled = false;
    //     deathSequence.enabled = true;
    //     deathSequence.Restart();
    // }
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
