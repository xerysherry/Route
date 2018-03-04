using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class TrunkConfig : MonoBehaviour
{ 
#if UNITY_EDITOR
    static readonly Vector3 kAxisX = new Vector3(1, 0, 0);

    void OnDrawGizmos()
    {
        var tps = GetComponentsInChildren<TrunkPoint>();
        Dictionary<TrunkPoint, HashSet<TrunkPoint>> td = new Dictionary<TrunkPoint, HashSet<TrunkPoint>>();
        foreach(var tp in tps)
        { 
            var tpps = tp.points;
            var set = new HashSet<TrunkPoint>();
            foreach(var t in tpps)
            {
                if(t != null && !set.Contains(t))
                    set.Add(t);
            }
            td[tp] = set;
        }

        TrunkPoint.material.SetPass(0);

        var ro = Mathf.Max(TrunkPoint.kRingRadius1, TrunkPoint.kRingRadius2);
        foreach(var tp in tps)
        {
            var ts1 = td[tp];
            foreach(var tp2 in ts1)
            {
                var ts2 = td[tp2];
                if(ts2.Contains(tp))
                {
                    DrawDoubleArrow(tp.transform.position, tp2.transform.position,
                                    tp.color, tp2.color, ro);
                    ts2.Remove(tp);
                }
                else
                {
                    DrawSingleArrow(tp.transform.position, tp2.transform.position,
                                tp.color, tp2.color, ro);
                }

            }
        }

    }

    static readonly float kLineWidth = 0.2f;
    static readonly float kArrowWidth = 0.3f;
    static readonly float kArrowHeight = 0.6f;

    void DrawSingleArrow(Vector3 start, Vector3 end, 
                        Color startcolor, Color endcolor, float offset)
    {
        var delta = (end - start).normalized;
        var deltaXZ = new Vector3(delta.x, 0, delta.z); deltaXZ.Normalize();

        var quat = Quaternion.FromToRotation(kAxisX, deltaXZ);
        start += deltaXZ * offset;
        end -= deltaXZ * offset;
        GL.Color(endcolor);
        DrawRightArrow(end, endcolor, quat);
        DrawLine(start, end - deltaXZ * kArrowWidth, quat,
                startcolor, endcolor);
    }

    void DrawDoubleArrow(Vector3 start, Vector3 end,
                        Color startcolor, Color endcolor, float offset)
    {
        var delta = (end - start).normalized;
        var deltaXZ = new Vector3(delta.x, 0, delta.z); deltaXZ.Normalize();

        var quat = Quaternion.FromToRotation(kAxisX, deltaXZ);

        start += deltaXZ * offset;
        end -= deltaXZ * offset;
        DrawLeftArrow(start, startcolor, quat);
        DrawRightArrow(end, endcolor, quat);
        DrawLine(start + deltaXZ * kArrowWidth, end - deltaXZ * kArrowWidth, quat,
                startcolor, endcolor);
    }

    void DrawLeftArrow(Vector3 p, Color color, Quaternion q)
    {
        var p0 = Vector3.zero;
        var p1 = new Vector3(kArrowWidth, 0, kArrowHeight / 2);
        var p2 = new Vector3(kArrowWidth, 0, -kArrowHeight / 2);

        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        GL.Vertex(q * p0 + p);
        GL.Color(color);
        GL.Vertex(q * p1 + p);
        GL.Color(color);
        GL.Vertex(q * p2 + p);
        GL.End();
    }

    void DrawRightArrow(Vector3 p, Color color, Quaternion q)
    {
        var p0 = Vector3.zero;
        var p1 = new Vector3(-kArrowWidth, 0, kArrowHeight / 2);
        var p2 = new Vector3(-kArrowWidth, 0, -kArrowHeight / 2);

        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        GL.Vertex(q * p0 + p);
        GL.Color(color);
        GL.Vertex(q * p1 + p);
        GL.Color(color);
        GL.Vertex(q * p2 + p);
        GL.End();
    }

    void DrawLine(Vector3 sp, Vector3 ep, Quaternion q,
                    Color sc, Color ec)
    {
        var l1 = q * new Vector3(0, 0, -kLineWidth / 2);
        var l2 = q * new Vector3(0, 0, kLineWidth / 2);

        var p0 = l1 + sp;
        var p1 = l2 + sp;
        var p2 = l1 + ep;
        var p3 = l2 + ep;

        GL.Begin(GL.TRIANGLES);
        GL.Color(sc);
        GL.Vertex(p0);
        GL.Vertex(p1);
        GL.Color(ec);
        GL.Vertex(p2);

        GL.Color(sc);
        GL.Vertex(p1);
        GL.Color(ec);
        GL.Vertex(p2);
        GL.Vertex(p3);
        GL.End();
    }

    public void Refresh()
    {
        var tps = GetComponentsInChildren<TrunkPoint>();
        foreach(var tp in tps)
        {
            tp.RouteValidTest();
            tp.RouteRefresh();
        }
    }

#endif
    /// <summary>
    /// 重新设置路点
    /// </summary>
    public void RefreshDijkstra()
    {
        dijkstra_.Clear();
        results_.Clear();

        var tps = GetComponentsInChildren<TrunkPoint>();
        foreach(var tp1 in tps)
        {
            tp1.RouteRefresh();

            var tpps = tp1.points;
            foreach(var tp2 in tpps)
                dijkstra_.SetDist(tp1, tp2, 
                    (tp1.transform.position - tp2.transform.position).magnitude, false);
        }
    }
    /// <summary>
    /// 获得路径点
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">终结点</param>
    /// <returns></returns>
    public List<TrunkPoint> GetPath(TrunkPoint start, TrunkPoint end)
    {
        if(start == null || end == null)
            return null;

        Dijkstra<TrunkPoint>.Result result = null;
        if(!results_.TryGetValue(start, out result))
        {
            result = dijkstra_.Emit(start);
            results_[start] = result;
        }
        return result.GetRoad(end);
    }

    /// <summary>
    /// 寻路算法类
    /// </summary>
    Dijkstra<TrunkPoint> dijkstra_ = new Dijkstra<TrunkPoint>();
    /// <summary>
    /// 路径结果
    /// </summary>
    Dictionary<TrunkPoint, Dijkstra<TrunkPoint>.Result> results_ = 
                        new Dictionary<TrunkPoint,Dijkstra<TrunkPoint>.Result>();
}
