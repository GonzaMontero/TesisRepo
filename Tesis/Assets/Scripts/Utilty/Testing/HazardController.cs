﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    [SerializeField] int damage;

    private void OnCollisionStay(Collision collision)
    {
        IHittable hittedObject = collision.gameObject.GetComponent<IHittable>();
        if (hittedObject == null)
            return;
        hittedObject.GetHitted(damage);
    }
}