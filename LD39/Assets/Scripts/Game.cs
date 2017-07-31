using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public enum GameState
    {
        PLAY, GAMEOVER, VICTORY
    }

    public int BaseHealth = 20;
    public int EnemiesKilled = 0;
    public GameState state;

    public GameObject gameOverScreen;
    public Text baseHealthText;
    public Text gameOverInformation;
    public Text enemiesRemaining;

    public float shotEnergyMultiplier = 1f;

    public static Game instance;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        if(instance == null)
        {
            instance = this;
        } else
        {
            DestroyObject(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        state = GameState.PLAY;
	}
	
	// Update is called once per frame
	void Update () {
        if(baseHealthText == null)
        {
            baseHealthText = GameObject.Find("BaseHealthText").GetComponent<Text>();
        }
        if(gameOverInformation == null)
        {
            gameOverInformation = GameObject.Find("Canvas").transform.GetChild(0).GetChild(1).GetComponent<Text>();
        }
        if(enemiesRemaining == null)
        {
            enemiesRemaining = GameObject.Find("EnemiesRemaining").GetComponentInChildren<Text>();
        }

        baseHealthText.text = "Base Health: " + BaseHealth;

        int totalEnemies = 0;
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        for (int i = 0; i < spawners.Length; i++)
        {
            totalEnemies += spawners[i].enemiesToSpawn;
        }


        int remaining = totalEnemies - EnemiesKilled + (BaseHealth - 20);
        enemiesRemaining.text = "Enemies Remaining: " + remaining;
    }

    public void EnemyKilled()
    {
        int totalEnemies = 0;
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        for (int i = 0; i < spawners.Length; i++)
        {
            totalEnemies += spawners[i].enemiesToSpawn;
        }


        int remaining = totalEnemies - EnemiesKilled + (BaseHealth - 20);
        enemiesRemaining.text = "Enemies Remaining: " + remaining;

        if (EnemiesKilled + (20 - BaseHealth) >= totalEnemies)
        {
            Debug.Log("ENEMIESKILLED: " + EnemiesKilled);
            Debug.Log("BASEHEALTH: " + BaseHealth);
            Debug.Log("TOTALENEMIES: " + totalEnemies);
            Debug.Log("Spawners: " + spawners.Length);
            state = GameState.VICTORY;
            DoVictory();
        }
        else
        {
            state = GameState.PLAY;
        }
    }

    public void DoVictory()
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ModifyBaseHealth(int modifier)
    {
        BaseHealth += modifier;
        if(BaseHealth <= 0)
        {
            state = GameState.GAMEOVER;
            gameOverInformation.text = "Boats Destroyed: " + EnemiesKilled;
            GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void Reload()
    {
        BaseHealth = 20;
        EnemiesKilled = 0;
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);

        state = GameState.PLAY;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReloadHarder()
    {
        BaseHealth = 20;
        EnemiesKilled = 0;
        shotEnergyMultiplier *= 2;
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);

        state = GameState.PLAY;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
