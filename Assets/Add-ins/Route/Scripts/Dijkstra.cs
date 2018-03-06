using System;
using System.Collections.Generic;

public class Dijkstra<T>
{
    /// <summary>
    /// 结果
    /// </summary>
    public class Result
    {
        public List<T> GetRoad(T end)
        {
            if(!road.ContainsKey(end))
                return null;

            List<T> list = new List<T>();
            list.Add(end);

            T prev = default(T);
            T next = end;
            while(!(prev = road[next]).Equals(next))
            {
                list.Add(prev);
                next = prev;
            }

            return list;
        }

        /// <summary>
        /// 路径连接点
        /// </summary>
        public Dictionary<T, T> road;
        /// <summary>
        /// 总距离
        /// </summary>
        public Dictionary<T, float> dist;
    }

    /// <summary>
    /// 添加点
    /// </summary>
    public void Add(T point)
    {
        points_.Add(point);
        graph_[point] = new Dictionary<T, float>();
    }
    /// <summary>
    /// 添加点集合
    /// </summary>
    public void Add(IEnumerable<T> points)
    {
        foreach(var p in points)
            Add(p);
    }

    public void Clear()
    {
        points_.Clear();
        graph_.Clear();
    }

    /// <summary>
    /// 设置点与点之间距离
    /// </summary>
    /// <param name="start_point">起点</param>
    /// <param name="end_point">终点</param>
    /// <param name="dist">距离</param>
    /// <param name="oneway">是否为单向</param>
    public void SetDist(T start_point, T end_point, float dist, bool oneway)
    {
        if(!points_.Contains(start_point))
            Add(start_point);
        graph_[start_point][end_point] = dist;
        if(!oneway)
            SetDist(end_point, start_point, dist, true);
    }

    /// <summary>
    /// 找寻最短距离点
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="dist"></param>
    /// <returns></returns>
    static T FindMinPoint(HashSet<T> queue, Dictionary<T, float> dist)
    {
        var min_dist = float.MaxValue;
        T point = default(T);
        foreach(var p in queue)
        {
            var d = dist[p];
            if(d < min_dist)
            {
                min_dist = d;
                point = p;
            }
        }
        return point;
    }
    /// <summary>
    /// 获得点与点的距离，负值表示无连接
    /// </summary>
    /// <param name="start_point"></param>
    /// <param name="end_point"></param>
    /// <returns></returns>
    float GetDist(T start_point, T end_point)
    {
        var dists = graph_[start_point];
        float dist = 0;
        if(dists.TryGetValue(end_point, out dist))
            return dist;
        return -1;
    }
    /// <summary>
    /// 计量路径点
    /// </summary>
    /// <param name="start_point"></param>
    /// <returns></returns>
    public Result Emit(T start_point)
    { 
        Result result = new Result();
        if(!points_.Contains(start_point))
            return result;

        Dictionary<T, T> road = new Dictionary<T, T>();
        Dictionary<T, float> dist = new Dictionary<T, float>();
        HashSet<T> queue = new HashSet<T>();
        foreach(var p in points_)
        {
            dist[p] = float.MaxValue;
            queue.Add(p);
        }
        dist[start_point] = 0;
        road[start_point] = start_point;

        while(queue.Count > 0)
        {
            var p = FindMinPoint(queue, dist);
            if(p == null)
                //剩下孤立点
                break;

            //移除点
            queue.Remove(p);
            foreach(var pp in points_)
            {
                var d = GetDist(p, pp);
                if(d > 0 && dist[pp] > dist[p] + d)
                {
                    dist[pp] = dist[p] + d;
                    road[pp] = p;
                }
            }
        }

        result.road = road;
        result.dist = dist;
        return result;
    }

    /// <summary>
    /// 点集
    /// </summary>
    HashSet<T> points_ = new HashSet<T>();
    /// <summary>
    /// 点与点之间距离对照表(单向)
    /// </summary>
    Dictionary<T, Dictionary<T, float>> graph_ = 
        new Dictionary<T,Dictionary<T,float>>();
}
