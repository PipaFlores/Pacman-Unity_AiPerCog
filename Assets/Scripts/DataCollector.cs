using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

public class GameDataCollector : MonoBehaviour
{
    public GameObject player;
    public GameObject[] ghosts; // Assuming you have multiple enemies
    private List<GameDataPoint> dataPointsList = new List<GameDataPoint>();

    void Start()
    {
        // Initialize data collection or other startup logic here
    }

    void CollectGameData()
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
            playerPosition = playerPos,
            ghostsPositions = ghostsPos,
            score = currentScore,
            livesRemaining = lives,
            timeElapsed = time
        };
        dataPointsList.Add(dataPoint);
    }

    // private System.Collections.IEnumerator SendGameData(string gameDataJson)
    // {
    //     string url = "http://yourserver.com/saveGameData.php";
    //     UnityWebRequest www = UnityWebRequest.Post(url, gameDataJson);
    //     www.SetRequestHeader("Content-Type", "application/json");
    //     yield return www.SendWebRequest();
    
    //     if (www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log(www.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Game data uploaded successfully.");
    //     }
    // }
    public void SaveData()
    {
        GameDataContainer container = new GameDataContainer { dataPoints = dataPointsList };
        string json = JsonUtility.ToJson(container, true);
        //var projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        //var path = Path.Combine(projectFolder, @"TestData\data.json");
        // Logic to save json to a file or send it to a server
        string fileName = @"D:\PacmanUnity\Logs\Datacollection\data.json";
        string directory = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(directory)){
            Directory.CreateDirectory(directory);
        }
        System.IO.File.WriteAllText(fileName, json);
        //@"D:\PacmanUnity\Logs\Datacollection\data.json"
        // "../Datacollection2/data.json" writes to d:\Datacoll....
        //"file:../Datacollection2/data.json" gets the local path but adds the "file:.."
    }

    void OnEnable()
    {
        CancelInvoke();
        InvokeRepeating(nameof(CollectGameData), 1f, 1f); // Adjust timing as needed
    }

    void OnDisable()
    {
        CancelInvoke();
        SaveData();
        // Optionally clear the list if planning to reuse this instance
        dataPointsList.Clear();
    }

    [System.Serializable]
    public class GameDataPoint
    {
        public Vector2 playerPosition;
        public Vector2[] ghostsPositions;
        public int score;
        public int livesRemaining;
        public float timeElapsed;
    }

    [System.Serializable]
    public class GameDataContainer
    {
        public List<GameDataPoint> dataPoints;
    }
}
