using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GhostHome : GhostBehavior
{
    public Transform inside;
    public Transform outside;


    void OnEnable(){
        StopAllCoroutines();
    }
    void OnDisable(){
        if (this.gameObject.activeSelf){
            StartCoroutine(ExitTransition());
            if (this.ghost.eaten){
                this.ghost.eaten = false;
            }
            this.ghost.scatter.Enable();
        }
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")){
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }


    private IEnumerator ExitTransition()
    {
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;

        Vector3 position = this.transform.position;

        float duration = 1.0f;
        float elapsed = 0.0f;
        
        // Animate to the starting point
        while (elapsed < duration){
            Vector3 newPosition = Vector3.Lerp(position, this.inside.position, elapsed/(duration/2.0f));
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0.0f;
        // Animate exiting the ghost home
        while (elapsed < duration){
            Vector3 newPosition = Vector3.Lerp(this.inside.position, this.outside.position, elapsed/duration);
            newPosition.z = position.z;
            this.ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pick a random direction left or right and re-enable movement
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5 ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
        
    }
}
