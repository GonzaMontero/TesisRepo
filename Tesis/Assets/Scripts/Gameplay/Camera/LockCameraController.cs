using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TimeDistortion.Gameplay.Cameras
{
    public class LockCameraController : CameraController
    {
        [Header("Set Values")]
        [SerializeField] internal Transform player;
        [SerializeField] Transform mainCam;
        [SerializeField] LayerMask lockLayers;
        [Tooltip("Which layers will the camera ignore " +
                    "when checking if the target is behind a wall?")]
        [SerializeField] LayerMask lockObstacleLayers;
        [Tooltip("X = min, Y = max")]
        [SerializeField] Vector2 lockAngle;
        [SerializeField] float lockRange;
        [SerializeField] float followRange;

        [Header("Runtime Values")]
        [SerializeField] Transform lockTarget;
        [SerializeField] internal bool isLocked;

        public System.Action<bool> CameraLocked;

        //Unity Events
        internal new void Start()
        {
            base.Start();

            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            if(!mainCam)
            {
                mainCam = Camera.main.transform;
            }
        }
        internal virtual void Update()
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

            UpdateLockOn();
        }

        //Methods
        internal virtual void UpdateLockOn()
        {
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
        void ClearLock()
        {
            cam.LookAt = null;
            lockTarget = null;
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
            for (int i = 0; i < colliders.Length; i++)
            {
                //Check if collider has the TargetComponent (if not, try next one)
                LockTarget target = colliders[i].GetComponent<LockTarget>();
                if (!target) continue;

                //Check if target is in range (if not, try next one)
                Vector3 lockTargetDir = target.transform.position - player.position;
                targetDis = Vector3.Distance(player.position, target.transform.position);
                if (targetDis > lockRange) continue;

                //Check if target is inside right angles (if not, try next one)
                float targetAngle = Vector3.Angle(lockTargetDir, mainCam.forward);
                if (targetAngle < lockAngle.x || targetAngle > lockAngle.y) continue;

                availableTargets.Add(colliders[i].transform);
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

            if (targetDis > followRange)
            {
                //Clear lock
                UpdateLockOn();
                return;
            }

            Vector3 camPos = mainCam.position;
            Vector3 targetDir = (lockTarget.position - camPos).normalized;
            float camDis = Vector3.Distance(player.position, mainCam.position);
            
            RaycastHit hit;
            if (Physics.Raycast(camPos, targetDir, out hit, followRange + camDis, lockObstacleLayers))
            {
                if (hit.transform != lockTarget)
                {
                    Debug.Log("Lock on canceled by " + hit.transform.gameObject.name);
                    
                    //Clear lock
                    UpdateLockOn();
                    return;
                }
            }
        }
    }
}