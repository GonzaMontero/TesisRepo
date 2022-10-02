﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SwordHit : MonoBehaviour
{
    [SerializeField] BoxCollider hitRange;
    [SerializeField] float hitImpulse;
    [SerializeField] int damage;

    public System.Action HittedSomething;
    public System.Action HittedStone;

    private void Awake()
    {
        hitRange = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.transform.tag == "Player")
            return;
        
        IHittable hit;
        hit = other.gameObject.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHitted(damage);
            HittedSomething?.Invoke();
        }
        else
        {
            HittedStone?.Invoke();
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
