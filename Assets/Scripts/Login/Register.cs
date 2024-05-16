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
    public InputField emailinput;
    public Button registerButton;
    public Button goToLoginButton;
    public TMP_Text confirmationText;
    private string registerUrl;


    // Start is called before the first frame update
    void Start()
    {
        //confirmationText = GetComponent<TextMeshProUGUI>();
        registerButton.onClick.AddListener(() => StartCoroutine(RegisterUser()));
        goToLoginButton.onClick.AddListener(goToLoginScene);
        confirmationText.text = "";
        registerUrl = MainManager.Instance.dataserver;
        
    }

    void goToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }


IEnumerator RegisterUser()
    {
            // Check if any field is empty
        if (string.IsNullOrWhiteSpace(usernameInput.text) ||
            string.IsNullOrWhiteSpace(passwordInput.text) ||
            string.IsNullOrWhiteSpace(emailinput.text))
        {
            confirmationText.text = "Please fill in all fields.";
            yield return new WaitForSeconds(2); // Display the message for 2 seconds
            confirmationText.text = "";
            yield break; // Stop the coroutine if validation fails
        }
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("email", emailinput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(registerUrl + "SQL/register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                confirmationText.text = "Failed to connect to server";
                yield return new WaitForSeconds(2);
                confirmationText.text = " ";
                
            }
            else
        {
            // Check the response from the server
            if (www.downloadHandler.text == "Username already exists")
            {
                // Trigger a special message
                Debug.Log("Username already exists. Please choose a different username.");
                confirmationText.text = www.downloadHandler.text;
                yield return new WaitForSeconds(2);
                confirmationText.text = "";
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                confirmationText.text = "User registered successfully, go to log in screen";
                yield return new WaitForSeconds(2);
                confirmationText.text = "";
            }
        }
        }
    }


}
