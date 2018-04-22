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
using Bezier3D = RouteMath.Bezier3D;

[ExecuteInEditMode]
[AddComponentMenu("Route/Route Config")]
public class RouteConfig : MonoBehaviour
{
    /// <summary>
    /// 切割精细度
    /// 值越大切割就越精确，但是生成成本大（非运行时计算）
    /// </summary>
    static readonly float kStepParts = 100;

	void Awake () 
    {
        Refresh();
	}

#if UNITY_EDITOR
    void DrawCurve(Bezier3D bezier, Quaternion rot, 
                RoutePoint pt0, RoutePoint pt1)
    {
        var p0 = pt0.transform.position;
        var p1 = p0 + rot * pt0.next_weight_point;
        var p3 = pt1.transform.position;
        var p2 = p3 + rot * pt1.prev_weight_point;

        bezier.Set(p0, p1, p2, p3);

        float iv = 1 / ((p0 - p3).magnitude / (step_length * 2));
        if(iv < 0.001f)
            //间隔太大
            iv = 0.001f;

        Gizmos.color = pt0.color;
        if((pt0.delta_next.magnitude == 0 && pt1.delta_next.magnitude == 0))
        {
            Gizmos.DrawLine(p0, p3);
        }
        else
        {
            int arrow = 0;
            float t = 0;
            Vector3 lp = p0;
            Color dc = pt1.color - pt0.color;
            do
            {
                t += iv;
                if(t > 1.0f)
                    t = 1.0f;
                Vector3 p = bezier.Get(t);
                Gizmos.color = pt0.color + dc * t;
                Gizmos.DrawLine(lp, p);

                if(arrow++ % 2 == 0)
                {
                    Vector3 tangent = bezier.GetTangent(t);
                    var angle = RouteMath.GetPolarEular(tangent);
                    var quat = Quaternion.AngleAxis(angle, RouteMath.kAxisY);
                    Vector3 v0 = quat * RouteMath.kAxisX;
                    Vector3 axis = Vector3.Cross(p - lp, v0);
                    v0 *= 0.3f;

                    Vector3 v1 = Quaternion.AngleAxis(-70, axis) * v0;
                    Gizmos.DrawLine(p, p - v1);
                    v1 = Quaternion.AngleAxis(-110, axis) * v0;
                    Gizmos.DrawLine(p, p - v1);
                }

                lp = p;
            } while(t < 1.0f);
        }
    }

    void OnDrawGizmos() 
    {
        foreach(var point in points_)
        {
            if(!point)
            {
                Refresh();
                break;
            }
        }

        //绘制节点信息
        int count = points_.Length;
        if(count < 0)
            return;

        Gizmos.color = Color.green;
        Bezier3D bezier = new Bezier3D();
        Quaternion rot = transform.rotation;

        if(count > 1)
        {
            int i = 0;
            for(; i < count - 1; ++i)
            {
                DrawCurve(bezier, rot, points_[i], points_[i + 1]);
            }
            if(loop_)
                DrawCurve(bezier, rot, points_[i], points_[0]);
        }
    }

    float RefreshLength(Bezier3D bezier, Quaternion rot, RoutePoint pt0, RoutePoint pt1)
    {
        var length = 0.0f;
        var p0 = pt0.transform.position;
        var p1 = p0 + rot * pt0.next_weight_point;
        var p3 = pt1.transform.position;
        var p2 = p3 + rot * pt1.prev_weight_point;

        bezier.Set(p0, p1, p2, p3);

        float microstep = step_length / kStepParts;
        float step = 0;
        pt0.steps_ = new float[0];

        float t = 0;
        Vector3 lp = p0;

        do
        {
            t += microstep;
            if(t > 1.0f)
                t = 1.0f;
            Vector3 p = bezier.Get(t);
            var l = (lp - p).magnitude;
            step += l;
            if(step >= step_length)
            {
                UnityEditor.ArrayUtility.Add(ref pt0.steps_, t);
                step -= step_length;
            }
            length += l;
            lp = p;
        } while(t < 1.0f);

        if(pt0.velocity > 0 && length / pt0.velocity < 0.001f)
            pt0.steps_ = new float[0];

        return length;
    }

    /// <summary>
    /// 刷新路径长度
    /// </summary>
    public void RefreshLength()
    {
        if(points_.Length == 0)
            return;

        if(loop_)
            length = new float[points_.Length];
        else
            length = new float[points_.Length - 1];
        total_length = 0;

        Bezier3D bezier = new Bezier3D();
        Quaternion rot = transform.rotation;

        int i = 0;
        for(; i < points_.Length - 1; ++i)
        {
            length[i] = RefreshLength(bezier, rot, points_[i], points_[i + 1]);
            total_length += length[i];
        }
        if(loop_)
        {
            length[i] = RefreshLength(bezier, rot, points_[i], points_[0]);
            total_length += length[i];
        }
    }

    /// <summary>
    /// 设置是否闭环
    /// </summary>
    public void SetLoop(bool value)
    {
        loop_ = value;
    }
#endif

