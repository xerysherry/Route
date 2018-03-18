using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo3 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        footman_controller = new RouteController();
        footman_controller.SetRouteConfig(footman_config);
        footman_controller.SetRotateEnable(true);
        footman.GetComponent<Animation>().Play("Walk");
        footman_controller.on_message += m =>
        {
            footman.GetComponent<Animation>().Play(m);
            Debug.Log(m);
        };

        camera_controller = new RouteController();
        camera_controller.SetRouteConfig(camera_config);
        camera_controller.SetRotateEnable(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        footman_controller.Update(Time.deltaTime, footman);
        camera_controller.Update(Time.deltaTime, camera);
    }

    public RouteConfig footman_config;
    public GameObject footman;
    RouteController footman_controller;

    public RouteConfig camera_config;
    public GameObject camera;
    RouteController camera_controller;
}
