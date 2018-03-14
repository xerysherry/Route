using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Route/LookAtSomething")]
public class LookAtSomething : MonoBehaviour 
{
	void Update () 
    {
        if(target == Target.TargetGameObject)
        {
            if(target_obj == null)
                return;
            transform.LookAt(target_obj.transform);
        }
        else
        {
            transform.LookAt(target_pos);
        }
	}

#if UNITY_EDITOR
    static readonly float kStep = 1.0f;

    void OnDrawGizmos()
    {
        Vector3 pos = Vector3.zero;
        Gizmos.color = Color.yellow;
        if(target == Target.TargetGameObject)
        {
            if(target_obj == null)
                return;
            pos = target_obj.transform.position;
        }
        else
        {
            pos = target_pos;
        }

        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(pos, 0.5f);
        Gizmos.DrawLine(transform.position, pos);

        Vector3 v = (pos - transform.position);
        Vector3 dir = v.normalized;
        float length = v.magnitude;
        float l = 0;
        v = v.normalized;
        
        if(v.x == 0 && Mathf.Abs(v.y) == 1 && v.z == 0)
            v.z += 0.1f;
        Vector3 axis = Vector3.Cross(v, Vector3.up);
        axis = Vector3.Cross(v, axis);
        var p1 = (Quaternion.AngleAxis(160, axis) * dir).normalized * 0.3f;
        var p2 = (Quaternion.AngleAxis(-160, axis) * dir).normalized * 0.3f;

        axis = Vector3.Cross(axis, dir);
        var p3 = (Quaternion.AngleAxis(160, axis) * dir).normalized * 0.3f;
        var p4 = (Quaternion.AngleAxis(-160, axis) * dir).normalized * 0.3f;

        while(l < length)
        {
            var p = transform.position + dir * l;
            Gizmos.DrawRay(p, p1);
            Gizmos.DrawRay(p, p2);
            Gizmos.DrawRay(p, p3);
            Gizmos.DrawRay(p, p4);
            l += kStep;
        }
    }
#endif

    public enum Target
    {
        TargetPosition,
        TargetGameObject,
    }
    public Target target = Target.TargetGameObject;
    public GameObject target_obj;
    public Vector3 target_pos;
}
