using System;
// This class is used to define the structure of the response from the server at login
[System.Serializable]
public class LoginResponse {
    public bool success;
    public string message;
    public int user_id;  // Assuming user_id is an integer
}

public class ServerResponse {
    public bool success;
    public string message;
}
