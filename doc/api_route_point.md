RoutePoint
==========

方法
----

**RouteConfig GetConfig()**

> 返回该节点所在的[RouteConfig](api_route_config.md)
> 
> 无参数

属性/变量
---------

**Color color**

> 节点在SceneView中绘制的颜色

**string message**

> 节点消息

**float keeptime**

> 停留时间

**float velocity**

> 局部速度，前往下一点速度，0表示无效

**Vector3 prev_weight_point**

> 对前一个节点曲线权重

**Vector3 next_weight_point**

> 对后一个节点曲线权重

**WeightType weight_type**

> 曲线杆模式类型。参见[WeightType](api_weight_type.md)
