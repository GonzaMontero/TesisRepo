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

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.transform.tag == "Player")
            return;

        IHittable hit;
        hit = collision.gameObject.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHitted(damage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (gameObject.transform.tag == "Player")
            return;

        IHittable hit;
        hit = collision.gameObject.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.GetHitted(damage);
        }
    }
}
