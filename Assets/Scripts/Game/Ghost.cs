// Purpose: This script is used to control the ghost's movement and behavior.
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Movement movement;
    public GhostHome home {get; private set;}
    public GhostScatter scatter {get; private set;}
    public GhostChase chase {get; private set;}
    public GhostFrightened frightened {get; private set;}
    public GhostBehavior initialBehavior;
    public Transform target; // who are u chasing or running away from

    public GameObject FloatingPoint; // Score prefab
    public int points = 200;

    public float eatenDuration = 8.0f;  // Time to be eaten before respawning
    public bool eaten = false; // Is the ghost eaten

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
        //this.initialBehavior = GetComponent<GhostBehavior>(); // Defined by the editor, otherwise uncomment this and it will get the first behavior script in the prefab
        //Debug.Log("Initial behavior set as:" + this.initialBehavior.name);
        //this.target = GetComponent<Transform>(); //Defined by the editor


    }

    public void Start()
    {

    }
    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
        
        //this.home.CancelInvoke(); // Fixing bug: ghosts leaving home before time on Roundreset
        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Disable();
        
        if (this.home != this.initialBehavior){
            this.home.Disable();
            //Debug.Log(this.gameObject.name  + ": Home disabled");
        }
        if (this.initialBehavior != null){
            this.initialBehavior.Enable();
        }
    }

    /// <summary>
    /// Instantiates the floating score indicator prefab, attached in editor
    /// </summary>
    /// <param name="points"></param>
    public void InstantiateFloatingPoint(int points)
    {
        GameObject floatingPoint = Instantiate(FloatingPoint, transform.position, Quaternion.identity);
        floatingPoint.GetComponentInChildren<TMP_Text>().text = points.ToString();
        Destroy(floatingPoint, 1.5f);
    }

    /// <summary>
    /// Sets the ghost's behavior
    /// </summary>
    /// <param name="homeTimerRatio">Ratio between the level appereance time and the first level departure (Between 0 and 1)</param>
    public void SetGhostBehavior(float homeTimerRatio){
        this.home.duration = this.home.duration * homeTimerRatio;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            if (this.frightened.enabled){
                FindObjectOfType<GameManager>().GhostEaten(this);
            }
            else{
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

}
