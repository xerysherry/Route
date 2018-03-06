using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RouteLine : MonoBehaviour 
{
    static readonly Vector2[] kUVs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1),
    };

    void Start () 
    {
        Remesh2();
	}
	
	void Update () 
    {
        //Remesh();
	}

    //void Remesh()
    //{
    //    route_.Refresh();
    //    renderer.material.mainTexture = texture;
    //
    //    float totallength = 0;
    //    for(int i = 0; i < route_.count; ++i)
    //    {
    //        route_.SetCurrent(i);
    //        totallength += route_.current_length;
    //    }
    //    var scount = (int)Mathf.Floor(totallength / step);
    //    Vector3[] vertices = new Vector3[(scount + 1) * 2];
    //    Vector2[] uv = new Vector2[(scount + 1) * 2];
    //    int[] triangles = new int[scount * 6];
    //
    //    int vi = 0;
    //    float dist = 0, subdist = 0, nt = 0;
    //    route_.SetCurrent(0);
    //
    //    Vector3 A, B;
    //    var pt = route_.current_point.transform.position;
    //    var tangent = route_.current_point.next_weight_point;
    //    var polar = MathUtils.GetPolarEular(tangent);
    //    GetABPoint(pt, polar, out A, out B);
    //    vi = VerticesStart(A, B, vi, ref vertices, ref uv);
    //
    //    do
    //    {
    //        subdist += step;
    //        while(subdist > route_.current_length)
    //        {
    //            subdist -= route_.current_length;
    //            if(route_.current+1 >= route_.count)
    //                goto EXIT;
    //            route_.SetCurrent(route_.current + 1);
    //        }
    //
    //        nt = route_.GetNormalizedT(subdist);
    //        pt = route_.GetPoint(nt);
    //        tangent = route_.GetTangent(nt);
    //        polar = MathUtils.GetPolarEular(tangent);
    //        GetABPoint(pt, polar, out A, out B);
    //        vi = VerticesNext(A, B, vi, 
    //                        ref vertices, ref uv, ref triangles);
    //        if(vi >= vertices.Length)
    //            break;
    //    } while((dist += step) < totallength);
    //
    //EXIT:
    //    Mesh mesh = new Mesh();
    //    mesh.vertices = vertices;
    //    mesh.uv = uv;
    //    mesh.triangles = triangles;
    //    filter.mesh = mesh;
    //
    //    return;
    //}

    public void Remesh2()
    {
        if(route == null)
            route = gameObject.GetComponent<RouteConfig>();
        if(!route || route.count == 0)
        {
            filter.mesh = null;
            return;
        }
        route.Refresh();

        material.mainTexture = texture;
        renderer.material = material;

        route.SetCurrent(0);

        float totallength = route.current_length;
        while(!route.IsTail())
        {
            route.Next();
            totallength += route.current_length;   
        }
        var scount = (int)Mathf.Floor((totallength + interval) / (step + interval));
        if(scount == 0)
            return;

        var iv = interval;
        if(self_adjust)
        {
            if(route.IsLoop)
                iv = (totallength - scount * (step + interval)) / scount + interval;
            else
                iv = (totallength + interval - scount * (step + interval)) / scount + interval;
        }

        List<Vector3> vertices = new List<Vector3>(new Vector3[(scount + 1) * 4]);
        List<Vector2> uv = new List<Vector2>(new Vector2[(scount + 1) * 4]);
        List<int> triangles = new List<int>(new int[(scount + 1) * 6]);

        int vi = 0;
        float dist = 0, subdist = 0, nt = 0;
        route.SetCurrent(0);

        Vector3 pos = transform.position;
        Vector3 A0, A1, B0, B1;
        Vector3 pt, tangent;
        float polar = 0;

        do
        {
            nt = route.GetNormalizedT(subdist);
            pt = route.GetPoint(nt) - pos;
            tangent = route.GetTangent(nt);
            polar = RouteMath.GetPolarEular(tangent);
            GetABPoint(pt, polar, out A0, out B0);
            vi = VerticesStart2(A0, B0, vi, ref vertices, ref uv);

            if(vi > 6 && interval == 0.0f)
            {
                vertices[vi - 2] = vertices[vi - 4];
                vertices[vi - 1] = vertices[vi - 3];    
                uv[vi - 2] = uv[vi - 6];
                uv[vi - 1] = uv[vi - 5];
            }

            subdist += step;
            while(subdist > route.current_length)
            {
                subdist -= route.current_length;
                if(route.IsTail())
                    goto EXIT;
                route.Next();
            }

            nt = route.GetNormalizedT(subdist);
            pt = route.GetPoint(nt) - pos;
            tangent = route.GetTangent(nt);
            polar = RouteMath.GetPolarEular(tangent);
            GetABPoint(pt, polar, out A1, out B1);
            vi = VerticesNext2(A1, B1, vi, ref vertices, ref uv, ref triangles);

            subdist += iv;
            while(subdist > route.current_length)
            {
                subdist -= route.current_length;
                if(route.IsTail())
                    goto EXIT;
                route.Next();
            }

            if(vi >= vertices.Count)
                break;
        } while((dist += step) < totallength);

    EXIT:
        if(vi % 4 != 0)
        {
            vertices.RemoveRange(vi - 2, 4);
            uv.RemoveRange(vi - 2, 4);
            triangles.RemoveRange(vi / 4 * 6, 6);
        }

        if(route.IsLoop && interval == 0.0f)
        {
            vertices[vertices.Count - 2] = vertices[0];
            vertices[vertices.Count - 1] = vertices[1];
            uv[uv.Count - 2] = uv[2];
            uv[uv.Count - 1] = uv[3];
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        filter.mesh = mesh;

        return;
    }

    /// <summary>
    /// A0        A1        A2   ...
    /// +---------+---------+---
    /// |         |         |
    /// +---------+---------+---
    /// B0        B1        B2   ...
    /// </summary>
    void GetABPoint(Vector3 loc, float angle,
                    out Vector3 A, out Vector3 B)
    {
        var quat = Quaternion.AngleAxis(angle, RouteMath.kAxisY);
        var iquat = Quaternion.AngleAxis(angle + 180, RouteMath.kAxisY);
        A = (quat * RouteMath.kAxisX) * width / 2 + loc;
        B = (iquat * RouteMath.kAxisX) * width / 2 + loc;
    }

    //int VerticesStart(Vector3 A, Vector3 B, int index,
    //                ref Vector3[] vertices, ref Vector2[] uv)
    //{
    //    vertices[index] = A;
    //    vertices[index + 1] = B;
    //
    //    uvoffset = (index / 2) % 2;
    //    uv[index] = kUVs[0];
    //    uv[index+1] = kUVs[1];
    //    return index + 2;
    //}
    //
    //int VerticesNext(Vector3 A, Vector3 B, int index,
    //                ref Vector3[] vertices, ref Vector2[] uv, ref int[] triangles)
    //{
    //    vertices[index] = A;
    //    vertices[index + 1] = B;
    //
    //    var uvi = ((index / 2 + uvoffset) % 2) * 2;
    //    uv[index] = kUVs[uvi];
    //    uv[index+1] = kUVs[uvi + 1];
    //
    //    var ti = (index / 2 - 1) * 6;
    //    triangles[ti] = index - 2;
    //    triangles[ti + 1] = index - 1;
    //    triangles[ti + 2] = index;
    //    triangles[ti + 3] = index + 1;
    //    triangles[ti + 4] = index;
    //    triangles[ti + 5] = index - 1;
    //
    //    return index + 2;
    //}

    int VerticesStart2(Vector3 A, Vector3 B, int index,
                    ref List<Vector3> vertices, ref List<Vector2> uv)
    {
        vertices[index] = A;
        vertices[index + 1] = B;

        uvoffset = (index / 2) % 2;
        uv[index] = kUVs[0];
        uv[index + 1] = kUVs[1];

        if(flip)
        {
            uv[index + 1] = kUVs[0];
            uv[index] = kUVs[1];
        }
        else
        {
            uv[index] = kUVs[0];
            uv[index + 1] = kUVs[1];
        }
        return index + 2;
    }

    int VerticesNext2(Vector3 A, Vector3 B, int index,
                    ref List<Vector3> vertices, ref List<Vector2> uv, ref List<int> triangles)
    {
        vertices[index] = A;
        vertices[index + 1] = B;

        var uvi = ((index / 2 + uvoffset) % 2) * 2;
        if(flip)
        {
            uv[index + 1] = kUVs[uvi];
            uv[index] = kUVs[uvi + 1];
        }
        else
        {
            uv[index] = kUVs[uvi];
            uv[index + 1] = kUVs[uvi + 1];
        }

        var ti = ((index-2) / 4 ) * 6;
        triangles[ti] = index - 2;
        triangles[ti + 1] = index - 1;
        triangles[ti + 2] = index;
        triangles[ti + 3] = index + 1;
        triangles[ti + 4] = index;
        triangles[ti + 5] = index - 1;

        return index + 2;
    }

    public RouteConfig route;
    /// <summary>
    /// 直线纹理
    /// </summary>
    public Texture texture;
    /// <summary>
    /// 步长
    /// </summary>
    public float step = 1;
    /// <summary>
    /// 宽度
    /// </summary>
    public float width = 1;
    /// <summary>
    /// 间隔
    /// </summary>
    public float interval = 0;
    /// <summary>
    /// 翻转
    /// </summary>
    public bool flip = false;
    /// <summary>
    /// 自适应
    /// </summary>
    public bool self_adjust = true;
    
    MeshFilter filter_;
    MeshFilter filter
    {
        get
        {
            if(!filter_)
            {
                filter_ = gameObject.GetComponent<MeshFilter>();
                if(filter_ == null)
                    filter_ = gameObject.AddComponent<MeshFilter>();
            }
            return filter_;
        }
    }

    MeshRenderer renderer_;
    MeshRenderer renderer
    {
        get
        {
            if(!renderer_)
            {
                renderer_ = gameObject.GetComponent<MeshRenderer>();
                if(!renderer_)
                    renderer_ = gameObject.AddComponent<MeshRenderer>();
            }
            return renderer_;
        }
    }

    Material material
    {
        get
        {
            if(!material_)
            {
                material_ = new Material(Shader.Find("Sprites/Default"));
                material_.color = Color.white;
            }
            return material_;
        }
    }
    Material material_;

    int uvoffset = 0;
}
