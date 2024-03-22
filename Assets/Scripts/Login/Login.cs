using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;
    public string serverUrl = "https://tapir-immune-chimp.ngrok-free.app/";

    public TMP_Text errorMsg;
    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(LoginUser()));
        goToRegisterButton.onClick.AddListener(moveToRegister);
        errorMsg.text = "";

    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else if (www.downloadHandler.text == "Login successful"){
                // Process the response
                Debug.Log(www.downloadHandler.text);
                PlayerPrefs.SetString("username", usernameInput.text);
                loadWelcomeScreen();
            }
            else if (www.downloadHandler.text == "Invalid password"){
                Debug.Log(www.downloadHandler.text);
                errorMsg.text = www.downloadHandler.text;
                yield return new WaitForSeconds(2);
                errorMsg.text = "";
            }
            else if (www.downloadHandler.text == "Username does not exist"){
                Debug.Log(www.downloadHandler.text);
                errorMsg.text = www.downloadHandler.text;
                yield return new WaitForSeconds(2);
                errorMsg.text = "";
            }
            else if (www.downloadHandler.text == "Error opening the file."){
                Debug.Log(www.downloadHandler.text);
                errorMsg.text = www.downloadHandler.text;
                yield return new WaitForSeconds(2);
                errorMsg.text = "";
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
