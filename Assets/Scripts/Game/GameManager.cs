using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using System.Numerics;
// TODO: Implement game identifiersss
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; } // I dont understand this one and the Awake start
    public Ghost[] ghosts;
    
    public Pacman pacman;
    
    public Transform pellets;

    public DataCollector gameDatacollector;

    public Text Gameover;
    public Text ScoreText;
    public Text livesText;

    public Text restartKey;

    public int ghostMultiplier { get; private set; } = 1;

    public int score {get ; private set; }

    public int remainingPellets {get ; private set; }

    public int remainingPills {get ; private set; }

    public int[] PowerPelletStates;  // for elements, 1 = not eaten, 0 = eaten . This is to keep track of the powerpellets. 
    //They are numbered cloclwise by their location in the grid first one is the one top left, second one is the one top right, third one is the one bottom right and fourth one is the one bottom left
    
    public int lives {get ; private set; }
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
    private void NewGame()
    {
        SetScore(0);
        SetLives(3);

        NewRound();
        

    }

    private void NewRound()
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
        for (int i = 0; i < this.ghosts.Length; i++) { // reset all ghosts
            this.ghosts[i].ResetState();
        }
        this.pacman.ResetState(); // reset pacman
        gameDatacollector.Startdatacollection();
        StartTimer();
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

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        AudioManager.Instance.PlayGhostEatenSound();
        SetScore(this.score + points);
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
        AudioManager.Instance.PlayEatingSound();
        if (remainingPellets == 0){
            win = true;
            this.pacman.gameObject.SetActive(false);
            gameDatacollector.SaveData();
            Invoke(nameof(NewRound), 3.0f);
             // TODO: change this to a game win screen
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