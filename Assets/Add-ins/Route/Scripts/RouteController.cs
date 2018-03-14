using UnityEngine;
using System;
using System.Collections;
using Bezier3D = RouteMath.Bezier3D;

public class RouteController
{
    public delegate void OnMessage(string message);
    public delegate void OnEnterPoint(RoutePoint pt);
    public delegate void OnExitPoint(RoutePoint pt);

    public RouteController()
    {
        on_message_ = msg => { };
        on_enter_point_ = pt => { };
        on_exit_point_ = pt => { };
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
        move_length_ = 0;
        if(route_config_ != null)
        {
            current_ = 0;
            bezier_ = route_config_.GetBezier3D(current_);
        }
    }
    ///// <summary>
    ///// 重置移动
    ///// </summary>
    //public void ResetMove()
    //{
    //    move_length_ = 0;
    //}
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
        if(is_finish)
            return;

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
            on_exit_point_(current_point);
            //是否完成
            if(is_finish)
                break;
            //进入下一路点
            SetCurrent(current + 1);

            on_enter_point_(current_point);
            if(!string.IsNullOrEmpty(current_point.message))
                on_message_(current_point.message);

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

    public void UpdateNegative(float dt)
    {
        if(current == 0)
            return;

        wait_time_ += dt;
        if(next_point)
        {
            if(wait_time_ <= next_point.keeptime)
                return;
            dt = wait_time_ - next_point.keeptime;
            wait_time_ = next_point.keeptime;
        }
        else
        {
            dt = wait_time_;
            wait_time_ = 0;
        }

        float velocity = GetCurrenVelocity();
        move_length_ -= velocity * dt;
        while(move_length_ < 0)
        {
            on_exit_point_(current_point);
            //已经到达头部
            if(current == 0)
            {
                move_length_ = 0;
                break;
            }
            //进入下一路点
            SetCurrent(current - 1);

            on_enter_point_(current_point);
            if(!string.IsNullOrEmpty(current_point.message))
                on_message_(current_point.message);

            //计算等待时间
            wait_time_ = move_length_ / velocity;
            float keeptime = 0;
            if(next_point != null)
                keeptime = next_point.keeptime;

            if(wait_time_ > keeptime)
            {
                velocity = GetCurrenVelocity();
                move_length_ = current_length - (wait_time_ - keeptime) * velocity;
            }
            else
            {
                move_length_ = current_length;
                break;
            }
        }
    }

    public void Update(float dt, GameObject obj)
    {
        Update(dt);
        Apply(obj);
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
        while(move_length_ >= current_length)
        {
            move_length_ -= current_length;
#if UNITY_EDITOR
            if(Application.isPlaying)
                on_exit_point_(current_point);
#else
            on_exit_point_(current_point);
#endif
            //是否完成
            if(is_finish)
                break;
            //进入下一路点
            SetCurrent(current_ + 1);

#if UNITY_EDITOR
            if(Application.isPlaying)
                on_enter_point_(current_point);
#else
            on_enter_point_(current_point);
#endif
            if(!string.IsNullOrEmpty(current_point.message))
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                    on_message_(current_point.message);
#else
                on_message_(current_point.message);
#endif
            }
        }
    }

    public void Next()
    {
        on_exit_point_(current_point);
        if(!is_finish)
        {
            SetCurrent(current + 1);
            wait_time_ = 0;
            move_length_ = 0;

            on_enter_point_(current_point);
            if(!string.IsNullOrEmpty(current_point.message))
                on_message_(current_point.message);
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
        {
            obj.transform.rotation = Quaternion.LookRotation(GetTangent());
        }
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
    
    /// <summary>
    /// 消息回调
    /// </summary>
    public OnMessage on_message { set { on_message_ = value; } }
    OnMessage on_message_;
    /// <summary>
    /// 进入路点回调
    /// </summary>
    public OnEnterPoint on_enter_point { set { on_enter_point_ = value; } }
    OnEnterPoint on_enter_point_;
    /// <summary>
    /// 离开路点回调
    /// </summary>
    public OnExitPoint on_exit_point { set { on_exit_point_ = value; } }
    OnExitPoint on_exit_point_;
    /// <summary>
    /// 路点配置
    /// </summary>
    public RouteConfig route_config { get { return route_config_; } }
    RouteConfig route_config_;
    /// <summary>
    /// 数量
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
                normalized_ = route_config_.GetNormalizedT(current_, move_length_);
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

    float curr_total_length
    {
        get
        {
            var list = total_length;
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
                return total_length[current_ - 1];
            return 0;
        }
    }
    float[] total_length
    {
        get
        {
            if(route_config_ == null)
            {
                total_length_ = null;
                config_length_ = null;
                return null;
            }
            if(route_config_.length != config_length_)
            {
                total_length_ = null;
                config_length_ = route_config_.length;
            }
            if(total_length_ == null)
            {
                float t = 0;
                total_length_ = new float[route_config_.length.Length];
                for(int i = 0; i < total_length_.Length; ++i)
                {
                    t += route_config_.length[i];
                    total_length_[i] = t;
                }
            }
            return total_length_;
        }
    }
    float[] total_length_ = null;
    float[] config_length_ = null;

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool is_finish 
    { 
        get 
        {
            return !route_config_ || current + 1 >= count; 
        }
    }
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
}
