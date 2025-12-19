using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Specialized;

public class Pacman : Agent
{
    public GameObject player;  // Reference to the player (Pacman)
    public GameObject[] ghosts;
    private List<GameDataPoint> dataPointsList = new List<GameDataPoint>(); // List to store collected game data points
    
    public Movement movement { get; private set; }
    public SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    
    public bool pacmanAttack = false;
    
    private Vector2 startPosition;

    public string inputDirection; // This is used to store the input direction of the player

    private int lastAction = 3; // Default = Right (classic Pacman start)

    public DataCollector gameDatacollector;
    public Transform pellets;

    // private void 
    public override void Initialize(){
        this.movement = GetComponent<Movement>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        startPosition = transform.position;
        // currentDirection;

    }

    public override void OnEpisodeBegin()
    {
        // Reset position, movement and state at the start of each episode
        
        transform.position = startPosition;
        ResetState();
        spriteRenderer.enabled = true;
        collider.enabled = true;
        gameObject.SetActive(true);
        lastAction = 3; // Reset to "Right"
        movement.SetDirection(Vector2.right);
    }

    // public void ResetState()
    // {
    //     enabled = true;
    //     spriteRenderer.enabled = true;
    //     collider.enabled = true;
    //     // deathSequence.enabled = false;
    //     ResetState();
    //     gameObject.SetActive(true);
    // }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add Pacman data
        sensor.AddObservation(GameManager.Instance.pacman.transform.position); //Might need to normaize
        sensor.AddObservation(this.pacmanAttack); // Player's attack state
        sensor.AddObservation(lastAction);//Input Direction
        sensor.AddObservation(movement.direction);// this is still buggy i think
        
        
        // Collect ghosts' data
        int ghost_length = GameManager.Instance.ghosts.Length;
        Vector2[] ghostsPos = new Vector2[ghost_length]; // Array to store positions of all ghosts
        int[] ghostsState = new int[ghost_length]; // Array to store states of all ghosts
		float[] ghostDistances = new float[ghost_length];
        for (int i = 0; i < ghost_length; i++)
        {
            ghostsPos[i] = GameManager.Instance.ghosts[i].transform.position; // Get each ghost's position
            sensor.AddObservation(ghostsPos[i]);
            ghostsState[i] = GetGhostState(GameManager.Instance.ghosts[i]); // Get each ghost's state
            sensor.AddObservation((float)ghostsState[i]);
			ghostDistances[i] = Vector2and3ManhattanDistance(GameManager.Instance.pacman.transform.position, ghostsPos[i]);
        }
        int[] PowerPelletStates = new int[GameManager.Instance.PowerPelletStates.Length];
        for (int i = 0; i < GameManager.Instance.PowerPelletStates.Length; i++)
        {
            PowerPelletStates[i] = GameManager.Instance.PowerPelletStates[i]; // Get power pellet states
            sensor.AddObservation(PowerPelletStates[i]);
        }
        sensor.AddObservation((float)GameManager.Instance.score / 3200f); // Current score pellet max 2400, 200 per ghost
        sensor.AddObservation((float)GameManager.Instance.lives / 3f); // Normalised lives
        sensor.AddObservation((float)GameManager.Instance.remainingPellets/244f); // assuming max 244 pellets
        sensor.AddObservation((float)GameManager.Instance.remainingPills / 4f); // assuming max 4 power pellets
        sensor.AddObservation((float)GameManager.Instance.fruitState_1); // State of the first fruit
        sensor.AddObservation((float)GameManager.Instance.fruitState_2); // State of the Second fruit
        
