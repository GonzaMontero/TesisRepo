using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class BreakableCircuitController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitManager manager;
        //[SerializeField] Rigidbody[] breakableParts;
        //[SerializeField] float explosionImpulse;
        //[Header("Runtime Values")]


        //Unity Events
        void Start()
        {
            manager.CircuitCompleted += OnCircuitCompleted;
        }

        //Methods
        void Break()
        {
            Destroy(gameObject);
            // for (int i = 0; i < breakableParts.Length; i++)
            // {
            //     breakableParts[i].isKinematic = false;
            //     breakableParts[i].AddExplosionForce(explosionImpulse, transform.position, 5f);
            // }
        }
        
        //Event Receiver
        void OnCircuitCompleted(bool isCircuitComplete)
        {
            if (isCircuitComplete)
            {
                Break();
            }
        }
    }
}