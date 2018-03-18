/* Copyright 2018 xerysherry
 * 
 * Permission is hereby granted, free of charge, to any person 
 * obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, 
 * merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall 
 * be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("Route/Route Generator")]
public class RouteGenerator : RouteComponent
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
        RefreshConfig();
    }

    void RefreshObjList()
    {
        var count = transform.childCount;
        obj_list_ = new List<GameObject>();
        for(int i = 0; i < count; ++i)
        {
            var obj = transform.GetChild(i).gameObject;
            if(!obj.GetComponent<RoutePoint>())
                obj_list_.Add(obj);
        }
    }

    public void Clear()
    {
        RefreshObjList();
        for(int i = 0; i < obj_list_.Count; ++i)
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
        if(controller_ == null)
            return;

#if UNITY_EDITOR
        if(!Application.isPlaying)
            route.Refresh();
#endif
        RefreshObjList();

        controller_.Reset();

        if(step <= 0.000001f)
            step = 1;
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
        obj_list_ = new List<GameObject>(scount);

        var curr = 0.0f;
        var index = 0;
        var sum_pos = Vector3.zero;
        var sum_eular = Vector3.zero;

        controller_.SetRotateEnable(rotation);
        do
        {
            var o = GetGameObject(index);
            if(o == null)
                return;

            if(indexnamed)
                o.name = obj.name + "_" + index;
            else
                o.name = obj.name;

            controller_.SetMove(curr);
            controller_.Apply(o);
            if(!rotation)
                o.transform.rotation = obj.transform.rotation;
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

            if(on_generate != null)
                on_generate(index, o);

            curr += delta;
            if(curr > totallength)
                curr = totallength;
            index += 1;
        } while(index < scount);

        if(last_obj_list_ != null && index < last_obj_list_.Count)
        { 
            for(; index<last_obj_list_.Count; ++index)
                DestroyGameObject(last_obj_list_[index]);
        }
        last_obj_list_ = null;
    }

    public GameObject GetGameObject(int index)
    {
        if(index >= obj_list_.Capacity)
            return null;

        GameObject o = null;
        if(last_obj_list_ != null && index < last_obj_list_.Count)
            o = last_obj_list_[index];
        else
            o = (GameObject)GameObject.Instantiate(obj);
        o.transform.parent = transform;
        while(obj_list_.Count < index + 1)
            obj_list_.Add(null);
        obj_list_[index] = o;
        return o;
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
    /// 序号名
    /// </summary>
    public bool indexnamed = true;
    /// <summary>
    /// 生成对象间隔
    /// </summary>
    public float step = 1;
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
    /// <summary>
    /// 创建回掉
    /// </summary>
    public Action<int, GameObject> on_generate = null;

    List<GameObject> obj_list_;
    List<GameObject> last_obj_list_;
}
