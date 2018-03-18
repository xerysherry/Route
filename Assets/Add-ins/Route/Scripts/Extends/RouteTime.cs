using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteTime : RouteComponent
{
	// Use this for initialization
	void Start ()
    {
        RefreshConfig();
        controller_.SetRotateEnable(rotation);
        controller_.Update(start_time);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(obj == null)
            return;
        controller_.SetRotateEnable(rotation);
        controller_.Update(delta, obj);
	}

    float delta
    {
        get
        {
            float dt = 0;
            switch(mode)
            {
            case Mode.DELTA_TIME:
                dt = Time.deltaTime;
                break;
            case Mode.FIXED_DELTA_TIME:
                dt = Time.fixedDeltaTime;
                break;
            case Mode.FIXED_UNSCALED_DELTA_TIME:
                dt = Time.fixedUnscaledDeltaTime;
                break;
            case Mode.CUSTOM_TIME:
                dt = custom_delta;
                break;
            }
            if(negative)
                dt = -dt;
            return dt;
        }
    }

    public GameObject obj;
    public bool rotation = true;
    public bool negative = false;
    public float start_time = 0;
    public enum Mode
    {
        DELTA_TIME,
        FIXED_DELTA_TIME,
        FIXED_UNSCALED_DELTA_TIME,
        CUSTOM_TIME,
    }
    public Mode mode = Mode.DELTA_TIME;
    public float custom_delta = 0;
}
