using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props.Circuit
{
    public class DoorCircuitController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] CircuitManager manager;
        [SerializeField] BoxCollider coll;
        [SerializeField] Vector3 openedPos;
        [SerializeField] Vector3 closedPos;
        [SerializeField] float speed;
        [Header("Runtime Values")]
        [SerializeField] Vector3 targetPos;
        [SerializeField] Vector3 moveDir;

        //Unity Events
        private void Start()
        {
            manager.CircuitCompleted += OnCircuitCompleted;
            SetPositions();
        }
        void Update()
        {
            Move();
        }

        //Methods
        void SetPositions()
        {
            closedPos = transform.position;
            openedPos = transform.position + coll.bounds.size.y * transform.up;
        }
        void Move()
        {
            if(moveDir.sqrMagnitude == 0) return;

            Vector3 movement = moveDir * speed * Time.deltaTime;

            float disToTarget = Vector3.Distance(transform.position, targetPos);
            float disToMove = Vector3.Distance(transform.position, transform.position + movement);
            
            if (disToTarget < disToMove)
            {
                moveDir = Vector3.zero;
                transform.position = targetPos;
                Debug.Log("Reached Destiny!");
                return;
            }
            
            transform.Translate(movement);

        }

        //Event Receiver
        void OnCircuitCompleted(bool isCircuitComplete)
        {
            if (isCircuitComplete)
            {
                targetPos = openedPos;
            }
            else
            {
                targetPos = closedPos;
            }

            moveDir = (targetPos - transform.position).normalized;
        }
    }
}