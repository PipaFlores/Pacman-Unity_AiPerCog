using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Static instance, which allows it to be accessed from any other script
    // Stores variables that need to be accessed from multiple scenes
    public static MainManager Instance;

    // Variables to store the desired information
    public string username;
    public int user_id;
    public int session_number;
    public int game_number;

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
