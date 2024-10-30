using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;

// This scripts controls the Self-Assessment Manikin (SAM) survey, which is used to measure 
// the emotional response of the player after a defined period of gameplay.
public class SAM : MonoBehaviour
{

    public Button gameButton; // Button to go back to game
    public Button exitButton; // Button to go back to welcome screen
    public Button submitButton; // Button to submit the survey
    public Button infoButton; // Button to display info

    private string SERVERSCRIPT = "SQL/SAM.php"; // Path to the server script appended to the server URL
    
    public TMP_Text infoMsg; // Text field to display error messages


    public GameObject infoPanel; // Panel to display info
    public GameObject valenceButtonsParent; // Parent GameObject for valence buttons
    public GameObject arousalButtonsParent; // Parent GameObject for arousal buttons
    public GameObject dominanceButtonsParent; // Parent GameObject for dominance buttons

    // Arrays to store the valence, arousal, and dominance buttons
    private Button[] valenceButtons;
    private Button[] arousalButtons;
    private Button[] dominanceButtons;

    private int valenceValue = 0; // Current valence value
    private int arousalValue = 0; // Current arousal value
    private int dominanceValue = 0; // Current dominance value
    
    // Previous selected buttons for color change
    private Button previousValenceButton;
    private Button previousArousalButton;
    private Button previousDominanceButton;

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
        infoButton.onClick.AddListener(ShowInfo);
        SERVERSCRIPT = MainManager.Instance.dataserver + SERVERSCRIPT;
        infoMsg.text = "Rate your emotional response to the last game";
        // Initialize buttons
        valenceButtons = InitializeButtons(valenceButtonsParent);
        arousalButtons = InitializeButtons(arousalButtonsParent);
        dominanceButtons = InitializeButtons(dominanceButtonsParent);

        // Add listeners to valence buttons
        for (int i = 0; i < valenceButtons.Length; i++)
        {
            int index = i + 1; // Button names start from "1"
            valenceButtons[i].onClick.AddListener(() => OnValenceButtonClicked(index));
        }

        // Add listeners to arousal buttons
        for (int i = 0; i < arousalButtons.Length; i++)
        {
            int index = i + 1; // Button names start from "1"
            arousalButtons[i].onClick.AddListener(() => OnArousalButtonClicked(index));
        }

        // Add listeners to dominance buttons
        for (int i = 0; i < dominanceButtons.Length; i++)
        {
            int index = i + 1; // Button names start from "1"
            dominanceButtons[i].onClick.AddListener(() => OnDominanceButtonClicked(index));
        }
        
    }

    void Update()
    {
        if (valenceValue != 0 && arousalValue != 0 && dominanceValue != 0 && !submitted)
        {
            submitButton.interactable = true;
        }
    }   
    private Button[] InitializeButtons(GameObject parent)
    {
        Button[] buttons = new Button[5];
        for (int i = 0; i < buttons.Length; i++)
        {
            string buttonName = (i + 1).ToString(); // Button names are "1", "2", "3", etc.
            Transform buttonTransform = parent.transform.Find(buttonName);
            if (buttonTransform != null)
            {
                buttons[i] = buttonTransform.GetComponent<Button>();
            }
        }
        return buttons;
    }

    private void OnValenceButtonClicked(int value)
    {
        Debug.Log($"Valence button {value} clicked");
        valenceValue = value; // Set the valence value based on the button clicked

        // Reset the color of the previous button
        if (previousValenceButton != null)
        {
            previousValenceButton.GetComponent<Image>().color = Color.white;
        }

        // Set the color of the current button to green
        Button currentButton = valenceButtons[value - 1];
        currentButton.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);

        // Update the previous button
        previousValenceButton = currentButton;

    }

    private void OnArousalButtonClicked(int value)
    {
        Debug.Log($"Arousal button {value} clicked");
        arousalValue = value; // Set the arousal value based on the button clicked

        // Reset the color of the previous button
        if (previousArousalButton != null)
        {
            previousArousalButton.GetComponent<Image>().color = Color.white;
        }

        // Set the color of the current button to green
        Button currentButton = arousalButtons[value - 1];
        currentButton.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);

        // Update the previous button
        previousArousalButton = currentButton;
    }

    private void OnDominanceButtonClicked(int value)
    {
        Debug.Log($"Dominance button {value} clicked");
        dominanceValue = value; // Set the dominance value based on the button clicked

        // Reset the color of the previous button
        if (previousDominanceButton != null)
        {
            previousDominanceButton.GetComponent<Image>().color = Color.white;
        }

        // Set the color of the current button to green
        Button currentButton = dominanceButtons[value - 1];
        currentButton.GetComponent<Image>().color = new Color(0.5f, 1f, 0.5f);

        // Update the previous button
        previousDominanceButton = currentButton;
    }

    void BackToGame()
    {
        SceneManager.LoadScene("Pacman");
    }

    void BackToWelcome()
    {
        SceneManager.LoadScene("Welcome Screen");
    }

    // Show or hide the info panel
    void ShowInfo()
    {
        if (infoPanel.activeSelf)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }

    void SubmitSurvey()
    {
        StartCoroutine(SendPsychData());
    }



    // Send the psychometric data to the server
    IEnumerator SendPsychData()
    {
        PsychData data = new PsychData();
        data.Valence = valenceValue;
        data.Arousal = arousalValue;
        data.Dominance = dominanceValue;
        WWWForm form = new WWWForm();
        form.AddField("user_id", MainManager.Instance.user_id);
        form.AddField("total_games", MainManager.Instance.total_games);
        form.AddField("val", data.Valence);
        form.AddField("ar", data.Arousal);
        form.AddField("dom", data.Dominance);
        int maxRetries = 3; // Maximum number of retries
        int retryDelay = 2; // Delay between retries in seconds
        int attempt = 0; // Current attempt counter
        while (attempt < maxRetries)
        {
            UnityWebRequest www = UnityWebRequest.Post(SERVERSCRIPT, form);
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
                    foreach (Button button in valenceButtons)
                    {
                        button.interactable = false;
                    }
                    foreach (Button button in arousalButtons)
                    {
                        button.interactable = false;
                    }
                    foreach (Button button in dominanceButtons)
                    {
                        button.interactable = false;
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
public class PsychData
{
    public int Valence;
    public int Arousal;
    public int Dominance;
}