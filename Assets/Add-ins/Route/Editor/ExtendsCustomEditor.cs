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
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }
}

public static class LookAtSomethingDrawer
{
    [MenuItem("Route/Extends/Create LookAtSomething")]
    static void Create()
    {
        GameObject obj = new GameObject("LookAtSomething");
        obj.AddComponent<LookAtSomething>();
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }
}

public static class RouteTrackDrawer
{
    [MenuItem("Route/Extends/Create Route Track")]
    static void Create()
    {
        GameObject obj = new GameObject("RouteTrack");
        obj.AddComponent<RouteTrack>();
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
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
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var g = (target as RouteGenerator);
        if(GUILayout.Button("Build"))
            g.Generate();
        if(GUILayout.Button("Clear"))
            g.Clear();
        base.OnInspectorGUI();
    }
}
