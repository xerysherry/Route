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
using Bezier3D = RouteMath.Bezier3D;

public class RouteController
{
    public delegate void OnMessageFunc(string message);
    public delegate void OnEnterPointFunc(RoutePoint pt);
    public delegate void OnExitPointFunc(RoutePoint pt);

    public RouteController()
    {
        on_message = msg => { };
        on_enter_point = pt => { };
        on_exit_point = pt => { };
    }
    /// <summary>
    /// 设置路点配置
    /// </summary>
    /// <param name="config"></param>
    public void SetRouteConfig(RouteConfig config)
    {
        Clear();
        route_config_ = config;
        Reset();
    }
    /// <summary>
    /// 是否旋转
    /// </summary>
    /// <param name="rotate"></param>
    public void SetRotateEnable(bool rotate)
    {
        rotate_ = rotate;
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        is_finish_ = false;
        move_length_ = 0;
        wait_time_ = 0;
        if(route_config_ != null)
        {
            current_ = 0;
            bezier_ = route_config_.GetBezier3D(current_);
        }
    }
    /// <summary>
    /// 清理数据
    /// </summary>
    public void Clear()
    {
        route_config_ = null;
        move_length_ = 0;
    }

    public void Update(float dt)
    {
        if(is_finish_)
            return;

        if(dt > 0)
        {
            //正向
            if(wait_time_ <= 0)
                wait_time_ = current_point.keeptime;
            //等待时间
            wait_time_ += dt;

            if(wait_time_ <= current_point.keeptime)
                return;
            dt = wait_time_ - current_point.keeptime;
            wait_time_ = current_point.keeptime;

            float velocity = GetCurrenVelocity();

            move_length_ += velocity * dt;
            while(move_length_ > current_length)
            {
                move_length_ -= current_length;
                OnExitPoint(current_point);
                OnEnterPoint(next_point);

                //进入下一路点
                var nextindex = route_config.GetNextIndex(current_);
                if(nextindex != current_)
                    SetCurrent(nextindex);
                else
                    is_finish_ = true;
                //是否完成
                if(is_finish_)
                {
                    move_length_ = current_length;
                    break;
                }

                //计算等待时间
                wait_time_ = move_length_ / velocity;
                if(wait_time_ > current_point.keeptime)
                {
                    velocity = GetCurrenVelocity();
                    move_length_ = (wait_time_ - current_point.keeptime) * velocity;
                }
                else
                {
                    move_length_ = 0;
                    break;
                }
            }
        }
        else
        {
            //反向
            if(wait_time_ >= 0)
                wait_time_ = -next_point.keeptime;
            //等待时间
            wait_time_ += dt;

            if(wait_time_ >= -next_point.keeptime)
                return;
            dt = wait_time_ + next_point.keeptime;
            wait_time_ = -next_point.keeptime;

            float velocity = GetCurrenVelocity();
            //float totalt = current_length / velocity;

            move_length_ += velocity * dt;
            while(move_length_ < 0)
            {
                move_length_ += prev_length;
                OnExitPoint(next_point);
                OnEnterPoint(current_point);

                //计算等待时间
                velocity = GetPrevVelocity();
                wait_time_ = -move_length_ / velocity;
                if(wait_time_ < -current_point.keeptime)
                {
                    move_length_ = (current_point.keeptime - wait_time_) * velocity;
                    wait_time_ = 0;
                }
                else
                {
                    move_length_ = current_length;
                    break;
                }

                //进入下一路点
                var previndex = route_config.GetPrevIndex(current_);
                if(previndex != current_)
                    SetCurrent(previndex);
                else
                    is_finish_ = true;
                //是否完成
                if(is_finish_)
                {
                    move_length_ = 0;
                    break;
                }
                
            }
        }
    }

    public void Update(float dt, GameObject obj)
    {
        Update(dt);
        Apply(obj);
    }

    public void SetIsFinish(bool value)
    {
        is_finish_ = value;
    }

    public void SetMove(float length)
    {
        if(prev_total_length <= length && length <= curr_total_length)
        {
            move_length_ = length - prev_total_length;
        }
        else
        {
            Reset();
            Step(length);
        }
    }

