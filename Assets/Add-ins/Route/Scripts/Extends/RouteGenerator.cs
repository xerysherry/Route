using UnityEngine;
using System.Collections;

[AddComponentMenu("Route/Route Generator")]
public class RouteGenerator : MonoBehaviour 
{
    static void DestroyGameObject(GameObject obj)
    {
#if UNITY_EDITOR
        if(Application.isPlaying)
            GameObject.Destroy(obj);
        else
            GameObject.DestroyImmediate(obj);
#else
            GameObject.Destroy(obj);
#endif
    }

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

    void RefreshObjList()
    {
        var count = transform.childCount;
        obj_list_ = new GameObject[count];
        for(int i = 0; i < count; ++i)
        {
            obj_list_[i] = transform.GetChild(i).gameObject;
        }
    }

    public void Clear()
    {
        RefreshObjList();
        for(int i = 0; i < obj_list_.Length; ++i)
        {
            DestroyGameObject(obj_list_[i]);
        }
        obj_list_ = null;
    }

    public void Generate()
    {
        if(obj == null)
        {
            Clear();
            return;
        }

        Refresh();
        if(ctrler_ == null)
            return;

#if UNITY_EDITOR
        if(!Application.isPlaying)
            route.Refresh();
#endif
        RefreshObjList();

        ctrler_.Reset();

        var totallength = route.total_length;
        var scount = (int)Mathf.Ceil(totallength / step);
        if(scount == 0)
            return;

        var delta = step;
        if(scount > 1 && self_adjust)
        {
            if(route.IsLoop)
            {
                scount -= 1;
                delta = (totallength - step) / (scount - 1);
            }
            else
                delta = totallength / (scount - 1);
        }
        last_obj_list_ = obj_list_;
        obj_list_ = new GameObject[scount];

        var curr = 0.0f;
        var index = 0;

        ctrler_.SetRotateEnable(rotation);
        do
        {
            var o = GetGameObject(index);
            if(o == null)
                return;

            if(curr > totallength)
                curr = totallength;

            ctrler_.SetMove(curr);
            ctrler_.Apply(o);
            if(!rotation)
                o.transform.rotation = obj.transform.rotation;
            if(eular != Vector3.zero)
                o.transform.rotation = o.transform.rotation * Quaternion.Euler(eular);
            if(position != Vector3.zero)
                o.transform.position = o.transform.position + position;

            index += 1;
            curr += delta;
        }while(index < scount);

        if(last_obj_list_ != null && index < last_obj_list_.Length)
        { 
            for(; index<last_obj_list_.Length; ++index)
                DestroyGameObject(last_obj_list_[index]);
        }
        last_obj_list_ = null;
    }

    public GameObject GetGameObject(int index)
    {
        if(index >= obj_list_.Length)
            return null;

        GameObject o = null;
        if(last_obj_list_ != null && index < last_obj_list_.Length)
            o = last_obj_list_[index];
        else
            o = (GameObject)GameObject.Instantiate(obj);
        o.transform.parent = transform;
        obj_list_[index] = o;
        return o;
    }

    /// <summary>
    /// 路点配置
    /// </summary>
    public RouteConfig route;
    /// <summary>
    /// 生成对象
    /// </summary>
    public GameObject obj;
    /// <summary>
    /// 匹配路点长度
    /// </summary>
    public bool self_adjust = true;
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
    /// 生成对象间隔
    /// </summary>
    public float step = 1;

    RouteController ctrler_;
    GameObject[] obj_list_;
    GameObject[] last_obj_list_;
}
