using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[ExecuteInEditMode]
public class TrunkPoint : MonoBehaviour
{
#if UNITY_EDITOR
    void Update()
    {
        UpdateColorSet();
    }

    public static readonly float kRingRadius1 = 0.6f;
    public static readonly float kRingRadius2 = 0.8f;
    public static readonly float kRingHeight = 0.05f;
    public static readonly float kIncircleRadius = 0.4f;
    public static readonly float kIncircleHeight = 0.05f;
    public static readonly int kCirclePartNum = 32;

    public static Material material 
    {
        get
        {
            if(material_ == null)
            {
                material_ = new Material(Shader.Find("__Trunk Point Material__"));
                material_.hideFlags = HideFlags.HideAndDontSave;
                material_.shader.hideFlags = HideFlags.HideAndDontSave;
            }
            return material_;
        }
    }
    static Material material_ = null;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Gizmos.DrawSphere(transform.position, kIncircleRadius);

        material.SetPass(0);

        GL.PushMatrix();
        GL.Begin(GL.TRIANGLE_STRIP);

        for(int i = 0; i < kCirclePartNum + 1; ++i)
        {
            var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            GL.Color(color);
            GL.Vertex(transform.position + new Vector3(cos * kRingRadius2, 0, 
                                                        sin * kRingRadius2));
            GL.Color(color);
            GL.Vertex(transform.position + new Vector3(cos * kRingRadius1, 0, 
                                                        sin * kRingRadius1));
        }
        GL.End();