        //get distances to all active amd inactive pellets active pellets
        List<float> pellet_distances = new List<float>();
        foreach (Transform pellet in GameManager.Instance.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<Pellet>() != null)//if pellet exists
            {
                float dist = Vector3ManhattanDistance(pellet.position, GameManager.Instance.pacman.transform.position);
                pellet_distances.Add((float)dist);
                // sensor.AddObservation((float)ist);
            }
            else
            {
                pellet_distances.Add(0.0f);
                // sensor.AddObservation(0.0f);
            }
        }
        // get the distance to the closest pellet
        float[] pellet_distances_array = pellet_distances.ToArray();
        float closest_pellet = 100.0f; //larg starting point to ensure all pellets are being compared
        foreach (float pellet in pellet_distances_array)
        {
            if (pellet != 0.0f && pellet < closest_pellet)// if pellet is Active and dist is less then current min
            {
                closest_pellet = pellet;
            }
        }
        sensor.AddObservation(closest_pellet);
        
        // converting the pellet coorindates to boolean grid
        List<Vector3> active = new List<Vector3>();
        List<Vector3> inactive = new List<Vector3>();
		List<Vector3> powerPellets = new List<Vector3>();
        foreach (Transform pellet in GameManager.Instance.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<Pellet>() != null)//if pellet exists
            {
                active.Add(pellet.position);
            }
            else if (!(pellet.gameObject.activeSelf) && pellet.GetComponent<Pellet>() != null )
            {
                inactive.Add(pellet.position);
            }
			if (pellet.gameObject.activeSelf && pellet.GetComponent<PowerPellet>() != null)
			{
				powerPellets.Add(pellet.position);
			}
        }
		// calculate ghost distances
		sensor.AddObservation(ghostDistances);
        //bool[] flatGrid = CreatePelletGrid(active, inactive); 
        // sensor.AddObservation((float)flatGrid);
        //foreach (bool pellet_loc in flatGrid)
        //{
            //sensor.AddObservation(pellet_loc);
        //}
		//int [] gameGrid = CreateGameGrid(active, inactive, GameManager.Instance.pacman.transform.position, ghostsPos, out _, out _);
		int [] gameGrid = CreateDetailedGameGrid(active, inactive, powerPellets, GameManager.Instance.pacman.transform.position, this.pacmanAttack, ghostsPos, ghostsState, out _, out _);
		foreach (int obj_loc in gameGrid)
        {
            sensor.AddObservation(obj_loc);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int move = actions.DiscreteActions[0]; // Single discrete action space: 0=NoMove, 1=Up, 2=Down, 3=Left, 4=Right

        // If action is invalid (-1) or nothing new, reuse last action
        if (move < 0 || move > 3)
            move = lastAction;

        switch (move)
        {
            case 0:
                movement.SetDirection(Vector2.up);
                break;
            case 1:
                movement.SetDirection(Vector2.down);
                break;
            case 2:
                movement.SetDirection(Vector2.left);
                break;
            case 3:
                movement.SetDirection(Vector2.right);
                break;
            default:
                movement.SetDirection(movement.direction); // No movement
                break;
        }

        lastAction = move;

        if (movement.direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
        // Rewards (example, adapt to your game logic)
        AddReward(-0.25f); // small negative reward per step (encourages faster play)
        // AddReward(-0.05f); // small negative reward per step (encourages faster play)
        // if (this.lives== 0)
        // {
        //     SetReward(-1.0f);
        //     EndEpisode();
        // }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow manual control for testing
        var discreteActionsOut = actionsOut.DiscreteActions;
        // discreteActionsOut[0] = movement.direction;sds

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 0;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            discreteActionsOut[0] = 1;
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            discreteActionsOut[0] = 2;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            discreteActionsOut[0] = 3;
        else
            discreteActionsOut[0] = lastAction; // Default = keep last move
        
    }
    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        // deathSequence.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }
    private int GetGhostState(Ghost ghost)
    {
        // Determine the state of a ghost
        if (ghost.GetComponent<GhostHome>().enabled)
        {
            if (ghost.GetComponent<Ghost>().eaten)
            {
                return 4; // Ghost is eaten
            }
            else return 0; // Ghost is at home (not eaten)
        }
        else if (ghost.GetComponent<GhostFrightened>().enabled)
        {
            return 3; // Ghost is frightened
        }
        else if (ghost.GetComponent<GhostChase>().enabled)
        {
            return 2; // Ghost is chasing
        }
        else if (ghost.GetComponent<GhostScatter>().enabled)
        {
            return 1; // Ghost is scattering
        }
        else
        {
            return -666; // Error state
        }
    }
    
    public bool[] CreatePelletGrid(List<Vector3> activePellets, List<Vector3> inactivePellets)
    {
        // Combine all pellets to find grid bounds
        List<Vector3> allPellets = new List<Vector3>();
        allPellets.AddRange(activePellets);
        allPellets.AddRange(inactivePellets);
    
        if (allPellets.Count == 0)
            return new bool[0];
    
        // Find min/max coordinates (using x and z for horizontal plane)
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
    
        foreach (Vector3 pellet in allPellets)
        {
            minX = Mathf.Min(minX, pellet.x);
            maxX = Mathf.Max(maxX, pellet.x);
            minY = Mathf.Min(minY, pellet.y);
            maxY = Mathf.Max(maxY, pellet.y);
        }
    
        // Calculate grid dimensions
        int width = Mathf.RoundToInt(maxX - minX) + 1;
        int height = Mathf.RoundToInt(maxY - minY) + 1;
    
        // Initialize flattened array with false (0)
        bool[] grid = new bool[width * height];
    
        // Mark active pellets as true (1)
        foreach (Vector3 pellet in activePellets)
        {
            int xIndex = Mathf.RoundToInt(pellet.x - minX);
            int yIndex = Mathf.RoundToInt(pellet.y - minY);
        
            // Convert 2D coordinates to 1D index (row-major order)
            int index = yIndex * width + xIndex;
            grid[index] = true;
        }
    
        return grid;
    }

	public int[] CreateGameGrid(List<Vector3> activePellets, List<Vector3> inactivePellets, 
                            Vector3 pacmanPosition, Vector2[] ghostPositions, 
                            out int width, out int height)
	{
    	// Combine all positions to find grid bounds
    	List<Vector3> allPositions = new List<Vector3>();
    	allPositions.AddRange(activePellets);
    	allPositions.AddRange(inactivePellets);
    	allPositions.Add(pacmanPosition);
	
		foreach (Vector2 ghost in ghostPositions)
    	{
        	allPositions.Add(new Vector3(ghost.x, ghost.y, 0));
    	}
    
    	if (allPositions.Count == 0)
    	{
        	width = 0;
        	height = 0;
        	return new int[0];
    	}
    
    	// Find min/max coordinates (using x and y)
    	float minX = float.MaxValue;
    	float maxX = float.MinValue;
    	float minY = float.MaxValue;
    	float maxY = float.MinValue;
    
    	foreach (Vector3 pos in allPositions)
    	{
        	minX = Mathf.Min(minX, pos.x);
        	maxX = Mathf.Max(maxX, pos.x);
        	minY = Mathf.Min(minY, pos.y);
        	maxY = Mathf.Max(maxY, pos.y);
    	}
    
    	// Calculate grid dimensions
    	width = Mathf.RoundToInt(maxX - minX) + 1;
    	height = Mathf.RoundToInt(maxY - minY) + 1;
    
    	// Initialize flattened array with 0 (empty space)
    	int[] grid = new int[width * height];
    
    	// Mark active pellets as 1
    	foreach (Vector3 pellet in activePellets)
    	{
        	int xIndex = Mathf.RoundToInt(pellet.x - minX);
        	int yIndex = Mathf.RoundToInt(pellet.y - minY);
        	int index = yIndex * width + xIndex;
        	grid[index] = 1;
    	}
    
    	// Inactive pellets remain 0 (already initialized)
    
    	// Mark Pacman as 2
    	int pacmanX = Mathf.RoundToInt(pacmanPosition.x - minX);
    	int pacmanY = Mathf.RoundToInt(pacmanPosition.y - minY);
    	int pacmanIndex = pacmanY * width + pacmanX;
    	grid[pacmanIndex] = 2;
    
    	// Mark ghosts as 3
    	foreach (Vector3 ghost in ghostPositions)
    	{
        	int ghostX = Mathf.RoundToInt(ghost.x - minX);
        	int ghostY = Mathf.RoundToInt(ghost.y - minY);
        	int ghostIndex = ghostY * width + ghostX;
        	grid[ghostIndex] = 3;
    	}
    
    	return grid;
	}

	public int[] CreateDetailedGameGrid(List<Vector3> activePellets, List<Vector3> inactivePellets, List<Vector3> PowerPellets,
                            Vector3 pacmanPosition, bool pacmanAttack, Vector2[] ghostPositions, int[] ghostStates, 
                            out int width, out int height)
	{
    	// Combine all positions to find grid bounds
    	List<Vector3> allPositions = new List<Vector3>();
    	allPositions.AddRange(activePellets);
    	allPositions.AddRange(inactivePellets);
    	allPositions.Add(pacmanPosition);
	
		foreach (Vector2 ghost in ghostPositions)
    	{
        	allPositions.Add(new Vector3(ghost.x, ghost.y, 0));
    	}
    
    	if (allPositions.Count == 0)
    	{
        	width = 0;
        	height = 0;
        	return new int[0];
    	}
    
    	// Find min/max coordinates (using x and y)
    	float minX = float.MaxValue;
    	float maxX = float.MinValue;
    	float minY = float.MaxValue;
    	float maxY = float.MinValue;
    
    	foreach (Vector3 pos in allPositions)
    	{
        	minX = Mathf.Min(minX, pos.x);
        	maxX = Mathf.Max(maxX, pos.x);
        	minY = Mathf.Min(minY, pos.y);
        	maxY = Mathf.Max(maxY, pos.y);
    	}
    
    	// Calculate grid dimensions
    	width = Mathf.RoundToInt(maxX - minX) + 1;
    	height = Mathf.RoundToInt(maxY - minY) + 1;
    
    	// Initialize flattened array with 0 (empty space)
    	int[] grid = new int[width * height];
    
    	// Mark active pellets as 1
    	foreach (Vector3 pellet in activePellets)
    	{
        	int xIndex = Mathf.RoundToInt(pellet.x - minX);
        	int yIndex = Mathf.RoundToInt(pellet.y - minY);
        	int index = yIndex * width + xIndex;
        	grid[index] = 1;
    	}
		foreach (Vector3 powerPellet in PowerPellets)
		{
		    int xIndex = Mathf.RoundToInt(powerPellet.x - minX);
        	int yIndex = Mathf.RoundToInt(powerPellet.y - minY);
        	int index = yIndex * width + xIndex;
        	grid[index] = 2;
		}
    
    	// Inactive pellets remain 0 (already initialized)
    
    
    	// Mark ghosts as 4 + state value
		for (int i =0; i < ghostPositions.Length; i++)
		{
			Vector3 ghost = ghostPositions[i];
        	int ghostX = Mathf.RoundToInt(ghost.x - minX);
        	int ghostY = Mathf.RoundToInt(ghost.y - minY);
        	int ghostIndex = ghostY * width + ghostX;

			// represent ghost state by adding state value to base ghost value
			grid[ghostIndex] = 5 + ghostStates[i];
		}

        // Mark Pacman as 2
    	int pacmanX = Mathf.RoundToInt(pacmanPosition.x - minX);
    	int pacmanY = Mathf.RoundToInt(pacmanPosition.y - minY);
    	int pacmanIndex = pacmanY * width + pacmanX;
		if (pacmanAttack)
		{
            grid[pacmanIndex] = 4;
		}
		else
		{
			grid[pacmanIndex] = 3;
		}
    
    	return grid;
	}

    // Helper to convert back to 2D coordinates if needed
    public Vector2Int GetGridCoordinates(int flatIndex, int width)
    {
        int z = flatIndex / width;
        int x = flatIndex % width;
        return new Vector2Int(x, z);
    }
	public static float Vector3ManhattanDistance(Vector3 a, Vector3 b)
	{
    	return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
	}
	public static float Vector2ManhattanDistance(Vector3 a, Vector3 b)
	{
    	return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}
	public static float Vector2and3ManhattanDistance(Vector3 a, Vector2 b)
	{
    	return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}
}
// Original Pacman
// public class Pacman : MonoBehaviour
// {
//     public Movement movement { get; private set; }
//     // public AnimatedSprite deathSequence;
//     public SpriteRenderer spriteRenderer;
//     private new Collider2D collider;
//     public bool pacmanAttack = false;

