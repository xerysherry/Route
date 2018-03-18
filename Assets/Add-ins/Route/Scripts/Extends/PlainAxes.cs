using UnityEngine;

/// <summary>
/// 十字对象
/// </summary>
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
