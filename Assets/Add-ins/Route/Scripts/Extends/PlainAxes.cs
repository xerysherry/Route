using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 十字对象
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Route/Plain Axes")]
public class PlainAxes : MonoBehaviour
{
    static readonly float kOSize = 0.025f;
    static readonly float kAxesSize = 0.3f;
#if UNITY_EDITOR
    void Update()
    {
        if(colorset == ColorSet.kNone)
            return;
        color = color_array[(int)colorset];
        colorset = ColorSet.kNone;
    }

    void OnDrawGizmos()
    {
        var rot = transform.rotation;
        var p = transform.position;
        var s = transform.lossyScale.x * kAxesSize;
        var v = rot * Vector3.up * s;
        Gizmos.color = color;
        Gizmos.DrawRay(p, v);
        Gizmos.DrawRay(p, -v);
        v = rot * Vector3.forward * s;
        Gizmos.DrawRay(p, v);
        Gizmos.DrawRay(p, -v);
        v = rot * Vector3.right * s;
        Gizmos.DrawRay(p, v);
        Gizmos.DrawRay(p, -v);

        Gizmos.DrawSphere(p, kOSize);
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
#endif
}
