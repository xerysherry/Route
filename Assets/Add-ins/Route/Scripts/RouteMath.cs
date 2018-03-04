using UnityEngine;

public static class RouteMath
{
    public static readonly Vector3 kAxisX = new Vector3(1, 0, 0);
    public static readonly Vector3 kAxisY = new Vector3(0, 1, 0);

    //获得极坐标角度（Vector3仅使用x, z变量）
    public static float GetPolarEular(Vector3 p)
    {
        var x = p.z;
        var y = p.x;

        float rad = 0;
        if(x > 0)
            rad = Mathf.Atan(y / x);
        else if(x < 0 && y >= 0)
            rad = Mathf.Atan(y / x) + Mathf.PI;
        else if(x < 0 && y < 0)
            rad = Mathf.Atan(y / x) - Mathf.PI;
        else if(x == 0 && y > 0)
            rad = Mathf.PI / 2;
        else if(x == 0 && y < 0)
            rad = -Mathf.PI / 2;
        return Mathf.Rad2Deg * rad;
    }

    /// <summary>
    /// Bezier曲线
    /// </summary>
    public class Bezier3D
    {
        public Bezier3D()
        { }
        public Bezier3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Set(p0, p1, p2, p3);
        }
        public void Set(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
        public Vector3 Get(float t)
        {
            float mt1 = 1 - t;
            float mt2 = mt1 * mt1;
            float mt3 = mt2 * mt1;
            float t1 = Mathf.Clamp(t, 0, 1);
            float t2 = t1 * t1;
            float t3 = t2 * t1;

            return mt3 * p0 + 3 * p1 * t1 * mt2 +
                3 * p2 * t2 * mt1 + p3 * t3;
        }
        public Vector3 GetTangent(float t)
        {
            float t1 = Mathf.Clamp(t, 0, 1);
            float t2 = t1 * t1;

            return (p0 * (-3 + 6 * t1 - 3 * t2) +
                    3 * p1 * (1 - 4 * t1 + 3 * t2) +
                    3 * p2 * (2 * t1 - 3 * t2) +
                    3 * p3 * t2).normalized;
        }
        Vector3 p0 = Vector3.zero,
                p1 = Vector3.zero,
                p2 = Vector3.zero,
                p3 = Vector3.zero;
    }
}
