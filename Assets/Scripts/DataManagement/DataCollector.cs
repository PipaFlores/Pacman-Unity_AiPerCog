using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Specialized;

public class DataCollector : MonoBehaviour
{
    public GameObject player;
    public GameObject[] ghosts; // Assuming you have multiple enemies
    private List<GameDataPoint> dataPointsList = new List<GameDataPoint>();

    public string dataserver = "http://localhost/"; // path to the server (e.g., http://localhost/)

    public string source = "local"; // or wherever the game is deployed (e.g., itch.io or ngrok)

    public string dataType = "json";

    private string username; // FIXME

 
    // GameManager calls Startdatacollection at the beginning of each round. Datacollection stops
    // with the CancelInvoke in SaveData() at GameOver or !HasRemainingPellets

    public void Startdatacollection(){
        InvokeRepeating(nameof(CollectGameData), 1f, 1f); 
        // Adjust timing as needed
        // Consider getting the session_id, username, or other one-time info here.
    }
    
    private void CollectGameData()
    {
        Vector2 playerPos = player.transform.position;
        Vector2[] ghostsPos = new Vector2[ghosts.Length];
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghostsPos[i] = ghosts[i].transform.position;
        }

        // Assume you have a way to access the current score and lives (e.g., through a GameManager)
        int currentScore = GameManager.Instance.score;
        int lives = GameManager.Instance.lives;
        float time = Time.timeSinceLevelLoad;

        GameDataPoint dataPoint = new GameDataPoint
        {
            client_source = this.source,
            playerPosition = playerPos,
            ghostsPositions = ghostsPos,
            score = currentScore,
            livesRemaining = lives,
            timeElapsed = time
        };
        dataPointsList.Add(dataPoint);
    }

    private System.Collections.IEnumerator SendGameData(string gameData)
    {
        if (dataType == "json"){
            string url = dataserver + "savegamedata_json.php";
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
        else if (dataType == "csv"){
            string url = dataserver + "savegamedata_csv.php";
            // Use a form to send CSV data
            WWWForm form = new WWWForm();
            form.AddField("data", gameData);

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
                }
            else{
                Debug.Log("Game data uploaded successfully.");
                this.dataPointsList.Clear();
                }
        }
        
    }

        public void SaveData()
    {
        
        if (dataType == "json"){
            CancelInvoke();
            GameDataContainer container = new GameDataContainer { dataPoints = dataPointsList };
            string json = JsonUtility.ToJson(container, true);
            StartCoroutine(SendGameData(json));
        }
        else if (dataType == "csv"){
            CancelInvoke();
            List<string> csvLines = new List<string>(); // headers initialized in server's php
            foreach (var dataPoint in dataPointsList){
                csvLines.Add(dataPoint.ToCsvString()); // Implement ToCsvString in GameDataPoint
            }
            
            string csvString = string.Join("\n", csvLines);
            StartCoroutine(SendGameData(csvString));
        }
        else{
            Debug.Log("Select a valid data type (json or csv)");
        }        
    }
    // public void SaveData()  // Local save data
    // {
    //     GameDataContainer container = new GameDataContainer { dataPoints = dataPointsList };
    //     string json = JsonUtility.ToJson(container, true);

    //     // Logic to save json to a file or send it to a server
    //     string fileName = @"D:\PacmanUnity\Logs\Datacollection\data.json";
    //     string directory = Path.GetDirectoryName(fileName);
    //     if (!Directory.Exists(directory)){
    //         Directory.CreateDirectory(directory);
    //     }
    //     System.IO.File.WriteAllText(fileName, json);
    //     //@"D:\PacmanUnity\Logs\Datacollection\data.json"
    //     // "../Datacollection2/data.json" writes to d:\Datacoll....
    //     //"file:../Datacollection2/data.json" gets the local path but adds the "file:.."
    // }

    // void OnEnable()
    // {
    //     CancelInvoke();
    //     InvokeRepeating(nameof(CollectGameData), 1f, 1f); // Adjust timing as needed
    // }

    // void OnDisable()
    // {
    //     CancelInvoke();
    //     SaveData();
    //     // Optionally clear the list if planning to reuse this instance
    //     dataPointsList.Clear();
    // }

    [System.Serializable]
    public class GameDataPoint
    {
        public string client_source;  // TODO: Gather this at the beginning of the game or the end of the login process
        public Vector2 playerPosition;
        public Vector2[] ghostsPositions;
        public int score;
        public int livesRemaining;
        public float timeElapsed;
        public string ToCsvString()
        {
            // Example for formatting; adjust based on your actual fields
            return $"{client_source};{playerPosition.x};{playerPosition.y};{string.Join(";", ghostsPositions.Select(gp => gp.ToString()))};{score};{livesRemaining};{timeElapsed}";
        }
    }

    [System.Serializable]
    public class GameDataContainer
    {
        public List<GameDataPoint> dataPoints;
    }
}
