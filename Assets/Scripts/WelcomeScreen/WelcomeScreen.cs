using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;
using JetBrains.Annotations;

// This script manages the welcome screen of the game. It initializes UI elements and handles user interactions
// such as starting the game, navigating to consent and survey forms, and updating game data. The script retrieves
// user-specific game data from a server using the user's ID and displays relevant information like username, session
// number, and game number. It also manages the state of consent and survey forms, ensuring that the user completes
// necessary steps before proceeding. The script uses Unity's UI system and networking capabilities to achieve these tasks.

public class WelcomeScreen : MonoBehaviour
{

    public Button GameButton;
    public Button goToConsentButton;

    public int consent; // State of consent form 0 = not done, 1 = unverified, 2 = verified
    public int survey; // State of survey form 0 = not done, 1 = unverified, 2 = verified
    public string consentUrl;
    public string surveyUrl;

    public GameObject StudyIntro;
    public GameObject Instructions;
    
    public Text errorMsg;

    public Text username;
    public Text session_number;
    public Text game_number;
    public Link linkOpener;
    public PressHandler pressHandler;
    
    void Start()
    {
        StartCoroutine(GetGameData(MainManager.Instance.user_id.ToString(), newsession: !MainManager.Instance.already_played)); // Get game data from the server, do not increment session number if the user has already played a game in the current session
        GameButton.onClick.AddListener(LoadGame);
        // goToConsentButton.onClick.AddListener(moveToConsent);
        pressHandler.OnPress.AddListener(moveToConsent);
        errorMsg.text = "";
        username.text = "Username: " + MainManager.Instance.username;

        if (consent != 2)
        {
            InvokeRepeating(nameof(UpdateGameData), 2, 5);
        }
    }

    IEnumerator GetGameData(string userId, bool newsession = true)
    {
        string url = $"{MainManager.Instance.dataserver + "SQL/getgamedata.php"}?user_id={userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            StartCoroutine(ShowError("Error" + request.error));
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            ProcessGameData(request.downloadHandler.text, newsession);
        }
    }

    void ProcessGameData(string jsonData, bool newsession) 
    {
        var data = JsonUtility.FromJson<GameData>(jsonData);
        MainManager.Instance.total_games = data.total_games;
        MainManager.Instance.session_number = data.last_session;
        consent = data.consent_done;
        if (consent == 2)
        {
            CancelInvoke(nameof(UpdateGameData));
        }
        survey = data.survey_done;
        consentUrl = data.consent_link;
        surveyUrl = data.survey_link;
        linkOpener.Field.text = consentUrl;
        if (newsession)
        {
            MainManager.Instance.session_number += 1;
        }
        session_number.text = "Session number: " + MainManager.Instance.session_number.ToString();
        game_number.text = "Games played: "+ MainManager.Instance.total_games.ToString();
        SetButtons();
    }

    void UpdateGameData()
    {
        Debug.Log("Updating user data");
        StartCoroutine(GetGameData(MainManager.Instance.user_id.ToString()));
        if (consent == 2)
        {
            CancelInvoke(nameof(UpdateGameData));
        }
    }

    public void SetButtons()
    {
        if (consent == 2)
        {
            GameButton.gameObject.SetActive(true);
            GameButton.interactable = true;
            GameButton.GetComponentInChildren<Text>().text = "Click here to play";
            goToConsentButton.gameObject.SetActive(false);
            StudyIntro.SetActive(false);
            Instructions.SetActive(true);
        }
        if (consent != 2){
            GameButton.interactable = false;
            GameButton.gameObject.SetActive(false);
            goToConsentButton.gameObject.SetActive(true);
            goToConsentButton.interactable = true;
            StudyIntro.SetActive(true);
            Instructions.SetActive(false);
        }
    }

    void moveToConsent()
    {
        StartCoroutine(ShowError("Opening external form, check pop-up window", 10));
        #if !UNITY_EDITOR
            linkOpener.OpenLinkJSPlugin();
        #else
            linkOpener.OpenLink();
        #endif
    }

    void LoadGame()
    {
        SceneManager.LoadScene("Pacman");
    }

    // Show error message for 3 seconds
    IEnumerator ShowError(string message, int time = 3)
    {
        errorMsg.text = message;
        yield return new WaitForSeconds(time);
        errorMsg.text = "";
    }

}

[System.Serializable]
public class GameData
{
    public int total_games;
    public int last_session;

    public int survey_done;
    public int consent_done;
    public string survey_link;
    public string consent_link;
}