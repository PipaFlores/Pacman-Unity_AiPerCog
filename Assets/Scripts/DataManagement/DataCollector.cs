using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using UnityEngine.UIElements;

// GameManager calls Startdatacollection at the beginning of each round. Datacollection stops
// with the CancelInvoke in SaveData() at GameOver or !HasRemainingPellets
public class DataCollector : MonoBehaviour
{
    public GameObject player;
    public GameObject[] ghosts; // Assuming you have multiple enemies
    private List<GameDataPoint> dataPointsList = new List<GameDataPoint>();

    private string dataserver;

    //public bool localSave = true; // local save data
    public bool serverSave = true; // server save data
    //public string localPath = @"C:\LocalData\pabflore\Unity_Projects\unity_pacman\Logs\gamedata.json"; // local path to save data

    public float saveInterval = 0.1f; // Save data every x seconds  

    // Variables to store the game duration
    private double game_duration;
    private System.DateTime game_startTime;
    private System.DateTime game_endTime;

    // list of pellet state
    //  List<PelletState> pellets;
 
    public void Awake(){
        // Get the dataserver url from the MainManager
        dataserver = MainManager.Instance.dataserver;
    }

    public void Startdatacollection(){
        InvokeRepeating(nameof(CollectGameData), 0.2f, saveInterval);
        game_startTime = System.DateTime.UtcNow;
        // Adjust timing as needed
        // Consider getting the session_id, username, or other one-time info here.
    }
    
    private void CollectGameData()
    {
        Vector2 playerPos = player.transform.position;
        Vector2[] ghostsPos = new Vector2[ghosts.Length];
        int[] ghostsState = new int[ghosts.Length];
        bool pacmanAttack = player.GetComponent<Pacman>().pacmanAttack;
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghostsPos[i] = ghosts[i].transform.position;
            ghostsState[i] = GetGhostState(ghosts[i]);
        }

        // Assume you have a way to access the current score and lives (e.g., through a GameManager)
        int currentScore = GameManager.Instance.score;
        int lives = GameManager.Instance.lives;
        float time = GameManager.Instance.round_timeElapsed;

        GameDataPoint dataPoint = new GameDataPoint
        {
            playerPosition = playerPos,
            pacmanAttack = pacmanAttack,
            ghostsPositions = ghostsPos,
            ghostStates = ghostsState,
            score = currentScore,
            pelletsRemaining = GameManager.Instance.remainingPellets,
            powerPelletsRemaining = GameManager.Instance.remainingPills,
            livesRemaining = lives,
            timeElapsed = time,
            // pellets = pellets
        };
        dataPointsList.Add(dataPoint);
    }

    private System.Collections.IEnumerator SendGameData(string gameData)
    {
        string url = dataserver + "SQL/savegamedata_json.php";
        UnityWebRequest www = UnityWebRequest.Post(url, gameData, "application/json");
        yield return www.SendWebRequest();
    
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Game data uploaded successfully.");
            this.dataPointsList.Clear();
        }
    
        
    }

    public void SaveData()
    {
        game_endTime = System.DateTime.UtcNow;
        game_duration = (game_endTime - game_startTime).TotalSeconds;
        
        CancelInvoke();
        GameDataContainer container = new GameDataContainer 
        { 
            dataPoints = dataPointsList,
            date_played = game_startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            game_duration = game_duration,
            session_number = MainManager.Instance.session_number,
            game_in_session = MainManager.Instance.games_in_session,
            user_id = MainManager.Instance.user_id,
            source = MainManager.Instance.source,
            win = GameManager.Instance.win
            };
        string json = JsonUtility.ToJson(container, true);
        //if (localSave){
        //    LocalSaveData(json);
        //}
        if (serverSave){
            StartCoroutine(SendGameData(json));
        }
          
    }
    // public void LocalSaveData(string json)  // Local save data in json format
    // {
    //     string directory = Path.GetDirectoryName(localPath);
    //     if (!Directory.Exists(directory)){
    //         Directory.CreateDirectory(directory);
    //     }
    //     System.IO.File.WriteAllText(localPath, json);
    //     //@"D:\PacmanUnity\Logs\Datacollection\data.json"
    //     // "../Datacollection2/data.json" writes to d:\Datacoll....
    //     //"file:../Datacollection2/data.json" gets the local path but adds the "file:.."
    // }

    private int GetGhostState(GameObject ghost){
        if (ghost.GetComponent<GhostHome>().enabled){
            if (ghost.GetComponent<Ghost>().eaten){
                return 4; // eaten
            }
            else return 0; // home (not eaten)
            }    
        else if (ghost.GetComponent<GhostFrightened>().enabled){  // Frightened state overlaps with scatter and chase, so check it first
            return 3;
        }
        else if (ghost.GetComponent<GhostChase>().enabled){
            return 2;
        }
        else if(ghost.GetComponent<GhostScatter>().enabled){
            return 1;
        }
        else{
            return -666; // error code
        }
    }
}
//     public void UpdatePellets(Dictionary<Vector2, bool> pelletsDictionary)
//     {
//         pellets = new List<PelletState>();
//         foreach (var pellet in pelletsDictionary)
//         {
//             pellets.Add(new PelletState(pellet.Key, pellet.Value));
//         }
//     }
// }

    // ------------------------------------------
    // MOVED GAMEDATAPOINT AND GAMEDATACONTAINER TO GameDataPoint.cs
    // ------------------------------------------
    // [System.Serializable]
    // public class GameDataPoint
    // {
    //     public string client_source;  // TODO: Gather this at the beginning of the game or the end of the login process
    //     public Vector2 playerPosition;
    //     public Vector2[] ghostsPositions;
    //     public int score;
    //     public int livesRemaining;
    //     public float timeElapsed;
    //     public string ToCsvString()
    //     {
    //         // Example for formatting; adjust based on your actual fields
    //         return $"{client_source};{playerPosition.x};{playerPosition.y};{string.Join(";", ghostsPositions.Select(gp => gp.ToString()))};{score};{livesRemaining};{timeElapsed}";
    //     }
    // }

    // [System.Serializable]
    // public class GameDataContainer
    // {
    //     public List<GameDataPoint> dataPoints;
    // }

