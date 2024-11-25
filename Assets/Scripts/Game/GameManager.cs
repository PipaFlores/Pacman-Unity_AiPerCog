using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Data.Common;
using Unity.VisualScripting;
// The GameManager class is responsible for managing the overall game state and logic.
// It handles the initialization and control of various game elements such as Pacman, ghosts, pellets, and cherries.
// The class also manages the level progression, including speed multipliers and other parameters for each level.
// Additionally, it interacts with the DataCollector class to gather and manage game data during each round.
// The collected data includes player and ghost positions, game state, scores, and other relevant information.
// This data can be saved locally or sent to a remote server for analysis.
// The GameManager class ensures that the game runs smoothly and provides a framework for game progression and data collection.

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    public Ghost[] ghosts;
    
    public Pacman pacman;
    
    public Transform pellets;

    public Cherry cherry;

    public DataCollector gameDatacollector;

    // level progression. 
    // rows = levels, columns = variables (pacmanSpeedMultiplier, ghostSpeedMultiplier, frightenedPacmanSpeedMultiplier, frightenedGhostSpeedMultiplier, frightenedDuration, fruitPoints, homeTimerRatio) 
    private float[,] levelData = new float[,]
     {
        // pacmanSpeedMultiplier, ghostSpeedMultiplier, frightenedPacmanSpeedMultiplier, frightenedGhostSpeedMultiplier, frightenedDuration, FruitPoints, FruitSymbol, homeTimerRatio
        { 0.8f, 0.75f, 0.9f, 0.5f, 6.0f , 100.0f, 0, 1},   // Level 1
        { 0.9f, 0.85f, 0.95f, 0.55f, 5.0f, 300.0f, 1, 1 }, // Level 2
        { 0.9f, 0.85f, 0.95f, 0.55f, 4.0f, 500.0f, 2, 0.9f}, // Level 3
        { 0.9f, 0.85f, 0.95f, 0.55f, 3.0f, 500.0f, 2, 0.9f}, // Level 4
        { 1.0f, 0.95f, 1.0f, 0.6f, 2.0f, 700.0f, 3, 0.8f},   // Level 5
        { 1.0f, 0.95f, 1.0f, 0.6f, 5.0f, 700.0f, 3, 0.8f},   // Level 6
        { 1.0f, 0.95f, 1.0f, 0.6f, 2.0f, 1000.0f, 4, 0.7f},   // Level 7
        { 1.0f, 0.95f, 1.0f, 0.6f, 2.0f, 1000.0f, 4, 0.6f },   // Level 8
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 2000.0f, 5, 0.5f },   // Level 9
        { 1.0f, 0.95f, 1.0f, 0.6f, 5.0f, 2000.0f, 5, 0.4f },   // Level 10
        { 1.0f, 0.95f, 1.0f, 0.6f, 2.0f, 3000.0f, 6, 0.3f },   // Level 11
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 3000.0f, 6, 0.2f },   // Level 12
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 13
        { 1.0f, 0.95f, 1.0f, 0.6f, 3.0f, 5000.0f, 7, 0.1f },   // Level 14
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 15
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 16
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 17
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 18
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 19
        { 1.0f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f },   // Level 20
        { 0.9f, 0.95f, 1.0f, 0.6f, 1.0f, 5000.0f, 7, 0.1f }    // Level 21+
    };


      private readonly Dictionary<string, int> columnIndices = new Dictionary<string, int>
    {
        { "pacmanSpeedMultiplier", 0 },
        { "ghostSpeedMultiplier", 1 },
        { "frightenedPacmanSpeedMultiplier", 2 },
        { "frightenedGhostSpeedMultiplier", 3 },
        { "frightenedDuration", 4},
        { "fruitPoints", 5 },
        { "fruitSymbol", 6 },  
        { "homeTimerRatio", 7}      
        // Add more columns as needed
    };

    public Text Gameover;
    public Text ScoreText;
    public Text livesText;

    public GameObject livesIndicator;

    public Text levelText;

    public Text restartKey;
    public Text escapeKey;
    public Text readyText;

    public Text UserNotification;
    public int ghostMultiplier { get; private set; } = 1;

    public int score {get ; private set; }

    private int previous_score = 0;

    public int remainingPellets {get ; private set; }

    public int remainingPills {get ; private set; }

    public int fruitState_1; // 0 = unactive, 1 = active, 2 = eaten
    public int fruitState_2;  // 0 = unactive, 1 = active, 2 = eaten

    public int[] PowerPelletStates;  // for elements, 1 = not eaten, 0 = eaten . This is to keep track of the powerpellets. 
    //They are numbered clocklwise by their location in the grid first one is the one top left, second one is the one top right, third one is the one bottom right and fourth one is the one bottom left
    
    public int lives {get ; private set; }
    public int level {get ; private set; }
    public int startLevel = 1 ; // For debugging purposes
    public float round_timeElapsed {get ; private set; }
    public float round_startTime {get ; private set; }
    public bool win {get ; private set; }

    // Survey controll variable
    public int games_threshold = 2; // Number of games to play before starting survey iterations
    public int n_games = 1; // Number of games to play between surveys


    

    //// Singleton pattern to ensure only one instance of GameManager exists (not used, as loading the survey and restarting the game corrups gamemanager references to other objects)
    // private void Awake()
    // {
    //     if (Instance != null) {
    //         DestroyImmediate(gameObject);
    //     } else {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    // }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (MainManager.Instance.already_played == false){
            MainManager.Instance.already_played = true;
        }
        NewGame();

    }

    private void Update()
    {
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Space) && restartKey.enabled == true){
            NewGame(); 
        }
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Escape) && escapeKey.enabled == true){
            SceneManager.LoadScene("Welcome Screen");
        }
        // Debugging
        if (MainManager.Instance.debugging){
            if (Input.GetKeyDown(KeyCode.F4)){
                StartCoroutine(AllLivesLost());
            }
            // Debugging death
            if (Input.GetKeyDown(KeyCode.F1)){
                this.lives = 1;
                PacmanEaten();
            }
            // Debugging win
            if (Input.GetKeyDown(KeyCode.F2)){
                foreach (Transform pellet in this.pellets){
                    PelletEaten(pellet.GetComponent<Pellet>());
                }
            }
            if (Input.GetKeyDown(KeyCode.F3)){
                this.SetScore(this.score + 1400);
            }
        }




        // Update timer for data gathering
        round_timeElapsed = Time.time - round_startTime;
    }
    private void NewGame() // Starts a new game from the starting level
    {
        SetScore(0);
        SetLives(3);
        SetLevel(startLevel);

        NewRound();
        

    }

    private void NewRound() // Starts a new level
    {
        MainManager.Instance.games_in_session++; // Increase the count of games in a session
        MainManager.Instance.total_games++; // Increase the count of total games played
        win = false;
        Gameover.enabled = false;
        restartKey.enabled = false;
        escapeKey.enabled = false;
        foreach (Transform pellet in this.pellets) // reset all pellets 
        {
            pellet.gameObject.SetActive(true);
        }
        remainingPellets = CountRemainingPellets();
        remainingPills = CountRemainingPowerPellets();
        PowerPelletStatesInit();  
        loadLevelData();      
        for (int i = 0; i < this.ghosts.Length; i++) { // reset all ghosts
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState(); // reset pacman
        //  freeze the game for 3 seconds before each level start
        StartCoroutine(GetReady(3.0f));
        
    }

    // Freeze the game for 3 seconds before each level start
    // Start the data collection and timer if startDataCollection is true
    // Set the time scale to 1 to resume the game (the time freeze keeps the data collection from being too noisy)
    private IEnumerator GetReady(float time, bool startDataCollection = true)
    {
        this.readyText.enabled = true;
        Time.timeScale = 0;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        int countdown = (int)time;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            this.readyText.text = "READY! " + countdown.ToString();
            countdown--;
            yield return new WaitForSecondsRealtime(1);
        }
        this.readyText.enabled = false;
        if (startDataCollection)
        {
            gameDatacollector.Startdatacollection();
            StartTimer();
        }
        Time.timeScale = 1;
    }

    // If pacman dies, resets ghots and pacman but not pellet
    private void ResetState()  
    {
       ResetGhostMultiplier();

       for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState();
        StartCoroutine(GetReady(3.0f, false));
    }

    private void LoadSurvey()
    {
        // Load the survey scene
        SceneManager.LoadScene("PsychState");
    }

    private void PromptRestart()
    {
        restartKey.enabled = true;
        escapeKey.enabled = true;
    }

    // Set the score and update the score text
    private void SetScore(int score)
    {
        this.score = score;
        ScoreText.text = score.ToString().PadLeft(2, '0');
        if (previous_score / 10000 != score / 10000){
            this.SetLives(this.lives + 1);
            AudioManager.Instance.PlayExtraLifeSound();
            StartCoroutine(ErrorMsg("Extra life!"));
        }
        previous_score = score;
    }

    // Set the lives and update the lives text
    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    // Set the level and update the level text
    private void SetLevel(int level)
    {
        this.level = level;
        levelText.text = "Level " + level.ToString();
    
    }

    // Load the level data from the levelData array
    private void loadLevelData()
    {
        float[] levelVariables = new float[levelData.GetLength(1)];
        for (int i = 0; i < levelData.GetLength(1); i++)
        {
            levelVariables[i] = levelData[this.level - 1, i];
        }
        this.pacman.movement.normalSpeedMultiplier = levelVariables[columnIndices["pacmanSpeedMultiplier"]];
        this.pacman.movement.frightenedSpeedMultiplier = levelVariables[columnIndices["frightenedPacmanSpeedMultiplier"]];
        this.cherry.points = (int)levelVariables[columnIndices["fruitPoints"]];
        this.cherry.SetSprite((int)levelVariables[columnIndices["fruitSymbol"]]);
        foreach (Transform pellet in pellets){
            if (pellet.gameObject.GetComponent<PowerPellet>() != null){
                pellet.gameObject.GetComponent<PowerPellet>().duration = levelVariables[columnIndices["frightenedDuration"]];
            }
        }
        foreach (Ghost ghost in ghosts){
            ghost.movement.normalSpeedMultiplier = levelVariables[columnIndices["ghostSpeedMultiplier"]];
            ghost.movement.frightenedSpeedMultiplier = levelVariables[columnIndices["frightenedGhostSpeedMultiplier"]];
            ghost.eatenDuration = ghost.eatenDuration * levelVariables[columnIndices["homeTimerRatio"]];
            if (ghost.gameObject.name != "Blinky")
            {
                ghost.SetGhostBehavior(levelVariables[columnIndices["homeTimerRatio"]]);
            }
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        AudioManager.Instance.PlayGhostEatenSound();
        SetScore(this.score + points);
        ghost.InstantiateFloatingPoint(points);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            ResetState(); // If pacman dies, resets ghots and pacman but not pellet (3 seconds delay)
            AudioManager.Instance.PlayDeathSound();
            this.livesIndicator.GetComponentInChildren<AnimatedSprite>().PacmanDeathAnimation();
        }
        else
        {
            AudioManager.Instance.PlayDeathSound();
            this.livesIndicator.GetComponentInChildren<AnimatedSprite>().PacmanDeathAnimation();
            for (int i = 0; i < this.ghosts.Length; i++) {
                this.ghosts[i].gameObject.SetActive(false);
            }
            this.pacman.gameObject.SetActive(false);
            Gameover.enabled = true; // Game over screen;
            StartCoroutine(AllLivesLost()); // Save data and wait for it to upload, then load survey or restart
        }
    }

    

    public void CherryEaten (Cherry cherry)
    {
        SetScore(this.score + cherry.points);
        if (cherry.cherryIndex == 1){
            fruitState_1 = 2;
        }
        if (cherry.cherryIndex == 2){
            fruitState_2 = 2;
        }
        cherry.gameObject.SetActive(false);
        cherry.InstantiateFloatingPoint(cherry.points);
    }

    public void PelletEaten(Pellet pellet) 
    {
        pellet.gameObject.SetActive(false);
        SetScore (this.score + pellet.points);
        remainingPellets = CountRemainingPellets();
        remainingPills = CountRemainingPowerPellets();
        if (remainingPellets == 174){
            this.cherry.gameObject.SetActive(true);
            this.cherry.cherryIndex = 1;
            fruitState_1 = 1;
        }

        if (remainingPellets == 74 && this.cherry.gameObject.activeSelf == false){
            this.cherry.gameObject.SetActive(true);
            this.cherry.cherryIndex = 2;
            fruitState_2 = 1;
        }

        AudioManager.Instance.PlayEatingSound();
        if (remainingPellets == 0){
            win = true;
            this.pacman.gameObject.SetActive(false);
            foreach (Ghost ghost in ghosts){
                ghost.gameObject.SetActive(false);
            }
            StartCoroutine(LevelComplete()); // Save data and wait for it to upload, then load next level
            

        }
    }

    public void PowerPelletEaten (PowerPellet pellet)
    {
        for (int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].frightened.Enable(pellet.duration);
            }
        PowerPelletEaten(pellet.GetPowerPelletIndex());
        PelletEaten(pellet);
        CancelInvoke(); // If you take more than one powerpellet, cancel the first invoke timer and start it again
        PacmanAttack();
        AudioManager.Instance.PlayIntermissionSound(pellet.duration);
        Invoke(nameof(PacmanAttackEnd), pellet.duration);
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);        
    }

    // change pacman state to attack for the duration of the power pellet
    public void PacmanAttack(){
        pacman.pacmanAttack = true;
    }
    public void PacmanAttackEnd(){
        this.pacman.pacmanAttack = false;
    }

    // Count the remaining pellets in the game
    private int CountRemainingPellets()
    {
        int count = 0;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<Pellet>() != null)
            {
                count++;
            }
        }
        return count;
    }

    // Count the remaining power pellets in the game
    private int CountRemainingPowerPellets()
    {
        int count = 0;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf && pellet.GetComponent<PowerPellet>() != null)
            {
                count++;
            }
        }
        return count;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

    // Start the timer for the round
    public void StartTimer(){
        round_startTime = Time.time;
        }

    float RoundToNearestHalf(float value)
    {
        return Mathf.Round(value * 2f) / 2f;
    }

    // Initialize the power pellet states
    public void PowerPelletStatesInit()
    {
        PowerPelletStates = new int[4];
        for (int i = 0; i < 4; i++)
        {
            PowerPelletStates[i] = 1;
        }
    }

    // Set the power pellet state to eaten
    public void PowerPelletEaten(int i)
    {
        PowerPelletStates[i] = 0;
    }

    public IEnumerator ErrorMsg(string msg){
        UserNotification.text = msg;
        yield return new WaitForSeconds(1.5f);
        UserNotification.text = "";
    }

    private IEnumerator LevelComplete()
    {
        yield return StartCoroutine(gameDatacollector.SaveData());
        SetLevel(this.level + 1);
        UserNotification.text = "Loading next level...";
        yield return new WaitForSeconds(1.5f);
        UserNotification.text = "";
        NewRound();
    }

    private IEnumerator AllLivesLost()
    {
        yield return StartCoroutine(gameDatacollector.SaveData());
        if (gameDatacollector.data_upload_success){
            if (MainManager.Instance.games_in_session % n_games == 0 && MainManager.Instance.total_games >= games_threshold){
                UserNotification.text = "Loading survey...";
                yield return new WaitForSeconds(1.5f);
                UserNotification.text = "";
                LoadSurvey();
            } else {
                yield return new WaitForSeconds(1.5f);
                PromptRestart();
            }
        } else {
            UserNotification.text = "Returning to main menu...";
            yield return new WaitForSeconds(1.5f);
            UserNotification.text = "";
            SceneManager.LoadScene("WelcomeScreen");
        }
    }


}