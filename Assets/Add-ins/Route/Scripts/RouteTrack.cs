using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RouteTrack : MonoBehaviour 
{
    void Refresh()
    {
        if(route == null)
        {
            route = GetComponent<RouteConfig>();
            if(route == null)
            {
                ctrler_ = null;
                return;
            }
        }
        if(ctrler_ == null)
        {
            ctrler_ = new RouteController();
            ctrler_.SetRouteConfig(route);
        }
        else if(ctrler_.route_config != route)
        {
            ctrler_.SetRouteConfig(route);
        }
    }

    void Update()
    {
        Refresh();
        if(obj == null)
            return;

        c = ctrler_.current;

        if(dist == last_dist)
            return;
        dist = Mathf.Clamp(dist, 0, route.total_length);
        ctrler_.Reset();
        ctrler_.SetRotateEnable(rotation);
        ctrler_.Step(dist);
        ctrler_.Apply(obj);
        last_dist = dist;
    }

    public RouteConfig route;
    public GameObject obj;
    public bool rotation = false;
    public float dist = 0;
    float last_dist = -1;
    int c = 0;

    RouteController ctrler_;

}
