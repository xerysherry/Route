using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Route/Route Placement")]
public class RoutePlacement : RouteComponent
{
#if UNITY_EDITOR
    static readonly Color kStartColor = Color.yellow;
    static readonly Color kEndColor = Color.red;
    static readonly float kPointSize = 0.1f;
    void OnDrawGizmos()
    {
        if(controller_ == null)
            return;

        start_dist = Mathf.Clamp(start_dist, 0, controller_.total_length);
        end_dist = Mathf.Clamp(end_dist, start_dist, controller_.total_length);

        int index = 0;
        float t = route.GetNormalizedByTotalLength(start_dist, out index);
        var p = route.GetBezier3D(index).Get(t);

        Gizmos.color = kStartColor;
        Gizmos.DrawWireSphere(p, kPointSize);

        t = route.GetNormalizedByTotalLength(end_dist, out index);
        p = route.GetBezier3D(index).Get(t);

        Gizmos.color = kEndColor;
        Gizmos.DrawWireSphere(p, kPointSize);
    }
#endif

    public void Refresh()
    {
        RefreshConfig();

        if(controller_ != null)
        {
            start_dist = Mathf.Clamp(start_dist, 0, controller_.total_length);
            if(end_dist == 0)
                end_dist = controller_.total_length;
            else
                end_dist = Mathf.Clamp(end_dist, start_dist, controller_.total_length);
            route.Refresh();
        }
    }

    void RefreshObjList()
    {
        var count = transform.childCount;
        obj_list_ = new List<GameObject>();
        for(int i = 0; i < count; ++i)
        {
            var obj = transform.GetChild(i).gameObject;
            if(!obj.activeSelf)
                continue;
            if(obj.GetComponent<RoutePoint>())
                continue;
            obj_list_.Add(obj);
        }
    }

    public void Setup()
    {
        Refresh();
        if(controller_ == null)
            return;
        RefreshObjList();
        if(obj_list_.Count == 0)
            return;

        start_dist = Mathf.Clamp(start_dist, 0, controller_.total_length);
        end_dist = Mathf.Clamp(end_dist, start_dist, controller_.total_length);

        controller_.Reset();
        controller_.SetRotateEnable(rotation);
        
        if(obj_list_.Count == 1)
        {
            var o = obj_list_[0];
            controller_.SetMove(start_dist);
            controller_.Apply(o);
            if(eular != Vector3.zero)
                o.transform.rotation = o.transform.rotation * Quaternion.Euler(eular);
            if(position != Vector3.zero)
                o.transform.position = o.transform.position + position;
        }
        else
        {
            var totallength = route.total_length;
            var dist = end_dist - start_dist;
            var step = dist / (obj_list_.Count - 1);
            var curr = start_dist;
            var sum_pos = Vector3.zero;
            var sum_eular = Vector3.zero;

            for(int i=0; i<obj_list_.Count; ++i)
            {
                var o = obj_list_[i];
                controller_.SetMove(curr);
                controller_.Apply(o);
                if(!rotation)
                    o.transform.rotation = Quaternion.identity;
                if(eular != Vector3.zero)
                    o.transform.rotation = o.transform.rotation * Quaternion.Euler(eular);
                if(position != Vector3.zero)
                    o.transform.position = o.transform.position + position;
                if(add_position != Vector3.zero)
                {
                    sum_pos += add_position;
                    o.transform.position = o.transform.position + sum_pos;
                }
                if(add_eular != Vector3.zero)
                {
                    sum_eular += add_eular;
                    o.transform.rotation = o.transform.rotation * Quaternion.Euler(sum_eular);
                }
                curr += step;
                if(curr > totallength)
                    curr = totallength;
            }
        }
        obj_list_.Clear();
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
    /// 面向曲线切线方向
    /// </summary>
    public bool rotation = true;
    /// <summary>
    /// 开始偏远
    /// </summary>
    public float start_dist = 0;
    /// <summary>
    /// 结束距离
    /// </summary>
    public float end_dist = 0;
    /// <summary>
    /// 附加偏移
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// 附加旋转
    /// </summary>
    public Vector3 eular = Vector3.zero;
    /// <summary>
    /// 叠加偏移
    /// </summary>
    public Vector3 add_position = Vector3.zero;
    /// <summary>
    /// 叠加旋转
    /// </summary>
    public Vector3 add_eular = Vector3.zero;

    List<GameObject> obj_list_;
}
