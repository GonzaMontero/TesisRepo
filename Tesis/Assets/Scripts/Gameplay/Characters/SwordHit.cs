using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SwordHit : MonoBehaviour
{
    [SerializeField] BoxCollider hitRange;
    [SerializeField] int damage;


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
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameObject.transform.tag == "Player")
            return;

        IHittable hit;
        hit = other.gameObject.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHitted(damage);
        }
    }
}
