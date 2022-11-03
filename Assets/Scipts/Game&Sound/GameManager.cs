using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance = null;

    bool isGameOver;
    bool playerReady;
    bool initReadyScreen;

    int Score;

    float gameRestartTime;
    float gamePlayerReadyTime;

    public float gameRestartDelay = 5f;
    public float gamePlayerReadyDelay = 3f;

    TextMeshProUGUI ScoreText;
    TextMeshProUGUI screenMessageText;

    // Initialize the singleton instance
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (playerReady)
        {
            if (initReadyScreen)
            {
                FreezePlayer(true);
                FreezeEnemies(true);
                screenMessageText.alignment = TextAlignmentOptions.Center;
                screenMessageText.alignment = TextAlignmentOptions.Top;
                screenMessageText.fontStyle = FontStyles.UpperCase;
                screenMessageText.fontSize = 24;
                screenMessageText.text = "\n\n\n\nREADY";
                initReadyScreen = false;
            }
            gamePlayerReadyTime -= Time.deltaTime;
            if (gamePlayerReadyTime < 0)
            {
                FreezePlayer(false);
                FreezeEnemies(false);
                screenMessageText.text = "";
                playerReady = false;
            }
            return;
        }

        if (ScoreText != null)
        {
            ScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>", ScoreText.fontSize, Score);
        }
        //Reset enemies and time
        if (!isGameOver)
        {
            RepositionEnemies();
        }
        else
        {
            gameRestartTime -= Time.deltaTime;
            if (gameRestartTime < 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    //Load and unload scenes
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // called second
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
    }
    //On start display score and ready message
    private void StartGame()
    {
        isGameOver = false;
        playerReady = true;
        initReadyScreen = true;
        gamePlayerReadyTime = gamePlayerReadyDelay;
        ScoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();
        SoundManager.Instance.MusicSource.Play();
    }
    //player score
    public void AddScorePoints(int points)
    {
        Score += points;
    }
    //Stopping player input
    private void FreezePlayer(bool freeze)
    {
        GameObject MegaMan = GameObject.FindGameObjectWithTag("MegaMan");
        if (MegaMan != null)
        {
            MegaMan.GetComponent<MegaMan>().FreezeInput(freeze);
            MegaMan.GetComponent<MegaMan>().FreezePlayer(freeze);
        }
    }
    //Stopping enemies
    private void FreezeEnemies(bool freeze)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            //enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }
    //Stopping all of the players bullets
    private void FreezeBullets(bool freeze)
    {
        GameObject[] MMBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject MMBullet in MMBullets)
        {
            MMBullet.GetComponent<MMBullet>().FreezeBullet(freeze);
        }
    }
    //Freeze player and destroy objects to set off animations
    public void PlayerDefeated()
    {
        isGameOver = true;
        gameRestartTime = gameRestartDelay;
        SoundManager.Instance.Stop();
        SoundManager.Instance.StopMusic();
        FreezePlayer(true);
        FreezeEnemies(true);
        GameObject[] MMBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject MMBullet in MMBullets)
        {
            Destroy(MMBullet);
        }
        GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");
        foreach (GameObject explosion in explosions)
        {
            Destroy(explosion);
        }
    }
    //Reset enemies
    private void RepositionEnemies()
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0));
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.transform.position.x < worldLeft.x)
            {
                switch (enemy.name)
                {
                    case "Bullet":
                        enemy.transform.position = new Vector3(worldRight.x, UnityEngine.Random.Range(-1.5f, 1.5f), 0);
                        enemy.GetComponent<Bullet>().ResetFollowingPath();
                        break;
                }
            }
        }
    }
}