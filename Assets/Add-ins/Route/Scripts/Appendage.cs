using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Appendage : MonoBehaviour
{
	void Update () 
    {
        if(target == null)
            return;
        transform.position = target.transform.position + offset;
        if(rotation)
            transform.rotation = target.transform.rotation;
	}

    public GameObject target;
    public Vector3 offset = Vector3.zero;
    public bool rotation = false;
}
