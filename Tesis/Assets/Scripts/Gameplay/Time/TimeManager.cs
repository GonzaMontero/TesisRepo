using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class TimeManager : MonoBehaviourSingletonInScene<TimeManager>
    {
        [Serializable]
        class SlowMoTarget
        {
            public Transform transform;
            public ITimed time;
            public float timer;

            public SlowMoTarget(Transform newtransform, ITimed newTarget, float newTimer)
            {
                transform = newtransform;
                time = newTarget;
                timer = newTimer;
            }
        }

        [Header("Set Values")]
        [SerializeField] Transform player;
        [SerializeField] float slowdownRange;
        [Tooltip("Speed of SlowMo")]
        [SerializeField] float slowdownFactor;
        [SerializeField] float slowdownLength;
        [Tooltip("Seconds before true deactivation")]
        [SerializeField] float slowdownExtraLength;
        [SerializeField] float cooldownLength;
        [SerializeField] int maxTargets;
        [Header("Runtime Values")]
        [SerializeField] List<SlowMoTarget> targets;
        [SerializeField] Camera mainCam;
        [SerializeField] Vector3 slowMoRayEnd;
        [SerializeField] Vector2 centerOfScreen;
        [SerializeField] float cooldownTimer;
        [SerializeField] bool slowMoIsReady;
        [SerializeField] bool playerOnFloor;

        public Action<bool> SlowMoReady;
        public Action<Transform, float> ObjectSlowed;
        public Action<Transform> ObjectUnSlowed;

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            Time.timeScale = slowdownFactor;
            mainCam = Camera.main;
            centerOfScreen = new Vector2(mainCam.pixelWidth / 2, mainCam.pixelHeight / 2);
        }
        void Update()
        {
            UpdateTimers();

            if (!slowMoIsReady) return;
            if (!PlayerIsOnFloor())
            {
                slowMoIsReady = false;
                return;
            }

            RaycastHit hit;
            if (!Physics.Raycast(mainCam.ScreenPointToRay(centerOfScreen), out hit)) return;
            slowMoRayEnd = hit.point;


#if UNITY_EDITOR
            DEBUGDrawRays();
#endif
        }
        public void OnReadySlowMo(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            ReadySlowMo();
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return; //only do action on press frame
            if (!slowMoIsReady) return; //Only slow if slow mo mode is active
            if (cooldownTimer < 1) return; //Only slow if cooldown over

            SlowTarget();

            if (targets.Count >= maxTargets)
            {
                DeSlowTarget(targets[0]);
            }
        }

        //Methods
        void DEBUGDrawRays()
        {
            if (!slowMoIsReady) return;

            //Debug.DrawRay(mainCam.ScreenPointToRay(centerOfScreen).origin, mainCam.ScreenPointToRay(centerOfScreen).direction, Color.gray);

            //Get target (if not, deactivate oldest target)
            RaycastHit hit;
            Vector3 direction = (slowMoRayEnd - player.position).normalized;
            if (!Physics.Raycast(player.position, direction, out hit, slowdownRange))
            {
                Debug.DrawRay(player.position, direction * slowdownRange, Color.blue);
                //Debug.Log("Hitted Nothing");
                return;
            }

            //Check if target is valid (if not, deactivate oldest target)
            ITimed target = hit.transform.GetComponent<ITimed>();
            if (target == null)
            {
                Debug.DrawRay(player.position, direction * slowdownRange, Color.red);
                //Debug.Log("Hitted Not Valid");
            }
            else
            {
                Debug.DrawRay(player.position, direction * slowdownRange, Color.green);
                //Debug.Log("Hitted Valid");
            }
        }
        void ReadySlowMo()
        {
            slowMoIsReady = !slowMoIsReady;
            SlowMoReady?.Invoke(slowMoIsReady);

            Debug.Log("SlowMo Ready: " + slowMoIsReady);
        }
        void SlowTarget()
        {
            //Get target (if not, deactivate oldest target)
            RaycastHit hit;
            Physics.Raycast(player.position, player.forward, out hit, slowdownRange);
            if (hit.transform == null)
            {
                if (targets.Count > 0)
                {
                    StartCoroutine(DeSlowTarget(targets[0]));
                }
                return;
            }

            //Check if target is valid (if not, deactivate oldest target)
            ITimed objectToSlow = hit.transform.GetComponent<ITimed>();
            if (objectToSlow == null)
            {
                if (targets.Count > 0)
                {
                    StartCoroutine(DeSlowTarget(targets[0]));
                }
                return;
            }

            //Add target to list and slow it
            targets.Add(new SlowMoTarget(hit.transform, objectToSlow, 0));
            objectToSlow.TimeChanged(true);
            ObjectSlowed?.Invoke(hit.transform, slowdownLength + slowdownExtraLength);

            //Start Cooldown
            cooldownTimer = 0;
        }
        IEnumerator DeSlowTarget(SlowMoTarget target)
        {
            //Update List
            targets.Remove(target);
            Debug.Log("Target Removed at " + Time.realtimeSinceStartup + "!");

            yield return new WaitForSecondsRealtime(slowdownExtraLength);

            //Update Target Time
            Debug.Log("Target UnSlowed at " + Time.realtimeSinceStartup + "!");
            target.time.TimeChanged(false);
            ObjectUnSlowed?.Invoke(target.transform);
        }
        void UpdateTimers()
        {
            //Run Cooldown Timer
            if (cooldownTimer < 1)
            {
                cooldownTimer += Time.unscaledDeltaTime / cooldownLength;
            }

            //Run SlowMo Timers from targets
            if (targets.Count <= int.MinValue) return;
            int i = 0;
            while (i < targets.Count)
            {
                if (targets[i].timer > 1)
                {
                    StartCoroutine(DeSlowTarget(targets[i]));                    
                    continue;
                }

                targets[i].timer += Time.unscaledDeltaTime / slowdownLength;
                i++;
            }
        }
        bool PlayerIsOnFloor()
        {
            /*bool*/ playerOnFloor = Physics.Raycast(player.position, -player.up, 2);

#if UNITY_EDITOR
            if (!playerOnFloor)
            {
                Debug.Log("Player Not On Floor!");
            }
#endif

            return player;
        }
    }
}