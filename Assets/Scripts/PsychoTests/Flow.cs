using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;

// This scripts controls the Self-Assessment Manikin (SAM) survey, which is used to measure 
// the emotional response of the player after a defined period of gameplay.
public class Flow : MonoBehaviour
{

    public Button gameButton; // Button to go back to game
    public Button exitButton; // Button to go back to welcome screen
    public Button submitButton; // Button to submit the survey
    private string SERVERSCRIPT = "SQL/Flow.php"; // Path to the server script appended to the server URL
    
    public Text infoMsg; // Text field to display error messages

    public GameObject[] fss_ButtonsParents; // Array of parent GameObjects for FSS buttons
    private Button[][] fss_ButtonsArray; // Arrays to store the FSS_1, FSS_2, FSS_3, FSS_4, FSS_5, FSS_6, FSS_7, FSS_8 buttons
    private int[] fss_Values; // Array to store the FSS values
    
    // Previous selected buttons for color change
    private Button[] previousFSS_Buttons; // Array to store the previous FSS buttons
    private bool submitted = false; // Whether the survey has been submitted

    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(SubmitSurvey);
        submitButton.interactable = false;
        gameButton.onClick.AddListener(BackToGame);
        gameButton.interactable = false;
        exitButton.onClick.AddListener(BackToWelcome);
        exitButton.interactable = false;
        SERVERSCRIPT = MainManager.Instance.dataserver + SERVERSCRIPT;
        // Initialize the 2D array with the correct dimensions
        fss_ButtonsArray = new Button[fss_ButtonsParents.Length][];
        fss_Values = new int[fss_ButtonsParents.Length];
        previousFSS_Buttons = new Button[fss_ButtonsParents.Length];

