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
