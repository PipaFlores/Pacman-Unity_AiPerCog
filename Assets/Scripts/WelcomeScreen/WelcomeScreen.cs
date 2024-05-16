using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;

public class WelcomeScreen : MonoBehaviour
{

    public Button GameButton;
    public Button goToConsentButton;
    public Button goToSurveyButton;
    
    public TMP_Text errorMsg;

    public TMP_Text username;
    public TMP_Text session_number;
    public TMP_Text game_number;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetGameData(MainManager.Instance.user_id.ToString()));

        GameButton.onClick.AddListener(loadGame);
        goToConsentButton.onClick.AddListener(moveToConsent);
        goToSurveyButton.onClick.AddListener(moveToSurvey);
        errorMsg.text = "";
        username.text = "Username: " + MainManager.Instance.username;
        

    }

    IEnumerator GetGameData(string userId)
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
            ProcessGameData(request.downloadHandler.text);
        }
    }

    void ProcessGameData(string jsonData) // TODO implement this
    {
        var data = JsonUtility.FromJson<GameData>(jsonData);
        MainManager.Instance.game_number = data.total_games;
        MainManager.Instance.session_number = data.last_session + 1;
        session_number.text = "Session number: " + MainManager.Instance.session_number.ToString();
        game_number.text = "Games played: "+ MainManager.Instance.game_number.ToString();
    }


    // FIXME modify the consent and survey functions to move to the correct information
    void moveToConsent()
    {
        SceneManager.LoadScene("Consent");
    }
    void moveToSurvey()
    {
        SceneManager.LoadScene("Survey");
    }

    void loadGame()
    {
        SceneManager.LoadScene("Pacman");
    }
}

[System.Serializable]
public class GameData
{
    public int total_games;
    public int last_session;
}