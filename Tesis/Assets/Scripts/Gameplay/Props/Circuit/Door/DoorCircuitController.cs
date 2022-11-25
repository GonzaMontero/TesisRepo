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
        [Header("DEBUG")]
        [SerializeField] bool setPositions;

        //Unity Events
        private void Start()
        {
            manager.CircuitCompleted += OnCircuitCompleted;
        }
        void Update()
        {
            Move();
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            //Draw closed door
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube( closedPos, coll.bounds.size);

            //Draw opened door
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(openedPos, coll.bounds.size);

            if(!setPositions) return;
            setPositions = false;
            SetPositions();
        }
#endif

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
            Vector3 pos = transform.position;

            float disToTarget = Vector3.Distance(pos, targetPos);
            float disToMove = Vector3.Distance(pos, pos + movement);
            
            if (disToTarget < disToMove)
            {
                moveDir = Vector3.zero;
                transform.position = targetPos;
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