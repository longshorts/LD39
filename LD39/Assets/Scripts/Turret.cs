using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    Transform enemyContainer;
    public bool Powered;
    public float range = 20f;
    public float shootCooldown = 1f;
    public float shootCurrentTime = 0f;
    public Enemy enemyTarget;

    private Battery m_containedBattery;
    

    [SerializeField]
    private Transform gunPlatform;
    [SerializeField]
    private Transform batteryContainer;

    // Use this for initialization
    void Start () {
        enemyContainer = GameObject.Find("Enemy_Container").transform;
    }

    // Update is called once per frame
    void Update () {
        if (Powered)
        {
            shootCurrentTime += Time.deltaTime;

            if (enemyTarget)
            {
                if(Vector3.Distance(transform.position,enemyTarget.transform.position) <= range && !enemyTarget.isDead)
                {
                    Shoot(enemyTarget);
                } else
                {
                    //target lost
                    enemyTarget = null;
                }
            }

            if (enemyTarget == null)
                FindEnemey();
        }


    }

    private void FindEnemey()
    {
        Enemy[] enemies = enemyContainer.GetComponentsInChildren<Enemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            if(Vector3.Distance(transform.position, enemies[i].transform.position) <= range && !enemies[i].isDead)
            {
                enemyTarget = enemies[i];
                Shoot(enemyTarget);
                break;
            }
        }
    }

    private void Shoot(Enemy enemyTarget)
    {
        //Look at the enemy
        gunPlatform.LookAt(enemyTarget.transform);
        var rot = gunPlatform.eulerAngles;
        rot.x = 0;
        gunPlatform.eulerAngles = rot;

        if(shootCurrentTime >= shootCooldown &&
            Vector3.Distance(transform.position, enemyTarget.transform.position) <= range)
        {
            enemyTarget.ModifyHealth(-1);
            if (enemyTarget.isDead)
                enemyTarget = null;

            shootCurrentTime = 0;
        }
    }

    public void AquireBattery(Battery battery)
    {
        battery.transform.SetParent(batteryContainer);
        m_containedBattery = battery;
        iTween.MoveTo(battery.gameObject, batteryContainer.position, 0.5f);
        Powered = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(enemyTarget)
            Gizmos.DrawRay(transform.position, enemyTarget.transform.position - transform.position);
    }
}
