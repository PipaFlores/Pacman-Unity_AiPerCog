using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class Register : MonoBehaviour
{

    public InputField usernameInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;

    public GameObject successtext;
    public GameObject failuretext;
    public GameObject alreadyexiststext;
    public string registerUrl = "https://tapir-immune-chimp.ngrok-free.app/";
    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        registerButton.onClick.AddListener(() => StartCoroutine(RegisterUser()));
        goToLoginButton.onClick.AddListener(goToLoginScene);
        failuretext.SetActive(false);
        successtext.SetActive(false);
        alreadyexiststext.SetActive(false);
    }

    void goToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }


IEnumerator RegisterUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(registerUrl + "register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                failuretext.SetActive(true);
                yield return new WaitForSeconds(2);
                failuretext.SetActive(false);
                
            }
            else
        {
            // Check the response from the server
            if (www.downloadHandler.text == "Username already exists")
            {
                // Trigger a special message
                Debug.Log("Username already exists. Please choose a different username.");
                alreadyexiststext.SetActive(true);
                yield return new WaitForSeconds(2);
                alreadyexiststext.SetActive(false);
            }
            else
            {
                Debug.Log("User registered successfully.");
                successtext.SetActive(true);
            }
        }
        }
    }


}
