using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Pacman : Agent
{
    public Movement movement { get; private set; }
    public SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    
    public bool pacmanAttack = false;
    
    private Vector2 startPosition;

    public string inputDirection; // This is used to store the input direction of the player

    private int lastAction = 3; // Default = Right (classic Pacman start)

    public DataCollector gameDatacollector;

    // private void 
    public override void Initialize(){
        this.movement = GetComponent<Movement>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        startPosition = transform.position;
        // currentDirection;

    }

    public override void OnEpisodeBegin()
    {
        // Reset position, movement and state at the start of each episode
        
        transform.position = startPosition;
        ResetState();
        spriteRenderer.enabled = true;
        collider.enabled = true;
        gameObject.SetActive(true);
        lastAction = 3; // Reset to "Right"
        movement.SetDirection(Vector2.right);
    }

    // public void ResetState()
    // {
    //     enabled = true;
    //     spriteRenderer.enabled = true;
    //     collider.enabled = true;
    //     // deathSequence.enabled = false;
    //     ResetState();
    //     gameObject.SetActive(true);
    // }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add Pacman’s position
        sensor.AddObservation(transform.localPosition);

        // Add Pacman’s current movement direction
        sensor.AddObservation(movement.direction);

        // Add Pacman’s velocity
        sensor.AddObservation(movement.rigidbody.velocity);

        // TODO: Add observations for environment
        // e.g., distance to ghosts, distance to pellets, etc.
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int move = actions.DiscreteActions[0]; // Single discrete action space: 0=NoMove, 1=Up, 2=Down, 3=Left, 4=Right

        // If action is invalid (-1) or nothing new, reuse last action
        if (move < 0 || move > 3)
            move = lastAction;

        switch (move)
        {
            case 0:
                movement.SetDirection(Vector2.up);
                break;
            case 1:
                movement.SetDirection(Vector2.down);
                break;
            case 2:
                movement.SetDirection(Vector2.left);
                break;
            case 3:
                movement.SetDirection(Vector2.right);
                break;
            default:
                movement.SetDirection(movement.direction); // No movement
                break;
        }

        lastAction = move;

        if (movement.direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
        // Rewards (example, adapt to your game logic)
        AddReward(-0.001f); // small negative reward per step (encourages faster play)
        // if (this.lives== 0)
        // {
        //     SetReward(-1.0f);
        //     EndEpisode();
        // }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow manual control for testing
        var discreteActionsOut = actionsOut.DiscreteActions;
        // discreteActionsOut[0] = movement.direction;sds

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 0;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            discreteActionsOut[0] = 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            discreteActionsOut[0] = 2;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            discreteActionsOut[0] = 3;
        else
            discreteActionsOut[0] = lastAction; // Default = keep last move
        
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
// Original Pacman
// public class Pacman : MonoBehaviour
// {
//     public Movement movement { get; private set; }
//     // public AnimatedSprite deathSequence;
//     public SpriteRenderer spriteRenderer;
//     private new Collider2D collider;
//     public bool pacmanAttack = false;

//     public string inputDirection; // This is used to store the input direction of the player
    
    
//     private void Awake(){
//         this.movement = GetComponent<Movement>();
//         this.spriteRenderer = GetComponent<SpriteRenderer>();
//         collider = GetComponent<Collider2D>();
//     }
    
//     private void Update(){

//         if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
//             this.movement.SetDirection(Vector2.up);
//             if (Input.GetKeyDown(KeyCode.UpArrow)){
//                 this.inputDirection = "up";
//             } 
//             else{
//                 this.inputDirection = "w";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
//             this.movement.SetDirection(Vector2.down);
//             if (Input.GetKeyDown(KeyCode.DownArrow)){
//                 this.inputDirection = "down";
//             } 
//             else{
//                 this.inputDirection = "s";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
//             this.movement.SetDirection(Vector2.left);
//             if (Input.GetKeyDown(KeyCode.LeftArrow)){
//                 this.inputDirection = "left";
//             } 
//             else{
//                 this.inputDirection = "a";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
//             this.movement.SetDirection(Vector2.right);
//             if (Input.GetKeyDown(KeyCode.RightArrow)){
//                 this.inputDirection = "right";
//             } 
//             else{
//                 this.inputDirection = "d";
//             }
//         }
//         float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x); // Gets the angle of dir
//         this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Sets rot to the angle
//     }

//     public void ResetState()
//     {
//         enabled = true;
//         spriteRenderer.enabled = true;
//         collider.enabled = true;
//         // deathSequence.enabled = false;
//         movement.ResetState();
//         gameObject.SetActive(true);
//     }
    
// }