    public void Step(float delta_length)
    {
        move_length_ += delta_length;
        if(delta_length > 0)
        {
            while(move_length_ >= current_length)
            {
                move_length_ -= current_length;

                //进入下一路点
                var nextindex = route_config.GetNextIndex(current_);
                if(nextindex != current_)
                {
                    OnExitPoint(current_point);
                    SetCurrent(nextindex);
                    OnEnterPoint(current_point);
                }
                else
                {
                    move_length_ = current_length;
                    if(!is_finish_)
                    {
                        OnExitPoint(current_point);
                        OnEnterPoint(next_point);
                        is_finish_ = true;
                    }
                    break;
                }
            }
        }
        else
        {
            while(move_length_ < 0)
            {
                move_length_ += prev_length;

                var previndex = route_config.GetPrevIndex(current_);
                if(previndex != current)
                {
                    OnExitPoint(current_point);
                    SetCurrent(previndex);
                    OnEnterPoint(current_point);
                }
                else
                {
                    move_length_ = 0;
                    if(!is_finish_)
                    {
                        OnExitPoint(current_point);
                        OnEnterPoint(prev_point);
                        is_finish_ = true;
                    }
                }
            }
        }
    }

    public void Next()
    {
        on_exit_point(current_point);
        if(!is_finish)
        {
            //进入下一路点
            var nextindex = route_config.GetNextIndex(current_);
            if(nextindex != current_)
                SetCurrent(nextindex);
            else
            {
                is_finish_ = true;
                return;
            }

            wait_time_ = 0;
            move_length_ = 0;

            on_enter_point(current_point);
            if(!string.IsNullOrEmpty(current_point.message))
                on_message(current_point.message);
        }
        else
            move_length_ = current_length;
    }

    public void Next(GameObject obj)
    {
        Next();
        Apply(obj);
    }

    public void Apply(GameObject obj)
    {
        if(!obj)
            return;

        obj.transform.position = GetPoint();
        if(rotate_)
            obj.transform.rotation = Quaternion.LookRotation(GetTangent());
    }

    public RoutePoint FindMessage(int start_index, string message)
    {
        if(route_config_ == null || start_index >= route_config_.count)
            return null;
        for(int i = start_index; i < route_config_.count; ++i)
        {
            var p = route_config_[i];
            if(p.message.CompareTo(message) == 0)
                return p;
        }
        return null;
    }

    public RoutePoint FindMessage(string message)
    {
        return FindMessage(0, message);
    }

    public RoutePoint FindMessageFromCurrent(string message)
    {
        return FindMessage(current, message);
    }

    /// <summary>
    /// 获得当前速度
    /// </summary>
    /// <returns></returns>
    public float GetCurrenVelocity()
    {
        float vel = current_point.velocity;
        if(vel <= 0.0f)
            vel = route_config_.velocity;
        return vel;
    }
    /// <summary>
    /// 下一节点速度
    /// </summary>
    /// <returns></returns>
    public float GetNextVelocity()
    {
        var next = next_point;
        if(next == null)
            return GetCurrenVelocity();
        float vel = next.velocity;
        if(vel <= 0.0f)
            vel = route_config_.velocity;
        return vel;
    }
    /// <summary>
    /// 获得上一节点速度
    /// </summary>
    /// <returns></returns>
    public float GetPrevVelocity()
    {
        var prev = prev_point;
        var vel = prev.velocity;
        if(vel <= 0.0f)
            vel = route_config_.velocity;
        return vel;
    }

    public void SetCurrent(int cur)
    {
        //route_config_.SetCurrent(cur);
        if(current_ != cur)
        {
            current_ = cur;
            bezier_ = route_config_.GetBezier3D(cur);
            last_move_length_ = -1;
        }
    }
    public Vector3 GetPoint()
    {
        if(bezier_ == null)
            bezier_ = route_config_.GetBezier3D(current_);
        return bezier_.Get(normalized);
    }
    public Vector3 GetTangent()
    {
        if(bezier_ == null)
            bezier_ = route_config_.GetBezier3D(current_);
        return bezier_.GetTangent(normalized);
    }

    void OnEnterPoint(RoutePoint p)
    {
        //if(last_enter_ || last_point_ == p)
        //    return;
#if UNITY_EDITOR
        if(Application.isPlaying)
        {
            on_enter_point(p);
            if(!string.IsNullOrEmpty(p.message))
                on_message(p.message);
        }
#else
        on_enter_point(p);
        if(!string.IsNullOrEmpty(p.message))
                on_message(p.message);
#endif
        last_enter_ = true;
        last_point_ = p;
    }
    void OnExitPoint(RoutePoint p)
    {
        //if(!last_enter_ || last_point_ == p)
        //    return;
#if UNITY_EDITOR
        if(Application.isPlaying)
            on_exit_point(p);
#else
        on_exit_point(p);
#endif
        last_enter_ = false;
        last_point_ = p;
    }

