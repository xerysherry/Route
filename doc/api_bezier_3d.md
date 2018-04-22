*class* Bezier3D
================

方法
----

**Bezier3D()**
> 构造函数

**Bezier3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)**
> 构造函数
> 参数：
>       p0: 点参数0
>       p1: 点参数1
>       p2: 点参数2
>       p3: 点参数3

**void Set(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)**
> 设置曲线参数
> 参数：
>       p0: 点参数0
>       p1: 点参数1
>       p2: 点参数2
>       p3: 点参数3

**Vector3 Get(float t)**
> 获取曲线上某一点
> 参数：
>       t: 曲线标准量
> 返回：返回曲线上某一点

**Vector3 GetTangent(float t)**
> 获取曲线上某一点切线
> 参数：
>       t: 曲线标准量
> 返回：返回曲线上某一点切线