    public void Refresh()
    {
        //current_ = -1;
        //获得路点信息
        points_ = GetComponentsInChildren<RoutePoint>();
        beziers_ = new RouteMath.Bezier3D[points_.Length];
#if UNITY_EDITOR
        RefreshLength();
#endif
    }
    /// <summary>
    /// 是否为闭环
    /// </summary>
    public bool IsLoop { get { return loop_; } }
    /// <summary>
    /// 路径点数组
    /// </summary>
    public RoutePoint[] points { get { return points_; } }
    RoutePoint[] points_;
    public RoutePoint this[int index] 
    { 
        get
        {
            if(points_ == null || index >= points_.Length)
                return null;
            return points_[index];
        }
    }
    /// <summary>
    /// 路点数量
    /// </summary>
    public int count
    {
        get
        {
            if(points_ == null)
                return 0;
            return points_.Length;
        }
    }
    /// <summary>
    /// 曲线数组
    /// </summary>
    Bezier3D[] beziers_;

    /// <summary>
    /// 切割单位长
    /// 值越小，切割越细，曲线越光滑，但是存储空间的越大
    /// </summary>
    [Range(0, 1)]
    public float step_length = 0.1f;
    /// <summary>
    /// 全局速度，在子节点速度未配置时有效。如果为0，表示无效
    /// </summary>
    public float velocity = 1;
    /// <summary>
    /// 是否为闭环
    /// </summary>
    [HideInInspector]
    [SerializeField]
    private bool loop_ = false;
    /// <summary>
    /// 路径间长度缓存数据
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public float[] length;
    /// <summary>
    /// 总长度
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public float total_length;

    /// <summary>
    /// 通过总长度设置对应点，并获得标准量
    /// </summary>
    /// <param name="dist"></param>
    /// <returns></returns>
    public float GetNormalizedByTotalLength(float dist)
    {
        int index = 0;
        var n = GetNormalizedByTotalLength(dist, out index);
        return n;
    }
    public float GetNormalizedByTotalLength(float dist, out int index)
    {
        if(dist < 0)
            dist = 0;
        else if(dist > total_length)
            dist = total_length;

        int count = length.Length;
        float movelength = 0;

        index = 0;
        for(; index < count; ++index)
        {
            var l = length[index];
            if(dist < l)
            {
                movelength = dist;
                break;
            }
            dist -= l;
        }
        return GetNormalized(index, movelength);
    }

    /// <summary>
    /// 获取曲线类
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Bezier3D GetBezier3D(int index)
    {
        if(index < 0)
            index = 0;
        if(loop_)
        {
            if(index > points_.Length - 1)
                index = points_.Length - 1;
        }
        else
        {
            if(index > points_.Length - 2)
                index = points_.Length - 2;
        }
#if UNITY_EDITOR
        if(beziers_ == null)
            beziers_ = new Bezier3D[points_.Length];
#endif
        Bezier3D b = beziers_[index];
        if(b == null)
        {
            Quaternion rot = transform.rotation;
            RoutePoint pt0 = null, pt1 = null;
            if(index < points_.Length - 1)
            {
                pt0 = points_[index];
                pt1 = points_[index + 1];
            }
            else
            {
                pt0 = points_[index];
                pt1 = points_[0];
            }

            var p0 = pt0.transform.position;
            var p1 = p0 + rot * pt0.next_weight_point;
            var p3 = pt1.transform.position;
            var p2 = p3 + rot * pt1.prev_weight_point;

            b = new Bezier3D(p0, p1, p2, p3);
            beziers_[index] = b;
        }
        return b;
    }
    /// <summary>
    /// 获取下一个索引
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetNextIndex(int current)
    {
        int next = current + 1;
        if(loop_)
            next = next % points_.Length;
        else
            next = Mathf.Clamp(next, 0, points_.Length - 2);
        return next;
    }
    /// <summary>
    /// 获取上一个索引
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetPrevIndex(int current)
    { 
        int prev = current - 1;
        if(loop_)
            prev = (prev + points_.Length) % points_.Length;
        else
            prev = Mathf.Clamp(prev, 0, points_.Length - 2);
        return prev;
    }
    /// <summary>
    /// 获得标准T
    /// </summary>
    /// <param name="index">点索引</param>
    /// <param name="move">移动距离</param>
    public float GetNormalized(int index, float move)
    {
        if(index < 0 ||
            index >= points_.Length ||
            index >= length.Length)
            return 0;

        var l = length[index];
        if(l == 0)
            return 0;

        var point = points_[index];
        if(point.steps_.Length == 0)
            return move / l;

        var sf = (move / step_length);
        var si = (int)sf;
        if(si >= point.steps_.Length)
            return 1;

        float prevt = 0;
        if(si > 0)
            prevt = point.steps_[si - 1];
        float nextt = point.steps_[si];

        var t = (sf - si) * (nextt - prevt) + prevt;
        return t;
    }
}
