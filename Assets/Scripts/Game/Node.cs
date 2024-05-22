using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Vector2> availableDirections { get; private set;}
    
    public LayerMask obstacleLayer;

    // public LayerMask nodeLayer;
    // public List<Vector3> availableneighbors { get; private set; }

    private void Start()
    {
        this.availableDirections = new List<Vector2>();
        // this.availableneighbors = new List<Vector3>();
        CheckAvailbleDirections(Vector2.up);
        CheckAvailbleDirections(Vector2.down);
        CheckAvailbleDirections(Vector2.left);
        CheckAvailbleDirections(Vector2.right);
        // CheckAvailbleNeighbors();

    }

    private void CheckAvailbleDirections(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.0f, this.obstacleLayer);  

        if (hit.collider == null){
            this.availableDirections.Add(direction);
        } 
    }

    // private void CheckAvailbleNeighbors()
    // {
    //     foreach (Vector2 dir in this.availableDirections)
    //     {
    //         RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, dir, 5.0f, this.nodeLayer);
    //         Debug.Log("Checking neighbors"); // Debugging
    //         Debug.Log("Hit collider: " + hit.collider.name);
    //         if (hit.collider != null){
                
    //             this.availableneighbors.Add(hit.collider.GetComponent<Transform>().position);
    //         }
    //     }
    // }
}
