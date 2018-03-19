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

[AddComponentMenu("Route/Route Time")]
public class RouteTime : RouteComponent
{
	void Start ()
    {
        RefreshConfig();
        controller_.SetRotateEnable(rotation);
        controller_.Update(start_time);
	}
	
	void Update ()
    {
        if(obj == null)
            return;
        controller_.SetRotateEnable(rotation);
        controller_.Update(delta, obj);
        if(eular != Vector3.zero)
            obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(eular);
        if(position != Vector3.zero)
            obj.transform.position = obj.transform.position + position;
    }

    float delta
    {
        get
        {
            float dt = 0;
            switch(mode)
            {
            case Mode.DELTA_TIME:
                dt = Time.deltaTime;
                break;
            case Mode.FIXED_DELTA_TIME:
                dt = Time.fixedDeltaTime;
                break;
#if UNITY_2017
            case Mode.FIXED_UNSCALED_DELTA_TIME:
                dt = Time.fixedUnscaledDeltaTime;
                break;
#endif
            case Mode.CUSTOM_TIME:
                dt = custom_delta;
                break;
            }
            if(negative)
                dt = -dt;
            return dt;
        }
    }

    /// <summary>
    /// 轨迹对象
    /// </summary>
    public GameObject obj;
    /// <summary>
    /// 面向曲线切线方向
    /// </summary>
    public bool rotation = true;
    /// <summary>
    /// 是否反向
    /// </summary>
    public bool negative = false;
    /// <summary>
    /// 开始时间偏移
    /// </summary>
    public float start_time = 0;
    public enum Mode
    {
        /// <summary>
        /// Time.deltaTime
        /// </summary>
        DELTA_TIME = 1,
        /// <summary>
        /// Time.fixedDeltaTime
        /// </summary>
        FIXED_DELTA_TIME = 2,
#if UNITY_2017
        /// <summary>
        /// Time.fixedUnscaledDeltaTime
        /// </summary>
        FIXED_UNSCALED_DELTA_TIME = 3,
#endif
        /// <summary>
        /// 自定义差量
        /// </summary>
        CUSTOM_TIME = 4,
    }
    public Mode mode = Mode.DELTA_TIME;
    /// <summary>
    /// 自定义差量
    /// </summary>
    public float custom_delta = 0;
    /// <summary>
    /// 附加偏移
    /// </summary>
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// 附加旋转
    /// </summary>
    public Vector3 eular = Vector3.zero;
}
