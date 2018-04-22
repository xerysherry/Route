*class* RouteController
=======================

方法
----

**void SetRouteConfig(RouteConfig config)**
> 设置路点配置[RouteConfig](api_route_config.md)  
> 参数:  
>       config: 路点配置[RouteConfig](api_route_config.md)  

**void SetRotateEnable(bool rotate)**
> 是否旋转。设置为TRUE是，在应用对象时将转向切线方向  
> 参数：  
>       rotate: 是否开启旋转  

**void Reset()**
> 重置，包括移动距离，完成标记，并清空路点缓存。  

**void Clear()**
> 清理数据，去除路点配置  

**void Update(float dt)**
> 更新  
> 参数：  
>       dt: 时间间隔  

**void Update(float dt, GameObject obj)**
> 更新并应用对象  
> 参数：  
>       dt: 时间间隔  
>       obj: 应用对象  
  
**void SetIsFinish(bool value)**
> 设置完成标记  
> 参数：  
>       value: 值  

**void SetMove(float length)**
> 设置移动距离  
> 参数：  
>       length: 移动距离  

**void Step(float delta_length)**
> 步进更新  
> 参数：  
>       delta_length: 步长  

**void Next()**
> 进入下一个节点  

**void Next(GameObject obj)**
> 进入下一个节点并应用对象  
> 参数：  
>       obj: 应用对象  

**void Apply(GameObject obj)**
> 应用对象  
> 参数：  
>       obj: 应用对象  

**RoutePoint FindMessage(int start_index, string message)**
> 通过消息查找节点  
> 参数：   
>       start_index: 开始节点索引  
>       message: 消息  
> 返回：返回路点[RoutePoint](api_route_point.md)

**RoutePoint FindMessage(string message)**
> 通过消息查找节点  
> 参数：  
>       message: 消息  
> 返回：返回路点[RoutePoint](api_route_point.md)

**RoutePoint FindMessageFromCurrent(string message)**
> 从当前索引开始通过消息查找节点  
> 参数：  
>       message: 消息  
> 返回：返回路点[RoutePoint](api_route_point.md)

**float GetCurrenVelocity()**
> 获得当前速度  
> 返回：当前速度

**float GetNextVelocity()**
> 获得下一节点速度  
> 返回：下一节点速度

**float GetPrevVelocity()**
> 获得上一节点速度  
> 返回：上一节点速度

**void SetCurrent(int cur)**
> 设置当前索引  
> 参数：  
>       cur: 索引

**Vector3 GetPoint()**
> 获取当前世界坐标点  
> 返回：当前世界坐标点

**Vector3 GetTangent()**
> 获取当前切线  
> 返回：当前切线

属性/变量
---------

**event OnMessageFunc on_message**
> 消息回调

**event OnEnterPointFunc on_enter_point**
> 进入路点回调

**event OnExitPointFunc on_exit_point**
> 离开路点回调

**RouteConfig route_config { get; }**
> 路点配置

**int count { get; }**
> 路点数量

**int current { get; }**
> 当前路点索引

**bool is_finish { get; }**
> 是否完成

**float move_length { get; }**
> 当前曲线段下，移动长度