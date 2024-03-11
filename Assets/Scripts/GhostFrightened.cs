using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten { get; private set; }

// this is done instead of Onenable() in case
// it doesn't get called when you eat a second pellet within the time frame (its already enabled)
    public override void Enable(float duration) 
    {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = true;

        Invoke(nameof(Flash), duration /2.0f);
    }

    private void OnEnable()
    {
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable()
    {
        this.ghost.movement.speedMultiplier = 1.0f;
        this.eaten = false;
        //this.ghost.chase.Enable();
    }

    public override void Disable()
    {
        base.Disable();
        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;

    }

    private void Flash()
    {
        if (!this.eaten){
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Restart();
        }
        
    }

    private void Eaten()  // TODO: Make it to fly to the home, instead of only teleporting.
    {
        this.eaten = true;
        Vector3 position = this.ghost.home.inside.position;
        position.z = this.ghost.transform.position.z;
        this.ghost.transform.position = position;

        this.ghost.home.Enable(this.duration);
        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;

    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            if (this.enabled){
                Eaten();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled){      // added the !this.enabled bc ontrigger is always called, 
            
            Vector2 chasingdirection = Vector2.zero;
            float maxDistance = float.MinValue;
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
                if (distance > maxDistance){
                    chasingdirection = availableDirection;
                    maxDistance = distance;
                }
            }
            this.ghost.movement.SetDirection(chasingdirection);
            //Debug.Log("Final index=" + index);

        }
    }

}
