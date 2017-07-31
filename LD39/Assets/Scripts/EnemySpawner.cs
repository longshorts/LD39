using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    GameObject enemyPrefab;
    Transform enemyContainer;
    public float spawnTimer = 2f;
    public int enemiesToSpawn = 50;
    public int enemiesSpawned = 0;
    private float currentTimer = 0f;

	// Use this for initialization
	void Start () {
        enemyPrefab = Resources.Load("Prefabs/Enemy") as GameObject;
        enemyContainer = GameObject.Find("Enemy_Container").transform;
        enemiesSpawned = 0;
    }
	
	// Update is called once per frame
	void Update () {
        currentTimer += Time.deltaTime;

        if(currentTimer > spawnTimer && enemiesSpawned < enemiesToSpawn)
        {
            //Create an enemy
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, enemyContainer);
            newEnemy.transform.position = transform.position;
            currentTimer = 0;
            enemiesSpawned++;
        }
	}
}
