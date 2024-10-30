using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using UnityEngine.UIElements;
using System.Collections;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.Purchasing;

// The DataCollector class is responsible for gathering and managing game data during each round.
// It collects data at regular intervals, including player and ghost positions, game state, and scores.
// The collected data can be saved locally or sent to a remote server for analysis.
// Data collection starts at the beginning of each round and stops when the game ends or specific conditions are met.
public class DataCollector : MonoBehaviour
{
    public GameObject player;  // Reference to the player (Pacman)
    public GameObject[] ghosts; // Array of ghost enemies in the game
    private List<GameDataPoint> dataPointsList = new List<GameDataPoint>(); // List to store collected game data points

    private string dataserver; // URL of the data server for remote saving

    public bool localSave = true; // Flag to determine if data should be saved locally
    public bool serverSave = true; // Flag to determine if data should be saved to the server

    public bool data_upload_success {get ; private set; } // Flag to check if the data has been uploaded successfully

    private float saveInterval = 0.050f; // Interval in seconds for data collection

    // Variables to track the duration of the game
    private double game_duration;
    private System.DateTime game_startTime;
    private System.DateTime game_endTime;


    public void Awake()
    {
        // Retrieve the data server URL from the MainManager instance
        dataserver = MainManager.Instance.dataserver;
    }

    public void Startdatacollection()
    {
        // Start collecting game data at regular intervals
        InvokeRepeating(nameof(CollectGameData), 0.2f, saveInterval);
        data_upload_success = false;
        game_startTime = System.DateTime.UtcNow; // Record the start time of the game
    }

    private void CollectGameData()
    {
        // Collect player data
        Vector2 playerPos = player.transform.position; // Player's current position
        bool pacmanAttack = player.GetComponent<Pacman>().pacmanAttack; // Player's attack state
        string inputDirection = player.GetComponent<Pacman>().inputDirection; // Player's input direction
        player.GetComponent<Pacman>().inputDirection = "none"; // Reset input direction after reading
        string movementDirection = dir_to_string(player.GetComponent<Movement>().direction); // Player's movement direction

        // Collect ghosts' data
        Vector2[] ghostsPos = new Vector2[ghosts.Length]; // Array to store positions of all ghosts
        int[] ghostsState = new int[ghosts.Length]; // Array to store states of all ghosts
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghostsPos[i] = ghosts[i].transform.position; // Get each ghost's position
            ghostsState[i] = GetGhostState(ghosts[i]); // Get each ghost's state
        }

        // Collect game manager data
        int[] PowerPelletStates = new int[GameManager.Instance.PowerPelletStates.Length];
        for (int i = 0; i < GameManager.Instance.PowerPelletStates.Length; i++)
        {
            PowerPelletStates[i] = GameManager.Instance.PowerPelletStates[i]; // Get power pellet states
        }

        int currentScore = GameManager.Instance.score; // Current score
        int lives = GameManager.Instance.lives; // Remaining lives
        float time = Mathf.Floor(GameManager.Instance.round_timeElapsed * 100) / 100f; // Elapsed time rounded to two decimal places
        int pelletsRemaining = GameManager.Instance.remainingPellets; // Remaining pellets
        int powerPelletsRemaining = GameManager.Instance.remainingPills; // Remaining power pellets
        int fruitState_1 = GameManager.Instance.fruitState_1; // State of the first fruit
        int fruitState_2 = GameManager.Instance.fruitState_2; // State of the second fruit

