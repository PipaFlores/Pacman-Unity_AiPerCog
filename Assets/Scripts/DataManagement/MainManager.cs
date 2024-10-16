using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script defines the MainManager class, which is a singleton used to manage and persist data across different scenes in a Unity game. 
// The MainManager class holds important information such as the username, user_id, session number, and game number, which are retrieved from the server.
// It also tracks the number of games played in a session and the source of the game deployment. 
// The class ensures that only one instance of MainManager exists throughout the game using the Awake method, which implements the singleton pattern.
// This allows other scripts to access and modify these variables as needed, facilitating data persistence and management across different game scenes.

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
    public int session_number; // Number of current session, retrieved from the server on welcome screen. A new session is recorder only when a first gamestate data is sent to the server. this avoids empty sessions being recorded.
    public int game_number;  // Number of overall games played, retrieved from the server on welcome screen. A new game is recorded only when gamestate data is sent to the server. this avoids empty games being recorded.

    public int games_in_session = 0; // Number of games in a session, a counter increased by the GameManager script on Newround()

    public string source = "Unity-Test"; // or wherever the game is deployed (e.g., itch.io). Defined in editor.

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
