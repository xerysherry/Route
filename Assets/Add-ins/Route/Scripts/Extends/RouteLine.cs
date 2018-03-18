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
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Route/Route Line")]
public class RouteLine : RouteComponent 
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
        Remesh();
	}
	void Update () 
    {}
    public void Remesh()
    {
        RefreshConfig();
        if(!route || route.count == 0)
        {
            filter.mesh = null;
            return;
        }
#if UNITY_EDITOR
        if(!Application.isPlaying)
            route.Refresh();
#endif

        mat.mainTexture = texture;
        renderer.material = mat;

        controller_.Reset();

        float totallength = route.total_length;
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
        float dist = 0, subdist = 0;

        Vector3 pos = transform.position;
        Vector3 A0, A1, B0, B1;
        Vector3 pt, tangent;

        do
        {
            if(mode == Mode.TWIST)
            {
                controller_.SetMove(subdist);
                pt = controller_.GetPoint() - pos;
                tangent = controller_.GetTangent();
                GetABPoint(pt, tangent, out A0, out B0);
                vi = VerticesStart(A0, B0, vi, ref vertices, ref uv);

                if(vi >= 6 && interval == 0.0f)
                {
                    vertices[vi - 2] = vertices[vi - 4];
                    vertices[vi - 1] = vertices[vi - 3];
                }

                subdist += step;
                if(self_adjust)
                {
                    if(subdist > totallength)
                        subdist = totallength;
                }
                else if(subdist > totallength)
                    break;

                controller_.SetMove(subdist);
                pt = controller_.GetPoint() - pos;
                tangent = controller_.GetTangent();
                GetABPoint(pt, tangent, out A1, out B1);
                vi = VerticesNext(A1, B1, vi, ref vertices, ref uv, ref triangles);
            }
            else
            {
                subdist += step / 2;

                if(subdist > totallength)
                    break;

                controller_.SetMove(subdist);
                pt = controller_.GetPoint() - pos;
                tangent = controller_.GetTangent();
                GetABPoint(pt, tangent, out A0, out B0);

                tangent = tangent * step/2;
                vi = VerticesStart(A0 - tangent, B0 - tangent, vi, ref vertices, ref uv);

                A1 = A0 + tangent;
                B1 = B0 + tangent;
                vi = VerticesNext(A1, B1, vi, ref vertices, ref uv, ref triangles);

                subdist += step/2;
            }
            subdist += iv;
            if(vi >= vertices.Count)
                break;
        } while((dist += step) < totallength);

        if(vi % 4 != 0)
        {
            vertices.RemoveRange(vi - 2, 4);
            uv.RemoveRange(vi - 2, 4);
            triangles.RemoveRange(vi / 4 * 6, 6);
        }

        if(mode == Mode.TWIST && route.IsLoop && interval == 0.0f)
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
    void GetABPoint(Vector3 loc, Vector3 tangent,
                    out Vector3 A, out Vector3 B)
    {
        var angle = RouteMath.GetPolarEular(tangent);
        var quat = Quaternion.AngleAxis(angle, RouteMath.kAxisY);
        var iquat = Quaternion.AngleAxis(angle + 180, RouteMath.kAxisY);

        loc += position;
        if(eular == 0)
        {
            A = (quat * RouteMath.kAxisX) * width / 2 + loc;
            B = (iquat * RouteMath.kAxisX) * width / 2 + loc;
        }
        else
        {
            A = (quat * RouteMath.kAxisX) * width / 2;
            B = (iquat * RouteMath.kAxisX) * width / 2;
            A = Quaternion.AngleAxis(eular, tangent) * A + loc;
            B = Quaternion.AngleAxis(eular, tangent) * B + loc;
        }
    }

    int VerticesStart(Vector3 A, Vector3 B, int index,
                    ref List<Vector3> vertices, ref List<Vector2> uv)
    {
        vertices[index] = A;
        vertices[index + 1] = B;

        //uvoffset = (index / 2) % 2;
        uv[index] = kUVs[0];
        uv[index + 1] = kUVs[1];

        if((flip & Flip.VERTICAL) != 0)
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

    int VerticesNext(Vector3 A, Vector3 B, int index,
                    ref List<Vector3> vertices, ref List<Vector2> uv, ref List<int> triangles)
    {
        vertices[index] = A;
        vertices[index + 1] = B;

        if((flip & Flip.VERTICAL) != 0)
        {
            uv[index + 1] = kUVs[2];
            uv[index] = kUVs[3];
        }
        else
        {
            uv[index] = kUVs[2];
            uv[index + 1] = kUVs[3];
        }
        if( //水平翻转
            (flip & Flip.HORIZON) != 0 ||
            //镜像模式
            ((flip & Flip.MIRROR) != 0 && (index / 4) % 2 == 1))
        {
            var t = uv[index - 2];
            uv[index - 2] = uv[index];
            uv[index] = t;
            t = uv[index - 1];
            uv[index - 1] = uv[index + 1];
            uv[index + 1] = t;
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

    /// <summary>
    /// 纹理
    /// </summary>
    public Texture texture;
    /// <summary>
    /// 材质（Null为空时，生成时会创建"Sprites/Default"材质）
    /// </summary>
    public Material material;
    /// <summary>
    /// 生成模式
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// 扭曲
        /// </summary>
        TWIST,
        /// <summary>
        /// 矩形
        /// </summary>
        RECTANGLE,
    }
    /// <summary>
    /// 模式
    /// </summary>
    public Mode mode;
    /// <summary>
    /// 翻转模式
    /// </summary>
    public enum Flip
    {
        NONE = 0,
        HORIZON = 1 << 1,
        VERTICAL = 1 << 2,
        MIRROR = 1 << 3,
        HORIZON_AND_VERTICAL = HORIZON | VERTICAL,
        MIRROR_AND_VERTICAL = MIRROR | VERTICAL,
    }
    /// <summary>
    /// 翻转
    /// </summary>
    public Flip flip = Flip.NONE;
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
    /// 附加偏移
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// 绕切线旋转角
    /// </summary>
    public float eular = 0;
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
#if UNITY_2017
    new MeshRenderer renderer
#else
    MeshRenderer renderer
#endif
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
    Material mat
    {
        get
        {
            if(!material)
            {
                material = new Material(Shader.Find("Sprites/Default"));
                material.color = Color.white;
            }
            return material;
        }
    }
}
