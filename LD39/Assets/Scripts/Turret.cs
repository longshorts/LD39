using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BatteryDock {

    Transform enemyContainer;
    public float range = 20f;
    public float shootCooldown = 0.5f;
    public float shootCurrentTime = 0f;
    public float shotCost = 1f;
    public Enemy enemyTarget;

    GameObject bulletPrefab;
    Transform bulletSpawn;

    int barrelIndex = 0;

    [SerializeField]
    private Transform gunPlatform;
    private GameObject clickTurretObj;

    // Use this for initialization
    void Start () {
        enemyContainer = GameObject.Find("Enemy_Container").transform;
        bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
        bulletSpawn = transform.FindChild("Base").FindChild("BulletSpawn");
        clickTurretObj = GameObject.Find("TurretChargeObj");
    }

    // Update is called once per frame
    void Update () {

        UpdateCharge();

        if (IsPowered())
        {
            shootCurrentTime += Time.deltaTime;

            if(enemyTarget)
            if (enemyTarget.isDead)
                enemyTarget = null;

            if (enemyTarget == null)
                FindEnemey();

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
        
        if(shootCurrentTime >= shootCooldown)

        if(shootCurrentTime >= shootCooldown &&
            Vector3.Distance(transform.position, enemyTarget.transform.position) <= range)
        {
                GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.GetChild(barrelIndex).position, Quaternion.identity);
                bullet.GetComponent<Bullet>().target = enemyTarget;
                barrelIndex = (barrelIndex + 1) % 2;
            
            shootCurrentTime = 0;
            m_containedBattery.ModifyCharge(-shotCost * Game.instance.shotEnergyMultiplier);
        }
    }

    public override void AquireBattery(Battery battery)
    {
        base.AquireBattery(battery);
        clickTurretObj.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(enemyTarget)
            Gizmos.DrawRay(transform.position, enemyTarget.transform.position - transform.position);
    }
}
