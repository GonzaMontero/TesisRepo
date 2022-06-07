using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private GameObject target = null;
    private Vector3 offset;
    void Start()
    {
        target = null;
    }
    void Update()
    {
        Vector3 v = transform.position;
        v.y += transform.localScale.y * 0.5f;
        Physics.OverlapBox(transform.position, transform.localScale * 0.5f);
    }
    void OnTriggerStay(Collider col)
    {
        if (col.tag != "Player")
            return;
        target = col.gameObject;
        offset = target.transform.position - transform.position;
    }
    void OnTriggerExit(Collider col)
    {
        target = null;
    }
    void LateUpdate()
    {
        if (target != null)
        {
            target.transform.position = transform.position + offset;
        }
    }
}