    /// <summary>
    /// 消息回调
    /// </summary>
    public event OnMessageFunc on_message;
    /// <summary>
    /// 进入路点回调
    /// </summary>
    public event OnEnterPointFunc on_enter_point;
    /// <summary>
    /// 离开路点回调
    /// </summary>
    public event OnExitPointFunc on_exit_point;
    /// <summary>
    /// 路点配置
    /// </summary>
    public RouteConfig route_config { get { return route_config_; } }
    RouteConfig route_config_;
    /// <summary>
    /// 路径数量
    /// </summary>
    public int count { get { return route_config_.count; } }
    /// <summary>
    /// 返回指针
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public RoutePoint this[int index]
    {
        get
        {
            if(route_config_ == null)
                return null;
            return route_config_[index];
        }
    }

    /// <summary>
    /// 当前路点序号
    /// </summary>
    public int current { get { return current_; } }
    int current_ = 0;
    /// <summary>
    /// 当前标准值
    /// </summary>
    float normalized
    {
        get
        {
            if(last_move_length_ != move_length_)
            {
                normalized_ = route_config_.GetNormalized(current_, move_length_);
                last_move_length_ = move_length_;
            }
            return normalized_;
        }
    }
    float normalized_ = 0;
    /// <summary>
    /// 当前曲线
    /// </summary>
    Bezier3D bezier_ = null;

    public RoutePoint current_point { get { return route_config_[current_]; } }
    public RoutePoint next_point { get { return route_config_[(current_ + 1) % route_config_.count]; } }
    public RoutePoint prev_point { get { return route_config_[(current_ - 1 + route_config_.count) % route_config_.count]; } }
    public float current_length
    {
        get
        {
            if(route_config_.IsLoop)
            {
                if(current < route_config_.count)
                    return route_config_.length[current_];
                return route_config_.length[current % route_config_.count];
            }
            else
            {
                if(current < route_config_.count - 1)
                    return route_config_.length[current_];
                return route_config_.length[route_config_.count - 2];
            }

        }
    }
    public float prev_length
    {
        get
        {
            if(current > 0)
                return route_config_.length[current_ - 1];
            else if(route_config_.IsLoop)
                return route_config_.length[(current - 1 + route_config_.count) % route_config_.count];
            else
                return route_config_.length[0];
        }
    }

    public float total_length { get { return route_config_.total_length; } }
    float curr_total_length
    {
        get
        {
            var list = total_lengths;
            if(current_ < list.Length)
                return list[current_];
            return route_config_.total_length;
        }
    }
    float prev_total_length 
    {
        get 
        {
            if(current_ > 0)
                return total_lengths[current_ - 1];
            return 0;
        }
    }
    float[] total_lengths
    {
        get
        {
            if(route_config_ == null)
            {
                total_lengths_ = null;
                config_lengths_ = null;
                return null;
            }
            if(route_config_.length != config_lengths_)
            {
                total_lengths_ = null;
                config_lengths_ = route_config_.length;
            }
            if(total_lengths_ == null)
            {
                float t = 0;
                total_lengths_ = new float[route_config_.length.Length];
                for(int i = 0; i < total_lengths_.Length; ++i)
                {
                    t += route_config_.length[i];
                    total_lengths_[i] = t;
                }
            }
            return total_lengths_;
        }
    }
    float[] total_lengths_ = null;
    float[] config_lengths_ = null;

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool is_finish { get { return is_finish_; } }
    bool is_finish_ = false;
    /// <summary>
    /// 当前进度
    /// </summary>
    public float t { get { return move_length_ / current_length; } }
    /// <summary>
    /// 当前节点下，移动长度
    /// </summary>
    public float move_length { get { return move_length_; } }
    float move_length_ = 0;
    float last_move_length_ = -1;

    /// <summary>
    /// 等待时间
    /// </summary>
    float wait_time_ = 0;
    /// <summary>
    /// 是否旋转
    /// </summary>
    bool rotate_ = false;


    bool last_enter_ = false;
    RoutePoint last_point_ = null;
}
