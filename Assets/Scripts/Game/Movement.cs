using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    private float speed = 9.47f;
    public float speedMultiplier ;

    public float normalSpeedMultiplier;
    public float frightenedSpeedMultiplier;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;        // we could implement data log through a layer like this
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; } // queue the next direction to avoid realtime collision in mov change
    public Vector3 startingPosition { get; private set; }
    private void Awake(){
        this.rigidbody = GetComponent<Rigidbody2D>(); // Always getting the unity component in the awake method
        this.startingPosition = this.transform.position;
    }

    private void Start(){
        ResetState();
    }

    public void ResetState(){
        this.speedMultiplier = this.normalSpeedMultiplier;
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void Update()  // This process is called each frame, variably across systems
    {
        if (this.nextDirection != Vector2.zero) {
            SetDirection(this.nextDirection);
        }
    }

    private void FixedUpdate(){       // this is useful for physics to control for variable frame rates. This may be important for data log also   
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * this.speed * this.speedMultiplier * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(position + translation);
    }

    public void SetDirection(Vector2 direction, bool forced = false){
        if (forced || !Occupied(direction)){
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
        else{
            this.nextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction){
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer);
        return hit.collider != null;
    }



}
