using System;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class LookAtController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] LayerMask targetLayers;
        [SerializeField] Vector3 checkRadius;
        [SerializeField] Vector3 checkOffset;
        [SerializeField] float lookSpeed;
        [Header("Runtime Values")]
        [SerializeField] Quaternion targetLook;
        [SerializeField] bool lookingAtTarget;
        Quaternion originalRot;

        public Vector3 targetPos { get; private set; }
        public bool hasTarget => lookingAtTarget;

        //Unity Events
        void Start()
        {
            originalRot = transform.rotation;
        }
        void Update()
        {
            SearchForTarget();
            //if (targetPos.sqrMagnitude == 0) return;
            Rotate();
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.Lerp(Color.cyan, Color.clear, 0.5f);
            
            Gizmos.DrawCube(transform.position + checkOffset, checkRadius * 2);
        }

        //Methods
        void SearchForTarget()
        {
            Vector3 pos = transform.position + checkOffset;

            Collider[] collisions;
            collisions = Physics.OverlapBox(pos, checkRadius, Quaternion.identity, targetLayers);

            if (collisions.Length < 1)
            {
                if (lookingAtTarget)
                {
                    lookingAtTarget = false;
                    targetLook = originalRot;
                }
                return;
            }
            
            if (!lookingAtTarget)
            {
                lookingAtTarget = true;
            }

            targetPos = collisions[0].transform.position;
            targetLook = Quaternion.LookRotation(targetPos - transform.position);
            //targetLook *= Quaternion.Euler(originalRot.eulerAngles * 2);
        }
        void Rotate()
        {
            float frameRot = Time.deltaTime * lookSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, frameRot);
            //transform.localRotation =  targetLook;
        }
    }
}