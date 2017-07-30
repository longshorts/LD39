using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Transform[] m_waypoints;

    int waypointIndex = 0;
    Transform destination;
    float destinationDistance = 0.1f;
    float movementSpeed = 6f;
    float sinkSpeed = 2f;

    float deathTime = 2f;
    float deathTimer = 0f;

    public bool isDead { get; private set; }

    private int hitPoints = 3;

	// Use this for initialization
	void Start () {
        m_waypoints = FindObjectOfType<Waypoints>().waypoints;
        isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (isDead)
        {
            //Sink
            float step = sinkSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, transform.position - Vector3.up, step);
            deathTimer += Time.deltaTime;
            if(deathTimer >= deathTime)
            {
                GameObject.Destroy(gameObject);
            }
            return;
        }

        if (!destination)
        {
            //Get new destination
            if(waypointIndex < m_waypoints.Length)
            {
                destination = m_waypoints[waypointIndex];
                waypointIndex++;
            } else
            {
                GoalReached();
            }
        } else
        {
            float step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination.position, step);
            if(Vector3.Distance(transform.position, destination.position) <= destinationDistance)
            {
                destination = null;
            }
        }
	}

    void GoalReached()
    {
        GameObject.Destroy(gameObject);
    }

    public void ModifyHealth(int modifier)
    {
        hitPoints += modifier;
        if (hitPoints <= 0)
            isDead = true;
    }
}
