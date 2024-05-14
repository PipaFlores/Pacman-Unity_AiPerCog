using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Static instance, which allows it to be accessed from any other script
    // Stores variables that need to be accessed from multiple scenes
    // https://learn.unity.com/tutorial/implement-data-persistence-between-scenes?uv=2022.3#
    public static MainManager Instance;

    // Variables to store the desired information
    // Username and user_id retrieved by the login script
    public string username;
    public int user_id;
    //public int session_number;
    public int game_number;
    public string source = "local"; // or wherever the game is deployed (e.g., itch.io or ngrok)

    public string dataserver = "https://aipercog-24.it.helsinki.fi/"; // path to the server (e.g., http://localhost/) paths to the PHP files are appended to this in the Register, Login, and DataCollector scripts

    // Awake is called in login scene and a singleton pattern is used to ensure only one instance of the MainManager exists
    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

}
