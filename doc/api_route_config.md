*class* RouteConfig
===================

方法
----

**void Refresh()**

> 刷新节点，重置曲线缓存  

**float GetNormalizedByTotalLength(float dist)**
> 通过总长度设置对应点，并获得曲线段标准量  
> 参数：  
>       dist: 总长度  
> 返回：返回曲线段标准量

**float GetNormalizedByTotalLength(float dist, out int index)**
> 通过总长度设置对应点，并获得曲线段标准量  
> 参数：  
>       dist: 总长度  
>       out index: 获取曲线段索引  
> 返回：返回曲线段标准量

**Bezier3D GetBezier3D(int index)**
> 获取曲线对象[Bezier3D](api_bezier_3d.md)  
> 参数：  
>       index: 曲线段索引  
> 返回：返回曲线对象[Bezier3D](api_bezier_3d.md)

**int GetNextIndex(int current)**
> 获取下一个索引  
> 参数：  
>       current: 当前索引  
> 返回：下一个索引

**int GetPrevIndex(int current)**
> 获取上一个索引  
> 参数：  
>       current: 当前索引  
> 返回：上一个索引

**float GetNormalized(int index, float move)**
> 获得标准量T  
> 参数：  
>       index: 曲线段索引  
>       move: 曲线段移动距离  
> 返回曲线段标准量T

属性/变量
---------

**bool IsLoop { *get* }**

> 是否为闭环

**RoutePoint[] points { *get* }**

> 路径点数组

**int count**

> 路点数量

**float step_length**

> 切割单位长  
> 值越小，切割越细，曲线越光滑，但是存储空间的越大

**float velocity**

> 全局速度，在子节点速度未配置时有效。如果为0，表示无效

**float total_length**

> 总长度