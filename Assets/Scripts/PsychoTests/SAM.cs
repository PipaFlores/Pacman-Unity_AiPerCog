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

    public Button gameButton; // Button to end survey and go back to game

    private string SERVERSCRIPT = "SQL/SAM.php"; // Path to the server script appended to the server URL
    
    public TMP_Text errorMsg; // Text field to display error messages
   
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

    // Start is called before the first frame update
    void Start()
    {
        gameButton.onClick.AddListener(BackToGame);
        SERVERSCRIPT = MainManager.Instance.dataserver + SERVERSCRIPT;
        errorMsg.text = "";
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
        currentButton.GetComponent<Image>().color = Color.green;

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
        currentButton.GetComponent<Image>().color = Color.green;

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
        currentButton.GetComponent<Image>().color = Color.green;

        // Update the previous button
        previousDominanceButton = currentButton;
    }


    void BackToGame()
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
        data.TimeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        WWWForm form = new WWWForm();
        form.AddField("user_id", MainManager.Instance.user_id);
        form.AddField("game_number", MainManager.Instance.game_number);
        form.AddField("Valence", data.Valence);
        form.AddField("Arousal", data.Arousal);
        form.AddField("Dominance", data.Dominance);
        form.AddField("TimeStamp", data.TimeStamp);
        int maxRetries = 3; // Maximum number of retries
        int retryDelay = 2; // Delay between retries in seconds
        int attempt = 0; // Current attempt counter
        while (attempt < maxRetries)
        {
            UnityWebRequest www = UnityWebRequest.Post(SERVERSCRIPT, form);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data sent successfully");
                SceneManager.LoadScene("Pacman"); // Return to the game
                yield break; // Exit the coroutine successfully
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
        errorMsg.text = message;
        yield return new WaitForSeconds(3);
        errorMsg.text = "";
    }

}


[System.Serializable]
public class PsychData
{
    public int Valence;
    public int Arousal;

    public int Dominance;
    public string TimeStamp;
}