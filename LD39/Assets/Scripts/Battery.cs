using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour {

    public float BatteryCharge { get; private set; }
    public BatteryDock Dock;

    public const float k_maxCharge = 100f;

	// Use this for initialization
	void Start () {
        BatteryCharge = 50f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Returns true if there is still power to draw
    public bool ModifyCharge(float modifier)
    {
        BatteryCharge += modifier;
        BatteryCharge = Mathf.Clamp(BatteryCharge, 0f, k_maxCharge);
        return BatteryCharge > 0f;
    }
}