        // Initialize buttons
        for (int i = 0; i < fss_ButtonsParents.Length; i++)
        {
            
            fss_ButtonsArray[i] = InitializeButtons(fss_ButtonsParents[i]);
            Debug.Log($"FSS_{i + 1} buttons initialized");
        }
        Debug.Log("Adding listeners to the buttons");
        // Add listeners to the buttons
        for (int i = 0; i < fss_ButtonsParents.Length; i++)
        {
            int capturedRow = i; // Capture the row index
            Add_listeners(fss_ButtonsArray[i], (value) => {
                OnFSSButtonClicked(capturedRow, value);
            });
        }
        
    }

    void Add_listeners(Button[] buttons, Action<int> callback)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Button names start from "1"
            buttons[i].onClick.AddListener(() => callback(index + 1));
        }
    }

    void Update()
    {
        if (fss_Values[0] != 0 && fss_Values[1] != 0 && fss_Values[2] != 0 && fss_Values[3] != 0 && fss_Values[4] != 0 && fss_Values[5] != 0 && fss_Values[6] != 0 && fss_Values[7] != 0 && !submitted)
        {
            submitButton.interactable = true;
        }
    }   
    private Button[] InitializeButtons(GameObject parent)
    {
        Button[] buttons = new Button[7]; // 7 buttons (likert choices) for FSS
        Debug.Log($"Initializing FSS_{parent.name} buttons");
        for (int i = 0; i < buttons.Length; i++)
        {
            string buttonName = (i + 1).ToString(); // Button names are "1", "2", "3", etc.
            Transform buttonTransform = parent.transform.Find(buttonName);
            Debug.Log($"Found FSS_{parent.name}_{buttonName} button");
            if (buttonTransform != null)
            {
                buttons[i] = buttonTransform.GetComponent<Button>();
                Debug.Log($"FSS_{parent.name}_{i + 1} button initialized"); 
            }
        }
        return buttons;
    }

    private void OnFSSButtonClicked(int arrayIndex, int value)
    {
        Debug.Log($"FSS_{arrayIndex + 1} button {value} clicked");
        fss_Values[arrayIndex] = value;

        // Reset the color of the previous button
        if (previousFSS_Buttons[arrayIndex] != null)
        {
            previousFSS_Buttons[arrayIndex].GetComponent<Image>().color = Color.white;
            previousFSS_Buttons[arrayIndex].GetComponent<AnimatedButton>().enabled = false;
        }

        // Set the color of the current button to green
        Button currentButton = fss_ButtonsArray[arrayIndex][value - 1];
        currentButton.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);
        currentButton.GetComponent<AnimatedButton>().enabled = true;
        // Update the previous button
        previousFSS_Buttons[arrayIndex] = currentButton;
    }

    void BackToGame()
    {
        SceneManager.LoadScene("Pacman");
    }

    void BackToWelcome()
    {
        SceneManager.LoadScene("Welcome Screen");
    }

    void SubmitSurvey()
    {
        StartCoroutine(SendPsychData());
    }



    // Send the psychometric data to the server
    IEnumerator SendPsychData()
    {
        FlowData data = new FlowData();
        for (int i = 0; i < fss_Values.Length; i++)
        {
            switch (i)  
            {
                case 0: data.fss_1 = fss_Values[i]; break;
                case 1: data.fss_2 = fss_Values[i]; break;
                case 2: data.fss_3 = fss_Values[i]; break;
                case 3: data.fss_4 = fss_Values[i]; break;
                case 4: data.fss_5 = fss_Values[i]; break;
                case 5: data.fss_6 = fss_Values[i]; break;
                case 6: data.fss_7 = fss_Values[i]; break;
                case 7: data.fss_8 = fss_Values[i]; break;
            }
        }
        WWWForm form = new WWWForm();
        form.AddField("user_id", MainManager.Instance.user_id);
        form.AddField("total_games", MainManager.Instance.total_games);
        form.AddField("fss_1", data.fss_1);
        form.AddField("fss_2", data.fss_2);
        form.AddField("fss_3", data.fss_3);
        form.AddField("fss_4", data.fss_4);
        form.AddField("fss_5", data.fss_5);
        form.AddField("fss_6", data.fss_6);
        form.AddField("fss_7", data.fss_7);
        form.AddField("fss_8", data.fss_8);
        int maxRetries = 3; // Maximum number of retries
        int retryDelay = 2; // Delay between retries in seconds
        int attempt = 0; // Current attempt counter
        while (attempt < maxRetries)
        {
            UnityWebRequest www = UnityWebRequest.Post(SERVERSCRIPT, form);
            Debug.Log($"Sending data to {SERVERSCRIPT} with form {form}");
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            if (www.result == UnityWebRequest.Result.Success)
            {
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(www.downloadHandler.text);
                if (response.success)
                {
                    Debug.Log("Data sent successfully");
                    // Enable the game button after data is sent successfully
                    submitButton.interactable = false;
                    foreach (Button[] buttons in fss_ButtonsArray)
                    {
                        foreach (Button button in buttons)
                        {
                            button.interactable = false;
                        }
                    }
                    submitButton.GetComponentInChildren<Text>().text = "Data Sent";
                    submitButton.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);
                    gameButton.interactable = true;
                    exitButton.interactable = true;
                    submitted = true;
                    yield break; // Exit the coroutine successfully
                }
                else
                {
                    Debug.Log($"Failed to send data: {response.message}"); // Log the error message
                    StartCoroutine(ShowError(response.message)); // Show the error message
                    yield break; // Exit the coroutine
                }
            }
            else
            {
                Debug.Log($"Attempt {attempt + 1} failed: {www.error}"); // Log the error
                attempt++;
                if (attempt < maxRetries)
                {
                    yield return new WaitForSeconds(retryDelay); // Wait before retrying
                }
            }
        }
        // If all attempts fail, notify the user
        StartCoroutine(ShowError("Failed to send data after multiple attempts."));
        Debug.Log("Failed to send data after multiple attempts.");

    }
    // Show error message for 3 seconds
    IEnumerator ShowError(string message)
    {
        infoMsg.text = message;
        yield return new WaitForSeconds(3);
        infoMsg.text = "";
    }

}


[System.Serializable]
public class FlowData // Flow Short Scale, 8 items
{
    public int fss_1;
    public int fss_2;
    public int fss_3;
    public int fss_4;
    public int fss_5;
    public int fss_6;
    public int fss_7;
    public int fss_8;
}