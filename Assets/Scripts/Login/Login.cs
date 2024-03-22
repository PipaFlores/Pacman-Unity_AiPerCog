using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public string loginUrl = "https://tapir-immune-chimp.ngrok-free.app/";
    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(LoginUser()));
        goToRegisterButton.onClick.AddListener(moveToRegister);

    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else if (www.downloadHandler.text == "Login successful"){
                // Process the response
                Debug.Log(www.downloadHandler.text);
                loadWelcomeScreen();
            }
            else if (www.downloadHandler.text == "Invalid password"){
                Debug.Log(www.downloadHandler.text);
            }
            else if (www.downloadHandler.text == "Username does not exist"){
                Debug.Log(www.downloadHandler.text);
            }
            else if (www.downloadHandler.text == "Error opening the file."){
                Debug.Log(www.downloadHandler.text);
            }

           
        }
    }

    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    void loadWelcomeScreen()
    {
        SceneManager.LoadScene("Pacman");
    }
}
