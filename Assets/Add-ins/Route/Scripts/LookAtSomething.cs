using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LookAtSomething : MonoBehaviour 
{
	void Update () 
    {
        if(target == null)
            return;

        //var dir = (target.transform.position - transform.position).normalized;
        transform.LookAt(target.transform);
	}

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(target == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
#endif

    public GameObject target;
}
