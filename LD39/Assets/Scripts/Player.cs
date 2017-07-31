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
    private BatteryDock m_dockTarget;
    private Battery m_containedBattery;
    private Game m_game;

    [SerializeField]
    private GameObject clickToMoveObj;
    [SerializeField]
    private GameObject clickBatteryObj;
    [SerializeField]
    private GameObject clickBatteryHolderObj;
    [SerializeField]
    private GameObject clickChargingStationObj;

    private GameObject m_target;

    private const float m_batteryCollectionRange = 1f;
    private const float m_batteryDeliveryRange = 1.8f;
    

	// Use this for initialization
	void Start () {
        m_camera = Camera.main;
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_game = FindObjectOfType<Game>();
	}
	
	// Update is called once per frame
	void Update () {

        if (m_game.state != Game.GameState.PLAY)
            return;

        ProcessInput();

        //Battery target behaviour
        if(m_batteryTarget)
        if(Vector3.Distance(transform.position, m_batteryTarget.transform.position) < m_batteryCollectionRange)
        {
                //Collect the battery
            if (m_batteryContainer)
            {
                m_batteryTarget.transform.SetParent(m_batteryContainer);
                    m_containedBattery = m_batteryTarget;
                m_batteryTarget = null;
                clickBatteryObj.SetActive(false);
                }
            }

        //Dock target behaviour
        if (m_dockTarget)
        if(Vector3.Distance(transform.position, m_dockTarget.GetBatteryContainer().position) < m_batteryDeliveryRange)
        {
            Battery dockBattery = m_dockTarget.GetBattery();
            clickBatteryHolderObj.SetActive(false);

            //Clear the dock's battery reference if it has one
            if (dockBattery)
            m_dockTarget.ClearBattery();

            //If we have a battery, give it to the dock
            if (m_containedBattery)
            {
                m_dockTarget.AquireBattery(m_containedBattery);
                m_containedBattery = null;
            }

            //Get the dock's battery
            if (dockBattery)
            {
                dockBattery.transform.SetParent(m_batteryContainer);
                m_containedBattery = dockBattery;
            }

            m_dockTarget = null;
            m_navMeshAgent.ResetPath();
        }

        //Charging station behaviour
        if (m_target)
        {
            ChargingStation station = m_target.GetComponent<ChargingStation>();
            if (station)
            {
                if (station.RobotDocked)
                    return;

                //If we have reached the station, rotate
                if(m_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && m_navMeshAgent.remainingDistance == 0f)
                {
                    if (!RotateTowards(station.transform))
                    {
                        //If nothing left to rotate, dock the robot
                        station.DockRobot(true);
                        clickChargingStationObj.SetActive(false);
                    }
                }
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
            m_dockTarget = null;
            if (m_target)
            {
                ChargingStation station = m_target.GetComponent<ChargingStation>();
                if (station)
                {
                    station.DockRobot(false);
                }
            }
            m_target = null;
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500))
            {
                m_hitPoint = hit.point;
                clickToMoveObj.SetActive(false);

                m_batteryTarget = hit.transform.GetComponent<Battery>();

                if (m_batteryTarget)
                {
                    //If battery is in a dock, set the target to the dock instead
                    if(m_batteryTarget.Dock != null)
                    {
                        m_dockTarget = m_batteryTarget.Dock;
                        m_batteryTarget = null;
                    }
                }

                if(hit.transform.tag == "BatteryDock")
                {
                    Debug.Log("dock hit");
                    BatteryDock dock = hit.transform.GetComponentInParent<BatteryDock>();
                    if (m_containedBattery)
                    {
                        m_dockTarget = dock;
                    }
                } if(hit.transform.tag == "ChargingStation")
                {
                    Debug.Log("station hit");
                    ChargingStation station = hit.transform.GetComponent<ChargingStation>();
                    m_navMeshAgent.SetDestination(station.RobotDestinationPoint.position);
                    m_target = station.gameObject;
                    return;
                }

                m_navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private const float k_rotationSpeed = 5f;

    private bool RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, lookRotation);
        Debug.Log("Angle: " + angle);
        if (angle >= 30f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * k_rotationSpeed);
            return true;
        }
        else
        {
            return false;
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
