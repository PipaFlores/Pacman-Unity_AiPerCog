using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using UnityEngine;

// Scatter behavior is simplified. In the original game, each ghost scatteres to a different quadrant and then
// patrols a pre-defined path, until chasing.
public class GhostScatter : GhostBehavior
{
     public PatrolPath patrolPath;
     private int currentPatrolIndex = 0;
     private Vector2 currentTarget;

    private void OnEnable()
    {
        this.ghost.chase.Disable();
        currentPatrolIndex = 0;
        currentTarget = patrolPath.waypoints[currentPatrolIndex];
        // Debug.Log("waypoints=" + patrolPath.waypoints.Length);
        // Debug.Log("waypoint vectors are: ");
        // foreach (Vector2 waypoint in patrolPath.waypoints){
            // Debug.Log(waypoint);
        // }
        
    }

    private void OnDisable()
    {
        this.ghost.chase.Enable();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        // If patrol is empty, then ghost will move randomly
        if (currentPatrolIndex >= patrolPath.waypoints.Length ){
            this.randomPath(node);
            // Debug.Log(this.ghost.name + "Random path");
        }
        else{
            currentTarget = new Vector3(patrolPath.waypoints[currentPatrolIndex].x, patrolPath.waypoints[currentPatrolIndex].y, 0.0f);

            if (Vector2.Distance(transform.position, currentTarget) < 1.5f)
            {
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPath.waypoints.Length){
                    this.randomPath(node);
                    // Debug.Log(this.ghost.name + "Random path");
                }
                else{
                    currentTarget = patrolPath.waypoints[currentPatrolIndex];
                    Patrol(node, currentTarget);
                    // Debug.Log(this.ghost.name + "Patrol path");
                }
            }
            else{
                Patrol(node, currentTarget);
                // Debug.Log(this.ghost.name + "Patrol path");
            }
            
        }
        
        
    }

    private void randomPath(Node node){
        if (node != null && this.enabled && !this.ghost.frightened.enabled)
        {
            int index = Random.Range(0, node.availableDirections.Count);

            if (node.availableDirections[index] == -this.ghost.movement.direction && node.availableDirections.Count > 1)
            {
                index++;
                if (index >= node.availableDirections.Count)
                {
                    index = 0;
                }
            }

            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }

    private void Patrol(Node node, Vector3 target){
        if (node != null && this.enabled && !this.ghost.frightened.enabled){      // added the !this.enabled bc ontrigger is always called, 
            
            Vector2 movedirection = Vector2.zero;
            float minDistance = float.MaxValue;
            // Debug.Log("Number of Available directions=" + node.availableDirections.Count);
            int i = 0;
            int finalindex = 0;

            foreach(Vector2 availableDirection in node.availableDirections)
            {
                i++;
                // Debug.Log("For available dir N°" + i);
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                // RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, availableDirection, 12.0f, );
                // Vector3 newPosition = hit.point;
                // Debug.Log("Estimated new position=" + "(" + newPosition.x + "," + newPosition.y + ")");
                float distance = (target - newPosition).sqrMagnitude; 
                // Debug.Log("Estimated distance to target=" + distance);
                // TOIMPROVE: If calculated distances are equal, then choose at random to avoid glitching
                if (distance < minDistance){
                    movedirection = availableDirection;
                    minDistance = distance;
                    finalindex = i;
                }
            }
            this.ghost.movement.SetDirection(movedirection);
            // Debug.Log("Final index=" + finalindex);

        }
    }
    
}
