using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo4 : MonoBehaviour
{
	// Use this for initialization
	void Start () {
        var g = obj.GetComponent<RouteGenerator>();
        var t = 0;
        g.on_generate = (i, o) =>
        {
            var rt = o.GetComponent<RouteTime>();
            rt.start_time = t++;
            rt.enabled = true;
            o.GetComponent<Animation>().Play("Walk");
        };
        g.Generate();
	}
    public GameObject obj;
}
