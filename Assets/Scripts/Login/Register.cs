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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInput.isFocused)
            {
                passwordInput.Select();
            }
            else if (passwordInput.isFocused)
            {
                emailinput.Select();
            }
            else if (emailinput.isFocused)
            {
                usernameInput.Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(RegisterUser());
        }
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
        int maxRetries = 3; // Maximum number of retries
        int retryDelay = 1; // Delay between retries in seconds
        int attempt = 0; // Current attempt counter

        while (attempt < maxRetries)
        {
            UnityWebRequest www = UnityWebRequest.Post(registerUrl + "SQL/register.php", form);
            yield return www.SendWebRequest();  
            Debug.Log(www.downloadHandler.text);
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                confirmationText.text = "Failed to connect to server - attempt " + attempt + " of " + maxRetries;
                yield return new WaitForSeconds(retryDelay);
                confirmationText.text = " ";
                attempt++;
            }
            else 
            {  
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                if (response.success == true)
                {
                    Debug.Log("User registered successfully");
                    confirmationText.text = "User registered successfully, returning to login screen";
                    yield return new WaitForSeconds(2);
                    confirmationText.text = "";
                    MainManager.Instance.user_id = response.user_id;
                    MainManager.Instance.username = usernameInput.text;
                    SceneManager.LoadScene("Welcome Screen");
                    yield break;
                }
                else if (response.success == false)
                {
                    Debug.Log($"Failed to register user: {response.message}");
                    if (response.message == "Username already exists")
                    {
                        confirmationText.text = "Username already exists. Please choose a different username.";
                    }
                    else
                    {
                        confirmationText.text = "Internal server error";
                    }
                    yield return new WaitForSeconds(2);
                    confirmationText.text = "";
                    yield break;
                }
            }
        }
    }

}
