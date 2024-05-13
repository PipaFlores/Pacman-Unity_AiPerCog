﻿using System;
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
    private string serverUrl;

    public TMP_Text errorMsg;
    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(LoginUser()));
        goToRegisterButton.onClick.AddListener(moveToRegister);
        errorMsg.text = "";
        serverUrl = MainManager.Instance.dataserver;

    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl + "SQL/login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                errorMsg.text = www.error + www.downloadHandler.text;
            } else {
                Debug.Log("Response: " + www.downloadHandler.text);
                var response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
                if (response.success == true)
                {
                    MainManager.Instance.username = usernameInput.text;
                    MainManager.Instance.user_id = response.user_id;
                    // PlayerPrefs.SetString("username", usernameInput.text);
                    // PlayerPrefs.SetInt("user_id", response.user_id);
                    // PlayerPrefs can help store data locally on the device, but it is neither safe nor scalable
                    loadWelcomeScreen();
                }
                else
                {
                    errorMsg.text = response.message;
                    yield return new WaitForSeconds(2);
                    errorMsg.text = "";
                }
            }
           
        }
    }

    void moveToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    void loadWelcomeScreen()
    {
        SceneManager.LoadScene("Welcome Screen");
    }
}
