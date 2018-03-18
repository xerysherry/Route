using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo1 : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        controller_ = new RouteController();
        controller_.SetRouteConfig(GetComponent<RouteConfig>());
        controller_.SetRotateEnable(true);

        controller_.on_enter_point += p =>
        {
            log_.Add("enter: " + p.name);
        };
        controller_.on_exit_point += p =>
        {
            log_.Add("exit: " + p.name);
        };
        controller_.on_message += m =>
        {
            log_.Add("message: " + m);
        };
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(switch_value)
            controller_.Update(-Time.deltaTime, obj);
        else
            controller_.Update(Time.deltaTime, obj);
	}

    void OnGUI()
    {
        if(GUILayout.Button("Reset"))
            controller_.Reset();
        if(GUILayout.Button("Switch"))
        {
            switch_value = !switch_value;
            controller_.SetIsFinish(false);
        }
        while(log_.Count > 10)
            log_.RemoveAt(0);
        foreach(var i in log_)
            GUILayout.Label(i);
    }

    public GameObject obj = null;
    public bool switch_value = false;
    RouteController controller_ = null;
    List<string> log_ = new List<string>();
}
