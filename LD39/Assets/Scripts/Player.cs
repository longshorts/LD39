using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

    [SerializeField]
    private Transform m_batteryContainer;

    private NavMeshAgent m_navMeshAgent;
    private Camera m_camera;
    private Vector3 m_hitPoint;
    private Battery m_batteryTarget;
    private Battery m_containedBattery;

    private const float m_batteryCollectionRange = 1f;
    

	// Use this for initialization
	void Start () {
        m_camera = Camera.main;
        m_navMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInput();

        if(m_batteryTarget)
        if(Vector3.Distance(transform.position, m_batteryTarget.transform.position) < m_batteryCollectionRange)
        {
                //Collect the battery
            if (m_batteryContainer)
            {
                m_batteryTarget.transform.SetParent(m_batteryContainer);
                    m_containedBattery = m_batteryTarget;
                m_batteryTarget = null;
            }
        }
	}

    private void FixedUpdate()
    {

        if (m_containedBattery)
        {
            iTween.MoveUpdate(m_containedBattery.gameObject, m_batteryContainer.position, 0.5f);
        }
    }

    void ProcessInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            m_batteryTarget = null;
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500))
            {
                m_hitPoint = hit.point;

                m_batteryTarget = hit.transform.GetComponentInChildren<Battery>();

                if (m_batteryTarget)
                {
                    Debug.Log("Battery found");
                }

                Turret turret = hit.transform.GetComponentInParent<Turret>();
                if (turret)
                {
                    Debug.Log("Turret found");
                    if (m_containedBattery)
                    {
                        turret.AquireBattery(m_containedBattery);
                        m_containedBattery = null;
                    }
                }

                m_navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_hitPoint == null)
            return;
        Vector3 direction = m_hitPoint - transform.position;
        Gizmos.DrawRay(transform.position, direction);
    }
}
