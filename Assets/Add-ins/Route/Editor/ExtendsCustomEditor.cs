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
using UnityEditor;

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

public static class RouteTimeDrawer
{
    [MenuItem("Route/Extends/Create Route Time")]
    static void Create()
    {
        GameObject obj = new GameObject("RouteTime");
        obj.AddComponent<RouteTime>();
        EditorGUIUtility.PingObject(obj);
        Selection.objects = new GameObject[] { obj };
        SceneView.lastActiveSceneView.LookAt(obj.transform.position);
    }
}
