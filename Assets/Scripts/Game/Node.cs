using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Vector2> availableDirections { get; private set;}
    public LayerMask obstacleLayer;

    private void Start()
    {
        this.availableDirections = new List<Vector2>();
        CheckAvailbleDirections(Vector2.up);
        CheckAvailbleDirections(Vector2.down);
        CheckAvailbleDirections(Vector2.left);
        CheckAvailbleDirections(Vector2.right);

    }

    private void CheckAvailbleDirections(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.0f, this.obstacleLayer);  

        if (hit.collider == null){
            this.availableDirections.Add(direction);
        } 
    }
}
