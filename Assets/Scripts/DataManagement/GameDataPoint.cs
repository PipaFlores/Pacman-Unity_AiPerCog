using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Specialized;

[System.Serializable]
public class GameDataPoint
{
    public Vector2 playerPosition;
    public Vector2[] ghostsPositions;
    public int score;
    public int livesRemaining;
    public float timeElapsed;
    public string ToCsvString()
    {
        // Example for formatting; adjust based on your actual fields
        return $"{playerPosition.x};{playerPosition.y};{string.Join(";", ghostsPositions.Select(gp => gp.ToString()))};{score};{livesRemaining};{timeElapsed}";
    }
    // Add the MYSQL formatting here
}

[System.Serializable]
public class GameDataContainer
{
    public List<GameDataPoint> dataPoints;
}