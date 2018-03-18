using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RouteComponent : MonoBehaviour
{
    protected void RefreshConfig()
    {
        if(route == null)
        {
            route = GetComponent<RouteConfig>();
            if(route == null)
            {
                controller_ = null;
                return;
            }
        }
        if(controller_ == null)
        {
            controller_ = new RouteController();
            controller_.SetRouteConfig(route);
        }
        else if(controller_.route_config != route)
        {
            controller_.SetRouteConfig(route);
        }
    }
    /// <summary>
    /// 路径配置（Null为空时，生成时会查询自身的RouteConfig组件)
    /// </summary>
    public RouteConfig route;

    public RouteController controller { get { return controller_; } }
    protected RouteController controller_;
}
