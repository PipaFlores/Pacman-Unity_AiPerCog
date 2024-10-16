using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;

// This script manages the welcome screen of the game. It initializes UI elements and handles user interactions
// such as starting the game, navigating to consent and survey forms, and updating game data. The script retrieves
// user-specific game data from a server using the user's ID and displays relevant information like username, session
// number, and game number. It also manages the state of consent and survey forms, ensuring that the user completes
// necessary steps before proceeding. The script uses Unity's UI system and networking capabilities to achieve these tasks.

public class WelcomeScreen : MonoBehaviour
{

    public Button GameButton;
    public Button goToConsentButton;
    public Button goToSurveyButton;
    public Button updateButton;

    public int consent; // State of consent form 0 = not done, 1 = unverified, 2 = verified
    public int survey; // State of survey form 0 = not done, 1 = unverified, 2 = verified
    public string consentUrl;
    public string surveyUrl;
    
    public TMP_Text errorMsg;

    public TMP_Text username;
    public TMP_Text session_number;
    public TMP_Text game_number;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetGameData(MainManager.Instance.user_id.ToString()));
        GameButton.onClick.AddListener(LoadGame);
        goToConsentButton.onClick.AddListener(moveToConsent);
        goToSurveyButton.onClick.AddListener(moveToSurvey);
        updateButton.onClick.AddListener(UpdateGameData);
        errorMsg.text = "";
        username.text = "Username: " + MainManager.Instance.username;
        
    }

    IEnumerator GetGameData(string userId, bool newsession = true)
    {
        string url = $"{MainManager.Instance.dataserver + "SQL/getgamedata.php"}?user_id={userId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            ProcessGameData(request.downloadHandler.text, newsession);
        }
    }

    void ProcessGameData(string jsonData, bool newsession) // TODO implement this
    {
        var data = JsonUtility.FromJson<GameData>(jsonData);
        MainManager.Instance.game_number = data.total_games;
        MainManager.Instance.session_number = data.last_session;
        consent = data.consent_done;
        survey = data.survey_done;
        consentUrl = data.consent_link;
        surveyUrl = data.survey_link;
        if (newsession)
        {
            MainManager.Instance.session_number += 1;
        }
        session_number.text = "Session number: " + MainManager.Instance.session_number.ToString();
        game_number.text = "Games played: "+ MainManager.Instance.game_number.ToString();
        SetButtons();
    }

    void UpdateGameData()
    {
        StartCoroutine(GetGameData(MainManager.Instance.user_id.ToString(), newsession:true));
    }

    public void SetButtons()
    {
        if (consent == 2)
        {
            GameButton.interactable = true;
            GameButton.GetComponentInChildren<Text>().text = "Play Game";
            goToSurveyButton.interactable = true;
            goToSurveyButton.GetComponentInChildren<Text>().text = "Take Survey";
            goToConsentButton.interactable = false;
            updateButton.gameObject.SetActive(false);
            goToConsentButton.GetComponentInChildren<Text>().text = "Consent form completed";
        }
        if (survey == 2)
        {
            goToSurveyButton.interactable = false;
            goToSurveyButton.GetComponentInChildren<Text>().text = "Survey completed";
        }
        if (consent != 2){
            GameButton.interactable = false;
            GameButton.GetComponentInChildren<Text>().text = "Complete consent to Play";
            goToSurveyButton.interactable = false;
            goToSurveyButton.GetComponentInChildren<Text>().text = "Complete consent to take survey";
            updateButton.enabled = true;
        }
    }

    void moveToConsent()
    {
        Application.OpenURL(consentUrl);
        // SceneManager.LoadScene("Consent");
    }
    void moveToSurvey()
    {
        if (consent != 2)
        {
            StartCoroutine(ShowError("Please complete the consent form first."));
            return;
        }
        
        Application.OpenURL(surveyUrl);
        // SceneManager.LoadScene("Survey");
    }

    void LoadGame()
    {
        SceneManager.LoadScene("Pacman");
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
public class GameData
{
    public int total_games;
    public int last_session;

    public int survey_done;
    public int consent_done;
    public string survey_link;
    public string consent_link;
}