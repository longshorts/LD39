using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    GameObject enemyPrefab;
    Transform enemyContainer;
    public float spawnTimer = 2f;
    private float currentTimer = 0f;

	// Use this for initialization
	void Start () {
        enemyPrefab = Resources.Load("Prefabs/Enemy") as GameObject;
        enemyContainer = GameObject.Find("Enemy_Container").transform;
	}
	
	// Update is called once per frame
	void Update () {
        currentTimer += Time.deltaTime;

        if(currentTimer > spawnTimer)
        {
            //Create an enemy
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, enemyContainer);
            newEnemy.transform.position = transform.position;
            currentTimer = 0;
        }
	}
}
