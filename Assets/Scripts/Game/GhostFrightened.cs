using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostFrightened : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;
    public float flashduration; // flashduration is the time the ghost will flicker before it stops
    public int flickerCount = 3; // flickerCount is the number of times the ghost will flicker before it stops
    public bool eaten { get; private set; }

// this is done instead of Onenable() in case
// it doesn't get called when you eat a second pellet within the time frame (its already enabled)
    public override void Enable(float duration) 
    {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;
        this.flashduration = duration/2.0f;
        Invoke(nameof(Flash), duration/2.0f);
    }

    private void OnEnable()
    {
        this.ghost.movement.speedMultiplier = this.ghost.movement.frightenedSpeedMultiplier;
        this.eaten = false;
    }

    private void OnDisable()
    {
        this.ghost.movement.speedMultiplier = this.ghost.movement.normalSpeedMultiplier;
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

    public void Flash()
    {
        if (!this.eaten)
        {
            StartCoroutine(Flicker(flashduration));
        }
    }

    private IEnumerator Flicker(float duration)
    {

        float flickerDuration = duration / (flickerCount * 2);
        for (int i = 0; i < flickerCount; i++)
        {
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Restart();
            yield return new WaitForSeconds(flickerDuration);

            this.blue.enabled = true;
            this.white.enabled = false;
            yield return new WaitForSeconds(flickerDuration);
        }
    }

    private void Eaten()  // TODO: Make it to fly to the home, instead of only teleporting.
    {
        this.eaten = true; // this is to prevent the flash from happening
        this.ghost.eaten = true; // this is the boolean state to be collected by the datacollector
        Vector3 position = this.ghost.home.inside.position;
        position.z = this.ghost.transform.position.z;
        this.ghost.transform.position = position;

        this.ghost.home.Enable(this.ghost.eatenDuration);  // sets the time the ghost is in home
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
