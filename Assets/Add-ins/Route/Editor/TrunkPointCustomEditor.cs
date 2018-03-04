using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TrunkPoint))]
public class TrunkPointDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        TrunkPoint point = target as TrunkPoint;
        GameObject obj = point.gameObject;

        TrunkConfig config = null;
        while(obj.transform.parent != null)
        {
            config = point.transform.parent.GetComponent<TrunkConfig>();
            if(config)
                break;
            obj = obj.transform.parent.gameObject;
        }
        if(config)
        {
            //隐藏所有路径数据
            var cs = config.GetComponentsInChildren<RouteConfig>();
            foreach(var c in cs)
                c.gameObject.SetActive(false);
            
            LinkPoint(config);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("======================================", GUILayout.Width(300));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal(); 
            //功能区
            if(GUILayout.Button("Battle Field"))
            {
                var c = point.BattleRouteBuildOrGet();
                Select(c.gameObject);
            }
            if(GUILayout.Button("Break All") && point.points.Length > 0)
            {
                if(EditorUtility.DisplayDialog("Warning !!",
                                            "You will Delete All Link And Route !!\n" +
                                            "你将会删除说要链接和配置的路径！！\n" +
                                            "Are you sure !!\n" +
                                            "你确定吗？",
                                            "Yes（是的）", "No（不）"))
                {
                    foreach(var p in point.points)
                    {
                        if(p == null)
                            continue;

                        ArrayUtility.Remove(ref p.points, point);
                    }
                    point.points = new TrunkPoint[0];
                    SceneView.RepaintAll();
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("======================================", GUILayout.Width(300));
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();            
        }
        base.OnInspectorGUI();
    }

    void LinkPoint(TrunkConfig config)
    {
        TrunkPoint point = target as TrunkPoint;
        var tps = config.GetComponentsInChildren<TrunkPoint>();
        var oldcolor = GUI.color;

        foreach(var tp in tps)
        {
            if(tp == point)
                continue;
            EditorGUILayout.BeginHorizontal();
            GUI.color = tp.color;
            if(GUILayout.Button(tp.name))
            {
                Select(tp.gameObject);
            }
            GUI.color = oldcolor;
            if(ArrayUtility.Contains(point.points, tp))
            {
                if(GUILayout.Button("Route", GUILayout.Width(50)))
                {
                    var c = point.RouteBuildOrGet(tp);
                    c.gameObject.SetActive(true);
                    c.Refresh();
                    Select(c.gameObject);
                    SceneView.RepaintAll();
                }
                if(GUILayout.Button("Break", GUILayout.Width(50)))
                {
                    point.RouteDelete(tp);
                    ArrayUtility.Remove(ref point.points, tp);
                    ArrayUtility.Remove(ref tp.points, point);
                    SceneView.RepaintAll();
                }
            }
            else
            {
                if(GUILayout.Button("Link", GUILayout.Width(50)))
                {
                    ArrayUtility.Add(ref point.points, tp);
                    ArrayUtility.Add(ref tp.points, point);
                    SceneView.RepaintAll();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void Select(GameObject obj)
    {
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }

    public void OnSceneGUI()
    { 
        TrunkPoint point = target as TrunkPoint;
        Handles.Label(point.transform.position + new Vector3(0, 0.8f, 0), point.name);
    }
}

[CustomEditor(typeof(TrunkConfig))]
public class TrunkConfigDrawer : Editor 
{
    public override void OnInspectorGUI()
    {
        TrunkConfig config = target as TrunkConfig;
        if(GUILayout.Button("New Trunk"))
        {
            GameObject obj = new GameObject("New Trunk");
            var tp = obj.AddComponent<TrunkPoint>();
            obj.transform.SetParent(config.transform);
            obj.transform.position = config.transform.position;
        }
        if(GUILayout.Button("Refresh"))
            config.Refresh();

        TestPath();

        var tps = config.GetComponentsInChildren<TrunkPoint>();
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

        var oldcolor = GUI.color;
        foreach(var tp in tps)
        {
            var ts1 = td[tp];
            foreach(var tp2 in ts1)
            {
                var ts2 = td[tp2];

                EditorGUILayout.BeginHorizontal();
                if(ts2.Contains(tp))
                {
                    GUI.color = tp.color;
                    if(GUILayout.Button(tp.name, GUILayout.Width(120)))
                    {
                        Select(tp.gameObject);
                    }
                    GUI.color = oldcolor;
                    EditorGUILayout.LabelField("<-->", GUILayout.Width(32));
                    GUI.color = tp2.color;
                    if(GUILayout.Button(tp2.name, GUILayout.Width(120)))
                    {
                        Select(tp2.gameObject);
                    }
                    GUI.color = oldcolor;
                    ts2.Remove(tp);
                }
                else
                {
                    GUI.color = tp.color;
                    if(GUILayout.Button(tp.name))
                    {
                        Select(tp.gameObject);
                    }
                    GUI.color = oldcolor;
                    EditorGUILayout.LabelField("--->", GUILayout.Width(32));
                    GUI.color = tp2.color;
                    if(GUILayout.Button(tp2.name))
                    {
                        Select(tp2.gameObject);
                    }
                    GUI.color = oldcolor;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        base.OnInspectorGUI();
    }

    void TestPath()
    {
        EditorGUILayout.BeginHorizontal();
        var new_start = EditorGUILayout.ObjectField(test_start, typeof(TrunkPoint), true, null) as TrunkPoint;
        EditorGUILayout.LabelField("--->", GUILayout.Width(32));
        var new_end = EditorGUILayout.ObjectField(test_end, typeof(TrunkPoint), true, null) as TrunkPoint;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        if(test_start != new_start || test_end != new_end)
        {
            test_start = new_start;
            test_end = new_end;

            TrunkConfig config = target as TrunkConfig;
            config.RefreshDijkstra();
            road = config.GetPath(test_start, test_end);
            SceneView.RepaintAll();
        }
    }
    TrunkPoint test_start, test_end;
    List<TrunkPoint> road;

    void Select(GameObject obj)
    {
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }

    //public void OnSceneGUI()
    //{
    //    if(road == null)
    //        return;

    //    Handles.color = Color.black;
    //    for(int i = 0; i < road.Count - 1; ++i)
    //    {
    //        var prev = road[i].transform.position;
    //        var next = road[i + 1].transform.position;
    //        Handles.DrawLine(prev, next);
    //        Handles.CylinderCap(0, prev, Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), 0.5f);
    //    }
    //    if(road.Count > 0)
    //        Handles.CylinderCap(0, road[road.Count - 1].transform.position,
    //                            Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), 0.5f);
    //}
}