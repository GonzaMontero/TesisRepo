﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay
{
    public class LockCameraController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] Cinemachine.CinemachineVirtualCamera cam;
        [SerializeField] Transform mainCam;
        [SerializeField] Transform player;
        [SerializeField] LayerMask lockLayers;
        [Tooltip("Which layers will the camera ignore " +
                    "when checking if the target is behind a wall?")]
        [SerializeField] LayerMask lockObstacleLayers;
        [Tooltip("X = min, Y = max")]
        [SerializeField] Vector2 lockAngle;
        [SerializeField] float lockRange;

        [Header("Runtime Values")]
        [SerializeField] Transform lockTarget;
        [SerializeField] bool isLocked;

        public System.Action<bool> CameraLocked;

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            if(!mainCam)
            {
                mainCam = Camera.main.transform;
            }
        }
        private void Update()
        {
            if (!isLocked) return;

            UpdateTarget();

            if(!lockTarget)
            {
                ClearLock();
            }
        }
#if UNITY_EDITOR
[ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            if(!player) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, lockRange);

            if (!lockTarget) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(mainCam.position, lockTarget.position);

        }
#endif
        public void OnLockOnInput(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (isLocked)
            {
                ClearLock();
            }
            else
            {
                GetTarget();
                SetLock();
            }
        }

        //Methods
        void ClearLock()
        {
            cam.LookAt = null;
            isLocked = false;
            CameraLocked?.Invoke(false);

            //CLEAN LATER
            player.GetComponent<Handler.PlayerController>().lockOnFlag = false;
        }
        void SetLock()
        {
            if (!lockTarget) return;

            cam.LookAt = lockTarget;
            isLocked = true;
            CameraLocked?.Invoke(true);

            //CLEAN LATER
            player.GetComponent<Handler.PlayerController>().lockOnFlag = true;
        }
        void GetTarget()
        {
            lockTarget = null;

            List<Transform> availableTargets = new List<Transform>();
            Collider[] colliders;
            colliders = Physics.OverlapSphere(player.position, lockRange, lockLayers);

            float targetDis;
            foreach (Collider collider in colliders)
            {
                //Check if collider has the TargetComponent (if not, try next one)
                LockTarget target = collider.GetComponent<LockTarget>();
                if (!target) continue;

                //Check if target is in range (if not, try next one)
                Vector3 lockTargetDir = target.transform.position - player.position;
                targetDis = Vector3.Distance(player.position, target.transform.position);
                if (targetDis > lockRange) continue;

                //Check if target is inside right angles (if not, try next one)
                float targetAngle = Vector3.Angle(lockTargetDir, mainCam.forward);
                if (targetAngle < lockAngle.x || targetAngle > lockAngle.y) continue;

                availableTargets.Add(collider.transform);
            }

            float shortestDistance = Mathf.Infinity;
            foreach (var newTarget in availableTargets)
            {
                targetDis = Vector3.Distance(player.position, newTarget.position);

                if (targetDis < shortestDistance)
                {
                    shortestDistance = targetDis;
                    lockTarget = newTarget;
                }
            }
        }
        void UpdateTarget()
        {
            float targetDis = Vector3.Distance(player.position, lockTarget.position);

            if (targetDis > lockRange)
            {
                lockTarget = null;
                return;
            }

            Vector3 camPos = mainCam.position;
            Vector3 targetDir = (lockTarget.position - camPos).normalized;
            float camDis = Vector3.Distance(player.position, mainCam.position);
            RaycastHit hit;
            if (Physics.Raycast(camPos, targetDir, out hit, lockRange + camDis, lockObstacleLayers))
            {
                if (hit.transform != lockTarget)
                {
                    Debug.Log("Lock on canceled by " + hit.transform.gameObject.name);
                    lockTarget = null;
                    return;
                }
            }
        }
    }
}