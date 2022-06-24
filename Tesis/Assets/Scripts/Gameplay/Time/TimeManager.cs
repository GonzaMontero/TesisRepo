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
            public Transform transform = null;
            public ITimed time = null;
            public float timer = 0;
            public int ID = 0;

            public SlowMoTarget(Transform newtransform, ITimed newTarget)
            {
                transform = newtransform;
                time = newTarget;
                timer = 0;
                ID = transform.GetInstanceID();
            }
        }

        [Header("Set Values")]
        [SerializeField] Transform player;
        [SerializeField] LayerMask slowableLayers;
        [SerializeField] float slowdownDelay;
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
        [SerializeField] SlowMoTarget currentTarget;
        [SerializeField] Camera mainCam;
        [SerializeField] Vector2 centerOfScreen;
        [SerializeField] float cooldownTimer;
        [SerializeField] bool slowMoIsReady;
        [SerializeField] bool playerOnFloor;
        Dictionary<int, SlowMoTarget> targetIDs;

        public Action<bool> SlowMoReady;
        public Action<bool> TargetInScope;
        public Action<Transform, float> ObjectSlowed;
        public Action<Transform> ObjectUnSlowed;
        public Action<int> ObjectDestroyed;
        public Action SlowMoFailed;

        public float publicCooldownTimer { get { return cooldownTimer; } }
        public float publicDelay { get { return slowdownDelay; } }

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            //Set Camera
            mainCam = Camera.main;
            centerOfScreen = new Vector2(mainCam.pixelWidth / 2, mainCam.pixelHeight / 2);

            //Instatiate Dictionary
            targetIDs = new Dictionary<int, SlowMoTarget>();
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

            GetValidTarget();

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
            //Debug.Log("Key Pressed!");
            if (currentTarget == null)
            {
                SlowMoFailed?.Invoke();
                return;
            }
            if (currentTarget.transform == null)
            {
                SlowMoFailed?.Invoke();
                return;
            }
            //Debug.Log("Target Is Valid!");
            //Only slow if slow mo mode is active
            if (!slowMoIsReady)
            {
                SlowMoFailed?.Invoke();
                return;
            }
            //Debug.Log("SlowMo Ready!");
            //Only slow if cooldown over
            if (cooldownTimer < 1)
            {
                SlowMoFailed?.Invoke();
                return;
            }
            //Debug.Log("Cooldown Over!");

            //if target was already slowed, DEslow
            if (targetIDs.ContainsKey(currentTarget.ID))
            {
                //if (currentTarget.timer < 0) return; // return if already DeSlowing Target

                //Debug.Log("DeSlowing " + currentTarget.transform + " by command!");
                //DeSlowTarget(currentTarget);
                SlowMoFailed?.Invoke();
                return;
            }

            //Slow Target after delay
            SlowTarget(currentTarget);
            ObjectSlowed?.Invoke(currentTarget.transform, slowdownLength + slowdownExtraLength);

            if (targets.Count > maxTargets)
            {
                Debug.Log("DeSlowing " + targets[0].transform + " by max reached!");
                DeSlowTarget(targets[0]);
            }
        }

        //Methods
        void DEBUGDrawRays()
        {
            if (!slowMoIsReady) return;

            //Debug.DrawRay(mainCam.ScreenPointToRay(centerOfScreen).origin, mainCam.ScreenPointToRay(centerOfScreen).direction, Color.gray);

            Vector3 direction = mainCam.ScreenPointToRay(centerOfScreen).direction;
            //Get target (if not, deactivate oldest target)
            RaycastHit hit;
            if (!Physics.Raycast(mainCam.ScreenPointToRay(centerOfScreen), out hit, slowdownRange))
            {
                Debug.DrawRay(mainCam.transform.position, direction * slowdownRange, Color.blue);
                //Debug.Log("Hitted Nothing");
                return;
            }

            //Check if target is valid (if not, deactivate oldest target)
            ITimed target = hit.transform.GetComponent<ITimed>();
            if (target == null)
            {
                Debug.DrawRay(mainCam.transform.position, direction * slowdownRange, Color.red);
                //Debug.Log("Hitted Not Valid");
            }
            else
            {
                Debug.DrawRay(mainCam.transform.position, direction * slowdownRange, Color.green);
                //Debug.Log("Hitted Valid");
            }
        }
        void ReadySlowMo()
        {
            slowMoIsReady = !slowMoIsReady;
            SlowMoReady?.Invoke(slowMoIsReady);
            if (!slowMoIsReady)
            {
                currentTarget = null;
                TargetInScope?.Invoke(false);
            }

            //Debug.Log("SlowMo Ready: " + slowMoIsReady);
        }
        void GetValidTarget()
        {
            if (!slowMoIsReady) return; //Only slow if slow mo mode is active

            //Get target (if not, exit)
            RaycastHit hit;
            Physics.Raycast(mainCam.ScreenPointToRay(centerOfScreen), out hit, slowdownRange, slowableLayers);
            if (hit.transform == null)
            {
                currentTarget = null;
                TargetInScope?.Invoke(false);
                return;
            }

            //Check if target is valid (if not, exit)
            ITimed objectToSlow = hit.transform.GetComponent<ITimed>();
            if (objectToSlow == null)
            {
                currentTarget = null;
                TargetInScope?.Invoke(false);
                return;
            }

            //Debug.Log("Valid Target");

            //If already targetting a valid object, exit
            if (currentTarget != null && currentTarget.transform) return;

            //If target is already in list, select old one else, create new
            if (!targetIDs.TryGetValue(hit.transform.GetInstanceID(), out currentTarget))
            {
                currentTarget = new SlowMoTarget(hit.transform, objectToSlow);
            }

            //Debug.Log("Target Found!");
            TargetInScope?.Invoke(true);
        }
        void SlowTarget(SlowMoTarget target)
        {
            if (target.timer > 0) return; //Check if already slowing the object

            //Add target to list
            targets.Add(currentTarget);
            targetIDs.Add(currentTarget.ID, currentTarget);

            //Slow
            StartCoroutine(SlowRoutine(target));
        }
        void DeSlowTarget(SlowMoTarget target)
        {
            //Debug.Log("Trying to DeSlow " + target.transform + " with timer " + target.timer);
            if (target.timer < 0) return; //Check if already deSlowing the object
            //Debug.Log("DeSlowing " + target.transform + " with timer " + target.timer);

            //Get Target from List
            //targetIDs.TryGetValue(target.ID, out target);

            //Update List
            targets.Remove(target);

            //Update Timer so dictionary doesn't break
            target.timer = -1f;

            StartCoroutine(DeSlowRoutine(target));
        }
        void RemoveDestroyedObject(SlowMoTarget target)
        {
            //Update Dictionary
            targetIDs.Remove(target.ID);

            //Update List if existant
            if (targets.Contains(target))
            {
                targets.Remove(target);
            }

            //Send Event
            ObjectDestroyed?.Invoke(target.ID);
        }
        void UpdateTimers()
        {
            //Run Cooldown Timer
            if (cooldownTimer < 1)
            {
                cooldownTimer += Time.deltaTime / cooldownLength;
            }

            //Run SlowMo Timers from targets
            if (targets.Count < 1) return;
            int i = 0;
            while (i < targets.Count)
            {
                //if there is no more target, remove
                if (targets[i].transform == null)
                {
                    RemoveDestroyedObject(targets[i]);
                    continue;
                }

                //If target timer is beyond limit, deslow
                if (targets[i].timer > 1)
                {
                    //Debug.Break();
                    Debug.Log("DeSlowing " + targets[i].transform + " by time reached!");
                    DeSlowTarget(targets[i]);
                    continue;
                }

                targets[i].timer += Time.deltaTime / slowdownLength;
                //Debug.Log(targets[i] + " timer: " + targets[i].timer);
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

        //Routines
        IEnumerator SlowRoutine(SlowMoTarget target)
        {
            yield return new WaitForSecondsRealtime(slowdownDelay * Time.timeScale);
            
            //Slow target
            target.time.TimeChanged(slowdownFactor);

            //Start Cooldown
            cooldownTimer = 0;
        }
        IEnumerator DeSlowRoutine(SlowMoTarget target)
        {
            //Debug.Log("Target Unslowing at " + Time.realtimeSinceStartup + "!");

            if (target.transform == null)
            {
                RemoveDestroyedObject(target);
            }

            yield return new WaitForSecondsRealtime(slowdownExtraLength);

            //Update Dictionary
            targetIDs.Remove(target.ID);

            //Update Target Time
            //Debug.Log("Target UnSlowed at " + Time.realtimeSinceStartup + "!");
            target.time.TimeChanged(1);
            ObjectUnSlowed?.Invoke(target.transform);
        }
    }
}