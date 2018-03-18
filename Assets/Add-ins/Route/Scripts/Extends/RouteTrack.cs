using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Route/Route Track")]
public class RouteTrack : RouteComponent
{
    public void Refresh()
    {
        RefreshConfig();
        if(controller_ != null)
        {
            route.Refresh();
            controller_.Reset();
            Update();
        }
    }
    void Update()
    {
        RefreshConfig();
        if(controller_ == null)
            return;
        if(obj == null)
            return;

        if(dist == last_dist)
            return;
        dist = Mathf.Clamp(dist, 0, total_length);
        controller_.SetRotateEnable(rotation);
        controller_.SetMove(dist);
        controller_.Apply(obj);
        if(eular != Vector3.zero)
            obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(eular);
        if(position != Vector3.zero)
            obj.transform.position = obj.transform.position + position;
        last_dist = dist;
    }
    public float total_length
    {
        get
        {
            if(route == null)
                return 0;
            return route.total_length;
        }
    }

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
}
