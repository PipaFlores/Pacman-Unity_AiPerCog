using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scatter behavior is simplified. In the original game, each ghost scatteres to a different quadrant and then
// patrols a pre-defined path, until chasing.
public class GhostScatter : GhostBehavior
{

    private void OnEnable()
    {
        this.ghost.chase.Disable();
    }
    private void OnDisable() // automatically called by unity when disabled
    {
        this.ghost.chase.Enable();

    }   
    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightened.enabled){      // added the !this.enabled bc ontrigger is always called, 
            
            int index = Random.Range(0, node.availableDirections.Count);
            //Debug.Log("Av. directions=" + node.availableDirections.Count);
            //Debug.Log("Random index=" + index);

            if (node.availableDirections[index] == -this.ghost.movement.direction && node.availableDirections.Count > 1){
                index++;
                if (index >= node.availableDirections.Count){
                    index = 0;
                }
            }
            this.ghost.movement.SetDirection(node.availableDirections[index]);
            //Debug.Log("Final index=" + index);

        }
    }
 
}