        // Create a new data point with the collected data
        GameDataPoint dataPoint = new GameDataPoint
        {
            playerPosition = playerPos,
            pacmanAttack = pacmanAttack,
            ghostsPositions = ghostsPos,
            ghostStates = ghostsState,
            score = currentScore,
            powerPelletStates = PowerPelletStates,
            pelletsRemaining = pelletsRemaining,
            powerPelletsRemaining = powerPelletsRemaining,
            livesRemaining = lives,
            timeElapsed = time,
            fruitState_1 = fruitState_1,
            fruitState_2 = fruitState_2,
            inputDirection = inputDirection,
            movementDirection = movementDirection
        };
        dataPointsList.Add(dataPoint); // Add the data point to the list
    }

    private IEnumerator SendGameData(string gameData)
    {
        string url = dataserver + "SQL/savegamedata_json.php";
        int maxRetries = 3; // Maximum number of retries
        int retryDelay = 2; // Delay between retries in seconds
        int attempt = 0; // Current attempt counter
        GameManager.Instance.UserNotification.text = "Uploading game data...";

        while (attempt < maxRetries)
        {
            UnityWebRequest www = UnityWebRequest.Post(url, gameData, "application/json");
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success){
                Debug.Log("Received: " + www.downloadHandler.text);
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(www.downloadHandler.text);
                if (response.success)
                {
                    Debug.Log("Game data uploaded successfully."); // Log success message
                    this.dataPointsList.Clear(); // Clear the data points list after successful upload
                    data_upload_success = true;
                    yield return StartCoroutine(GameManager.Instance.ErrorMsg("Data uploaded successfully."));
                    yield break; // Exit the coroutine successfully
                } 
                else {
                    Debug.Log($"Attempt {attempt + 1} failed: {response.message}"); // Log the error
                    attempt++;
                    if (attempt < maxRetries)
                    {
                        yield return new WaitForSeconds(retryDelay); // Wait before retrying
                    }
                }
            }
            else
            {
                Debug.Log($"Attempt {attempt + 1} failed: {www.error}"); // Log the error
                attempt++;
                if (attempt < maxRetries)
                {
                    yield return new WaitForSeconds(retryDelay); // Wait before retrying
                }
            }
        }
        // If all attempts fail, notify the user
        yield return StartCoroutine(GameManager.Instance.ErrorMsg("Failed to upload game data."));
        Debug.Log("Failed to upload game data after multiple attempts.");
    }
 

    // SaveData sends the data to the server, or locally. Is called at the end of the game (either when lives run out, or when a level is completed).
    public IEnumerator SaveData()
    {
        // Calculate game duration and save data
        game_endTime = System.DateTime.UtcNow;
        game_duration = (game_endTime - game_startTime).TotalSeconds;

        CancelInvoke(); // Stop data collection
        GameDataContainer container = new GameDataContainer
        {
            dataPoints = dataPointsList,
            date_played = game_startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            game_duration = game_duration,
            session_number = MainManager.Instance.session_number,
            game_in_session = MainManager.Instance.games_in_session,
            total_games = MainManager.Instance.total_games,
            user_id = MainManager.Instance.user_id,
            source = MainManager.Instance.source,
            win = GameManager.Instance.win,
            level = GameManager.Instance.level
        };
        string json = JsonUtility.ToJson(container, true);
        
        if (localSave)
        {
            LocalSaveData(json);
        }
        
        if (serverSave)
        {
            data_upload_success = false; // Reset the flag
            yield return StartCoroutine(SendGameData(json));
        }
    }


    public void LocalSaveData(string json)
    {
        // Save data locally in JSON format
        string localPath = @"C:\LocalData\pabflore\Unity_Projects\unity_pacman\Logs\gamedata.json"; // Local path for saving data
        string directory = Path.GetDirectoryName(localPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory); // Create directory if it doesn't exist
        }
        System.IO.File.WriteAllText(localPath, json); // Write JSON data to file
        Debug.Log("Data saved locally at " + localPath); // Log success message
    }

    private int GetGhostState(GameObject ghost)
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

    private string dir_to_string(Vector2 dir)
    {
        // Convert a Vector2 direction to a string representation
        if (dir == Vector2.up)
        {
            return "up";
        }
        else if (dir == Vector2.down)
        {
            return "down";
        }
        else if (dir == Vector2.left)
        {
            return "left";
        }
        else if (dir == Vector2.right)
        {
            return "right";
        }
        else
        {
            return "none"; // No direction
        }
    }
}

