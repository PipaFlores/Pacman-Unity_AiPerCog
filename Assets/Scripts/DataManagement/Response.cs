using System;
// This class is used to define the structure of the response from the server at login
[System.Serializable]
public class Response {
    public bool success;
    public string message;
    public int user_id;  // Assuming user_id is an integer
}