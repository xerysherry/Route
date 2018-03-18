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
[AddComponentMenu("Route/Route Point")]
public class RoutePoint : MonoBehaviour
{
    static readonly float kPointSize = 0.31f;

#if UNITY_EDITOR
    void Update()
    {
        transform.eulerAngles = Vector3.zero;
        UpdateColorSet();

        keeptime = Mathf.Max(0, keeptime);
        velocity = Mathf.Max(0, velocity);
    }

    void UpdateColorSet()
    {
        if(colorset == ColorSet.kNone)
            return;
        color = color_array[(int)colorset];
        colorset = ColorSet.kNone;
    }

    void OnDrawGizmosSelected()
    {
        OnDrawGizmos();
    }

    void OnDrawGizmos()
    {
        DrawPoint(color);
        //if(show_arrow_)
            DrawArrow(color);
    }

    void DrawPoint(Color color)
    {
        float r = transform.localScale.x * kPointSize;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, r);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, r);
    }

    void DrawArrow(Color color)
    {
        if(next_weight_point == Vector3.zero)
            return;

        Quaternion rot = Quaternion.identity;
        RouteConfig config = GetConfig();
        if(config)
            rot = config.transform.rotation;

        Gizmos.color = color;
        var next_axis = rot * next_weight_point.normalized;
        var next_point = transform.position + rot * next_weight_point;
        Gizmos.DrawLine(transform.position, next_point);
        DrawArrow(next_axis, next_point);

        var prev_axis = rot * prev_weight_point.normalized;
        var prev_point = transform.position + rot * prev_weight_point;
        Gizmos.DrawLine(transform.position, prev_point);
        DrawArrow(-prev_axis, prev_point - prev_weight_point.normalized * 0.3f);
    }

    void DrawArrow(Vector3 axis, Vector3 point)
    {
        var p = Vector3.zero;
        if(Mathf.Abs(axis.x) < 0.0001f &&
            Mathf.Abs(axis.y) == 1 &&
            Mathf.Abs(axis.z) < 0.0001f)
            p = Vector3.Cross(axis, Vector3.right).normalized;
        else
            p = Vector3.Cross(axis, Vector3.up).normalized;
        p = p * 0.1f;

        var t = point - axis * 0.3f;
        var a1 = Vector3.zero;
        var a2 = Vector3.zero;
        var la1 = t;
        var la2 = t;
        for(int i = 0; i < 5; ++i)
        {
            var q = Quaternion.AngleAxis(360 / (5 * 2) * i, axis) * p;
            a1 = t + q;
            Gizmos.DrawLine(t, a1);
            Gizmos.DrawLine(a1, point);
            Gizmos.DrawLine(a1, la1);
            la1 = a1;

            a2 = t - q;
            Gizmos.DrawLine(t, a2);
            Gizmos.DrawLine(a2, point);
            Gizmos.DrawLine(a2, la2);
            la2 = a2;
        }
        Gizmos.DrawLine(a1, t - p);
        Gizmos.DrawLine(a2, t + p);
    }

    static readonly Color[] color_array = new Color[]
    {
        Color.white,
        Color.green,
        Color.red,
        Color.blue,
        Color.black,
        Color.cyan,
        Color.gray,
        Color.white,
        Color.magenta,
        Color.yellow,
    };

    //public void ShowArrow(bool value)
    //{
    //    show_arrow_ = value;
    //}
    //bool show_arrow_ = true;
#endif

    void OnDestroy()
    {}

    public RouteConfig GetConfig()
    {
        if(!transform.gameObject)
            return GetComponent<RouteConfig>();

        GameObject curr = gameObject;
        RouteConfig config = null;
        do
        {
            var parent = curr.transform.parent.gameObject;
            config = parent.GetComponent<RouteConfig>();
            if(config)
                break;
            curr = parent;
        } while(curr.transform.parent != null);
        return config;
    }

    /// <summary>
    /// 颜色
    /// </summary>
    public Color color = Color.green;
    /// <summary>
    /// 快速设置
    /// </summary>
    public enum ColorSet
    { 
        kNone = 0,
        kGreen,
        kRed,
        kBlue,
        kBlack,
        kCyan,
        kGray,
        kWhite,
        kMegenta,
        kYellow,
    }
    public ColorSet colorset;
    /// <summary>
    /// 消息
    /// </summary>
    public string message = "";
    /// <summary>
    /// 停留时间
    /// </summary>
    public float keeptime = 0;
    /// <summary>
    /// 局部速度，前往下一点速度，0表示无效
    /// </summary>
    public float velocity = 0;

    public Vector3 prev_weight_point = new Vector3(-1, 0, 0);
    public Vector3 next_weight_point = new Vector3(1, 0, 0);

    public Vector3 delta_next { get { return next_weight_point - transform.position; } }
    public Vector3 delta_prev { get { return prev_weight_point - transform.position; } }

    public enum WeightType
    {
        kNormal,
        kBroken,
    }
    [HideInInspector]
    public WeightType weight_type_ = WeightType.kNormal;

    [HideInInspector]
    [SerializeField]
    public float[] steps_;
}
