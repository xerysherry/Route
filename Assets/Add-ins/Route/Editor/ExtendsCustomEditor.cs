using UnityEngine;
using UnityEditor;
using System.Collections;

public static class PlainAxesDrawer
{
    [MenuItem("Route/Extends/Create Plain Axes")]
    static void Create()
    {
        GameObject obj = new GameObject("PlainAxes");
        obj.AddComponent<PlainAxes>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }
}

public static class LookAtSomethingDrawer
{
    [MenuItem("Route/Extends/Create LookAtSomething")]
    static void Create()
    {
        GameObject obj = new GameObject("LookAtSomething");
        obj.AddComponent<LookAtSomething>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }
}

[CustomEditor(typeof(RouteTrack))]
public class RouteTrackDrawer : Editor
{
    [MenuItem("Route/Extends/Create Route Track")]
    static void Create()
    {
        GameObject obj = new GameObject("RouteTrack");
        obj.AddComponent<RouteTrack>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var self = (target as RouteTrack);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Total Length(总长度):");
        GUILayout.Label(self.total_length.ToString());
        GUILayout.EndHorizontal();
        if(GUILayout.Button("Refresh(刷新)"))
            self.Refresh();
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(RouteGenerator))]
public class RouteGeneratorDrawer : Editor
{
    [MenuItem("Route/Extends/Create Route Generator")]
    static void Create()
    {
        GameObject obj = new GameObject("RouteGenerator");
        obj.AddComponent<RouteGenerator>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var self = (target as RouteGenerator);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Total Length(总长度):");
        GUILayout.Label(self.total_length.ToString());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Generate Count(生成数量):");
        GUILayout.Label(((int)Mathf.Ceil(self.total_length / self.step)).ToString());
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Build(生成对象)"))
            self.Generate();
        if(GUILayout.Button("Clear(清空对象)"))
            self.Clear();
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(RoutePlacement))]
public class RoutePlacementDrawer : Editor
{
    [MenuItem("Route/Extends/Create Route Placement")]
    static void Create()
    {
        GameObject obj = new GameObject("RoutePlacement");
        obj.AddComponent<RoutePlacement>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var self = (target as RoutePlacement);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Total Length(总长度):");
        GUILayout.Label(self.total_length.ToString());
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Refresh(刷新)"))
            self.Refresh();
        if(GUILayout.Button("Setup(放置)"))
            self.Setup();
        base.OnInspectorGUI();
    }
}
