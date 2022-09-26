using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SwordHit : MonoBehaviour
{
    [SerializeField] BoxCollider hitRange;
    [SerializeField] float hitImpulse;
    [SerializeField] int damage;

    public System.Action HittedSomething;

    private void Awake()
    {
        hitRange = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.transform.tag == "Player")
            return;

            HittedSomething?.Invoke();
        
        IHittable hit;
        hit = other.gameObject.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHitted(damage);
        }
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(hitImpulse * transform.forward);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (gameObject.transform.tag == "Player")
    //        return;

    //    IHittable hit;
    //    hit = other.gameObject.GetComponent<IHittable>();
    //    if (hit != null)
    //    {
    //        hit.GetHitted(damage);
    //    }
    //}
}
