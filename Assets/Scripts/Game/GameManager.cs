using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
//using System.Numerics;
// TODO: Implement game identifiersss
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } // I dont understand this one and the Awake start
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

    public Text levelText;

    public Text restartKey;
    public Text readyText;

    public int ghostMultiplier { get; private set; } = 1;

    public int score {get ; private set; }

    public int remainingPellets {get ; private set; }

    public int remainingPills {get ; private set; }

    public int[] PowerPelletStates;  // for elements, 1 = not eaten, 0 = eaten . This is to keep track of the powerpellets. 
    //They are numbered cloclwise by their location in the grid first one is the one top left, second one is the one top right, third one is the one bottom right and fourth one is the one bottom left
    
    public int lives {get ; private set; }
    public int level {get ; private set; }
    public int startLevel = 1 ; // For debugging purposes
    public float round_timeElapsed {get ; private set; }
    public float round_startTime {get ; private set; }
    public bool win {get ; private set; }
    // public Dictionary<Vector2, bool> pelletsPositions = new Dictionary<Vector2, bool>();
    


    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    
    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown){
            NewGame(); 
        }
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
        win = false;
        Gameover.enabled = false;
        restartKey.enabled = false;
        foreach (Transform pellet in this.pellets) // reset all pellets 
        {
            pellet.gameObject.SetActive(true);
            // Vector2 gridPosition = new Vector2(RoundToNearestHalf(pellet.position.x),RoundToNearestHalf(pellet.position.y));
            // pelletsPositions[gridPosition] = true;
        }
        // gameDatacollector.UpdatePellets(pelletsPositions);
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

    private IEnumerator GetReady(float time)
    {
        this.readyText.enabled = true;
        Time.timeScale = 0;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        this.readyText.enabled = false;
        gameDatacollector.Startdatacollection();
        StartTimer();
        Time.timeScale = 1;
    }

    private void ResetState()  // If pacman dies, resets ghots and pacman but not pellet
    {
       ResetGhostMultiplier();

       for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState(); 
    }

    private void GameOver()
    {
        for (int i = 0; i < this.ghosts.Length; i++) {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false); 
        // Game over screen
        Gameover.enabled = true;
        restartKey.enabled = true;
        gameDatacollector.SaveData();
    }

    private void SetScore(int score)
    {
        this.score = score;
        ScoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    private void SetLevel(int level)
    {
        this.level = level;
        levelText.text = "Level " + level.ToString();
    
    }

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
        // for (int i = 0; i < this.ghosts.Length; i++)
        // {
        //     this.ghosts[i].movement.normalSpeedMultiplier = levelVariables[columnIndices["ghostSpeedMultiplier"]];
        //     this.ghosts[i].movement.frightenedSpeedMultiplier = levelVariables[columnIndices["frightenedGhostSpeedMultiplier"]];
        //     //this.ghosts[i].chase.timer = levelVariables[columnIndices["ChaseTimer"]];
        // }
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
        AudioManager.Instance.PlayDeathSound();
        this.pacman.DeathSequence();


        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f); // If pacman dies, resets ghots and pacman but not pellet (3 seconds delay)
        }
        else
        {
            GameOver();
        }
    }

    public void CherryEaten (Cherry cherry)
    {
        SetScore(this.score + cherry.points);
        cherry.gameObject.SetActive(false);
        cherry.InstantiateFloatingPoint(cherry.points);
    }

    public void PelletEaten(Pellet pellet) // TODO track eaten pellets in pellet position list
    {
        // Vector2 pelletPosition = pellet.transform.position;
        // Vector2 gridPosition = new Vector2(RoundToNearestHalf(pelletPosition.x), RoundToNearestHalf(pelletPosition.y));
        // if (pelletsPositions.ContainsKey(gridPosition))
        // {
        //     pelletsPositions[gridPosition] = false; // Set to false indicating the pellet is eaten
        // }
        // gameDatacollector.UpdatePellets(pelletsPositions);
        pellet.gameObject.SetActive(false);
        SetScore (this.score + pellet.points);
        remainingPellets = CountRemainingPellets();
        remainingPills = CountRemainingPowerPellets();
        if (remainingPellets == 174){
            this.cherry.gameObject.SetActive(true);
        }

        if (remainingPellets == 74 && this.cherry.gameObject.activeSelf == false){
            this.cherry.gameObject.SetActive(true);
        }

        AudioManager.Instance.PlayEatingSound();
        if (remainingPellets == 0){
            win = true;
            this.pacman.gameObject.SetActive(false);
            foreach (Ghost ghost in ghosts){
                ghost.gameObject.SetActive(false);
            }
            gameDatacollector.SaveData();
            SetLevel(this.level + 1);
            Invoke(nameof(NewRound), 3.0f);

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


    public void StartTimer(){
        round_startTime = Time.time;
        }

    float RoundToNearestHalf(float value)
    {
        return Mathf.Round(value * 2f) / 2f;
    }

    public void PowerPelletStatesInit()
    {
        PowerPelletStates = new int[4];
        for (int i = 0; i < 4; i++)
        {
            PowerPelletStates[i] = 1;
        }
    }

    public void PowerPelletEaten(int i)
    {
        PowerPelletStates[i] = 0;
    }

}