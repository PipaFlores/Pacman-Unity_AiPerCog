using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class WelcomeScreen : MonoBehaviour
{

    public Button GameButton;
    public Button goToConsentButton;
    public Button goToSurveyButton;
    
    public TMP_Text errorMsg;

    public TMP_Text username;
    public TMP_Text session_number;
    public TMP_Text game_number;
    ArrayList credentials;

    // Start is called before the first frame update
    void Start()
    {
        MainManager.Instance.game_number = GetGameNumber(); // TODO: Implement this
        MainManager.Instance.session_number = GetSessionNumber();

        GameButton.onClick.AddListener(loadGame);
        goToConsentButton.onClick.AddListener(moveToConsent);
        goToSurveyButton.onClick.AddListener(moveToSurvey);
        errorMsg.text = "";
        username.text = "Username: " + MainManager.Instance.username;
        session_number.text = "Session number: " + MainManager.Instance.session_number.ToString();
        game_number.text = "Games played: "+ MainManager.Instance.game_number.ToString();

    }

    int GetGameNumber() // TODO implement this
    {
        return MainManager.Instance.game_number;
    }

    int GetSessionNumber()
    {
        return MainManager.Instance.session_number;
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