        if(kRingHeight > 0)
        {
            GL.Begin(GL.TRIANGLE_STRIP);
            for(int i = 0; i < kCirclePartNum + 1; ++i)
            {
                var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kRingRadius2, kRingHeight,
                                                            sin * kRingRadius2));
                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kRingRadius1, kRingHeight,
                                                            sin * kRingRadius1));
            }
            GL.End();

            GL.Begin(GL.TRIANGLE_STRIP);
            for(int i = 0; i < kCirclePartNum + 1; ++i)
            {
                var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kRingRadius2, kRingHeight,
                                                            sin * kRingRadius2));
                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kRingRadius2, 0,
                                                            sin * kRingRadius2));
            }
            GL.End();
        }

        GL.Begin(GL.TRIANGLE_STRIP);
        for(int i = 0; i < kCirclePartNum + 1; ++i)
        {
            var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            GL.Color(color);
            GL.Vertex(transform.position);
            GL.Color(color);
            GL.Vertex(transform.position + new Vector3(cos * kIncircleRadius, 0,
                                                        sin * kIncircleRadius));
        }
        GL.End();

        if(kIncircleHeight > 0)
        {
            GL.Begin(GL.TRIANGLE_STRIP);
            for(int i = 0; i < kCirclePartNum + 1; ++i)
            {
                var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(0, kIncircleHeight, 0));
                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kIncircleRadius, kIncircleHeight,
                                                            sin * kIncircleRadius));
            }
            GL.End();

            GL.Begin(GL.TRIANGLE_STRIP);
            for(int i = 0; i < kCirclePartNum + 1; ++i)
            {
                var angle = (360.0f * i / kCirclePartNum) * Mathf.Deg2Rad;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);

                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kIncircleRadius, 0,
                                                            sin * kIncircleRadius));
                GL.Color(color);
                GL.Vertex(transform.position + new Vector3(cos * kIncircleRadius, kIncircleHeight,
                                                            sin * kIncircleRadius));
            }
            GL.End();
        }
        GL.PopMatrix();
    }

    void UpdateColorSet()
    {
        if(colorset == ColorSet.kNone)
            return;
        color = color_array[(int)colorset];
        colorset = ColorSet.kNone;
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

    /// <summary>
    /// 创建或者获得特殊路径（非运行期函数）
    /// </summary>
    /// <param name="next"></param>
    public RouteConfig RouteBuildOrGet(TrunkPoint next)
    {
        if(!ArrayUtility.Contains<TrunkPoint>(points, next))
            return null;

        RouteRefresh();

        RouteConfig config = null;
        if(trunk_route.TryGetValue(next, out config))
        {
            config.name = name + "<->" + next.name;
            return config;
        }
        if(trunk_prev_route.TryGetValue(next, out config))
        {
            config.name = next.name + "<->" + name;
            return config;
        }

        //创建路径
        GameObject o = new GameObject(name + "<->" + next.name);
        config = o.AddComponent<RouteConfig>();
        ArrayUtility.Add(ref configs, config);
        ArrayUtility.Add(ref next.prev_configs, config);

        o.transform.position = transform.position;
        o.transform.SetParent(transform);

        GameObject r1 = new GameObject("self");
        RoutePoint rp1 = r1.AddComponent<RoutePoint>();
        rp1.transform.position = transform.position;
        rp1.transform.SetParent(o.transform);
        Appendage a1 = r1.AddComponent<Appendage>();
        a1.target = gameObject;

        GameObject r2 = new GameObject("next");
        RoutePoint rp2 = r2.AddComponent<RoutePoint>();
        rp2.transform.position = next.transform.position;
        rp2.transform.SetParent(o.transform);
        Appendage a2 = r2.AddComponent<Appendage>();
        a2.target = next.gameObject;

        var delta = (rp2.transform.position - rp1.transform.position).normalized * 2;
        rp1.prev_weight_point = -delta;
        rp2.prev_weight_point = -delta;
        rp1.next_weight_point = delta;
        rp2.next_weight_point = delta;

        config.Refresh();
        return config;
    }

    /// <summary>
    /// 删除路径
    /// </summary>
    /// <param name="next"></param>
    public void RouteDelete(TrunkPoint next)
    {
        if(!ArrayUtility.Contains<TrunkPoint>(points, next))
            return;

        RouteRefresh();
        RouteConfig config = null;
        if(!trunk_route.TryGetValue(next, out config) && 
            !trunk_prev_route.TryGetValue(next, out config))
            return ;

        ArrayUtility.Remove(ref configs, config);
        ArrayUtility.Remove(ref prev_configs, config);
        ArrayUtility.Remove(ref next.configs, config);
        ArrayUtility.Remove(ref next.prev_configs, config);

        config.gameObject.SetActive(false);
        GameObject.DestroyImmediate(config.gameObject);
    }

    public void RouteValidTest()
    {
        if(!battle_route)
            battle_route = null;

        //发现无效点
        foreach(var p in points)
        {
            if(p == null)
                //发现无效点
                goto DO_VALID;
        }
        return;

DO_VALID:
        List<TrunkPoint> list = new List<TrunkPoint>();
        foreach(var p in points)
        {
            if(p != null)
                list.Add(p);
        }
        points = list.ToArray();
    }

    /// <summary>
    /// 战场路点创建获取
    /// </summary>
    public RouteConfig BattleRouteBuildOrGet()
    {
        if(battle_route)
        {
            battle_route.gameObject.SetActive(true);
            return battle_route;
        }

        //创建路径
        GameObject o = new GameObject("battlefield");
        battle_route = o.AddComponent<RouteConfig>();

        o.transform.position = transform.position;
        o.transform.SetParent(transform);

        GameObject r1 = new GameObject("battle");
        RoutePoint rp1 = r1.AddComponent<RoutePoint>();
        rp1.transform.position = transform.position;
        rp1.transform.SetParent(o.transform);
        rp1.message = "site";

        return battle_route;
    }
#endif

    public void RouteRefresh()
    {
        trunk_route.Clear();
        trunk_prev_route.Clear();

        //设置主动路径
        foreach(var c in configs)
        {
            c.gameObject.SetActive(true);
            c.Refresh();
            c.gameObject.SetActive(false);

            var appendages = c.GetComponentsInChildren<Appendage>(true);
            foreach(var a in appendages)
            {
                if(a.target == this || a.target == null)
                    continue;
                var tp = a.target.GetComponent<TrunkPoint>();
                if(tp)
                    trunk_route[tp] = c;
            }
        }
        //设置被动路径
        foreach(var c in prev_configs)
        {
            trunk_prev_route[c.transform.parent.GetComponent<TrunkPoint>()] = c;
        }
    }

    public RouteConfig GetTrunkRoute(TrunkPoint tp)
    {
        RouteConfig rc = null;
        trunk_route.TryGetValue(tp, out rc);
        return rc;
    }

    public RouteConfig GetTrunkPrevRoute(TrunkPoint tp)
    {
        RouteConfig rc = null;
        trunk_prev_route.TryGetValue(tp, out rc);
        return rc;
    }

    ///// <summary>
    ///// 消息名
    ///// </summary>
    //public string message = "";
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
    /// 关联分支点
    /// </summary>
    [SerializeField]
    [HideInInspector]
    public TrunkPoint[] points = new TrunkPoint[0];
    /// <summary>
    /// 主动路径
    /// </summary>
    [SerializeField]
    //[HideInInspector]
    public RouteConfig[] configs = new RouteConfig[0];
    /// <summary>
    /// 被动路径
    /// </summary>
    [SerializeField]
    //[HideInInspector]
    public RouteConfig[] prev_configs = new RouteConfig[0];

    /// <summary>
    /// 战场路径点
    /// </summary>
    [SerializeField]
    //[HideInInspector]
    public RouteConfig battle_route = null;

    /// <summary>
    /// 分支点对应于路径
    /// </summary>
    Dictionary<TrunkPoint, RouteConfig> trunk_route = new Dictionary<TrunkPoint, RouteConfig>();
    /// <summary>
    /// 分支对应于被动路径
    /// </summary>
    Dictionary<TrunkPoint, RouteConfig> trunk_prev_route = new Dictionary<TrunkPoint, RouteConfig>();
}