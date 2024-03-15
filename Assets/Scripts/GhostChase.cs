using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostChase : GhostBehavior
{
    
    private void OnEnable()
    {
        this.ghost.scatter.Disable();
    }
    
    private void OnDisable() // automatically called by unity when disabled
    {
        this.ghost.scatter.Enable();

    }   
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightened.enabled){      // added the !this.enabled bc ontrigger is always called, 
            
            Vector2 chasingdirection = Vector2.zero;
            float minDistance = float.MaxValue;
            //Debug.Log("Number of Available directions=" + node.availableDirections.Count);
            int i = 0;

            foreach(Vector2 availableDirection in node.availableDirections)
            {
                i++;
                //Debug.Log("For available dir N°" + i);
                Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
                //Debug.Log("Estimated new position=" + "(" + newPosition.x + "," + newPosition.y + ")");
                float distance = (this.ghost.target.position - newPosition).sqrMagnitude; 
                //Debug.Log("Estimated distance to target=" + distance);
                // TOIMPROVE: If calculated distances are equal, then choose at random to avoid glitching
                if (distance < minDistance){
                    chasingdirection = availableDirection;
                    minDistance = distance;
                }
            }
            this.ghost.movement.SetDirection(chasingdirection);
            //Debug.Log("Final index=" + index);

        }
    }
}
