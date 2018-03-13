using UnityEngine;
using UnityEditor;
using System.Collections;

public static class PlainAxesDrawer
{
    [MenuItem("Route/Extends/Create Plain Axes")]
    static void CreateRouteConfig()
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
    static void CreateRouteConfig()
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
    static void CreateRouteConfig()
    {
        GameObject obj = new GameObject("LookAtSomething");
        obj.AddComponent<LookAtSomething>();
        Selection.objects = new GameObject[] { obj };
        SceneView.currentDrawingSceneView.LookAt(obj.transform.position);
    }
}