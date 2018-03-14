using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Route/Route Track")]
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
        if(ctrler_ == null)
            return;
        if(obj == null)
            return;

        if(dist == last_dist)
            return;
        dist = Mathf.Clamp(dist, 0, route.total_length);
        ctrler_.SetRotateEnable(rotation);
        ctrler_.SetMove(dist);
        ctrler_.Apply(obj);
        if(eular != Vector3.zero)
            obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(eular);
        if(position != Vector3.zero)
            obj.transform.position = obj.transform.position + position;
        last_dist = dist;
    }

    /// <summary>
    /// 路点配置
    /// </summary>
    public RouteConfig route;
    /// <summary>
    /// 轨迹对象
    /// </summary>
    public GameObject obj;
    /// <summary>
    /// 面向曲线切线方向
    /// </summary>
    public bool rotation = true;
    /// <summary>
    /// 附加偏移
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// 附加旋转
    /// </summary>
    public Vector3 eular = Vector3.zero;
    /// <summary>
    /// 轨迹距离
    /// </summary>
    public float dist = 0;

    float last_dist = -1;
    RouteController ctrler_;

}
