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
        [SerializeField] float chargeLength;
        [SerializeField] int maxTargets;
        [Header("Runtime Values")]
        [SerializeField] List<SlowMoTarget> targets;
        [SerializeField] SlowMoTarget currentTarget;
        [SerializeField] Camera mainCam;
        [SerializeField] Vector2 centerOfScreen;
        [SerializeField] float chargeTimer;
        [SerializeField] bool activating;
        Dictionary<int, SlowMoTarget> targetIDs;

        public Action<bool> SlowMoReady;
        public Action<bool> TargetInScope;
        public Action<Transform, float> ObjectSlowed;
        public Action<Transform> ObjectUnSlowed;
        public Action<int> ObjectDestroyed;
        public Action SlowMoFailed;

        public float publicCharge { get { return chargeTimer; } }
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

            if (!activating) return;

            GetValidTarget();

#if UNITY_EDITOR
            DEBUGDrawRays();
#endif
        }

        //Methods
        public void Activate()
        {
            if (chargeTimer > 0) return;
            activating = true;
        }
        public void Release()
        {
            activating = false;
            if(chargeTimer < 1)
            {
                chargeTimer = 0;
                currentTarget = null;
                TargetInScope?.Invoke(false);
                //DO CANCEL

                return;
            }
            chargeTimer = 0;
            SlowTarget(currentTarget);

            currentTarget = null;
            TargetInScope?.Invoke(false);
        }
        void DEBUGDrawRays()
        {
            if (!activating) return;

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
        void GetValidTarget()
        {
            if (chargeTimer > 1) return; //Only slow if slow mo mode is active

            Ray ray = mainCam.ScreenPointToRay(centerOfScreen);

            //Get target (if not, exit)
            Transform hittedObj = null;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray.origin, ray.direction, slowdownRange, slowableLayers);

            ITimed objectToSlow = null;

            //Search for target even between old targets
            foreach (var hit in hits)
            {
                //I 
                if (targetIDs.ContainsKey(hit.transform.GetInstanceID())) continue;
                hittedObj = hit.transform;

                if (hittedObj == null)
                    continue;

                //Check if target is valid (if not, exit)
                objectToSlow = hittedObj.GetComponent<ITimed>();
                if (objectToSlow != null) break;
            }

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
            if (!targetIDs.TryGetValue(hittedObj.GetInstanceID(), out currentTarget))
            {
                currentTarget = new SlowMoTarget(hittedObj, objectToSlow);
            }

            //Debug.Log("Target Found!");
            TargetInScope?.Invoke(true);
        }
        void SlowTarget(SlowMoTarget target)
        {
            if(target == null) return;
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
            if (activating && chargeTimer < 1)
            {
                chargeTimer += Time.deltaTime / chargeLength;
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

        //Routines
        IEnumerator SlowRoutine(SlowMoTarget target)
        {
            yield return new WaitForSecondsRealtime(slowdownDelay * Time.timeScale);
            
            //Slow target
            target.time.TimeChanged(slowdownFactor);

            //Start Cooldown
            chargeTimer = 0;

            ObjectSlowed.Invoke(target.transform, target.timer);
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

            //Send event
            ObjectUnSlowed?.Invoke(target.transform);
        }
    }
}