//     public string inputDirection; // This is used to store the input direction of the player
    
    
//     private void Awake(){
//         this.movement = GetComponent<Movement>();
//         this.spriteRenderer = GetComponent<SpriteRenderer>();
//         collider = GetComponent<Collider2D>();
//     }
    
//     private void Update(){

//         if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
//             this.movement.SetDirection(Vector2.up);
//             if (Input.GetKeyDown(KeyCode.UpArrow)){
//                 this.inputDirection = "up";
//             } 
//             else{
//                 this.inputDirection = "w";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
//             this.movement.SetDirection(Vector2.down);
//             if (Input.GetKeyDown(KeyCode.DownArrow)){
//                 this.inputDirection = "down";
//             } 
//             else{
//                 this.inputDirection = "s";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
//             this.movement.SetDirection(Vector2.left);
//             if (Input.GetKeyDown(KeyCode.LeftArrow)){
//                 this.inputDirection = "left";
//             } 
//             else{
//                 this.inputDirection = "a";
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
//             this.movement.SetDirection(Vector2.right);
//             if (Input.GetKeyDown(KeyCode.RightArrow)){
//                 this.inputDirection = "right";
//             } 
//             else{
//                 this.inputDirection = "d";
//             }
//         }
//         float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x); // Gets the angle of dir
//         this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Sets rot to the angle
//     }

//     public void ResetState()
//     {
//         enabled = true;
//         spriteRenderer.enabled = true;
//         collider.enabled = true;
//         // deathSequence.enabled = false;
//         movement.ResetState();
//         gameObject.SetActive(true);
//     }
    
// }
