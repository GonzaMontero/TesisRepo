using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class TimeChanger : MonoBehaviourSingletonInScene<TimeChanger>
    {
        [Header("Set Values")]
        [SerializeField] Transform player;
        [SerializeField] LayerMask slowableLayers;
        [SerializeField] float slowdownDelay;
        [SerializeField] float slowdownRange;
        [Tooltip("Speed of SlowMo")]
        [SerializeField] float slowdownFactor;
        [SerializeField] float chargeLength;
        [Tooltip("Global time while timeChange is charged")]
        [SerializeField] float chargeSlowdown;
        [SerializeField] int maxTargets;
        [Header("Runtime Values")]
        [SerializeField] Camera mainCam;
        [SerializeField] Vector2 centerOfScreen;
        [SerializeField] float chargeTimer;
        [SerializeField] bool activating;
        ITimed currentTarget;

        public Action<bool> SlowMoReady;
        public Action<bool> TargetInScope;
        public Action<Transform, float> ObjectSlowed;
        public Action<Transform> ObjectUnSlowed;
        public Action<int> ObjectDestroyed;
        public Action ActivatingCharge;
        public Action ReleasedCharge;
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
        }
        void Update()
        {
            UpdateTimers();

            if (chargeTimer < 1) return; //Only slow if slow mo mode is active

            GetValidTarget();

#if UNITY_EDITOR
            DEBUGDrawRays();
#endif
        }

        //Methods
        public void Activate() //change to "Charge"
        {
            if (chargeTimer > 0) return;
            activating = true;
            ActivatingCharge?.Invoke();
        }
        public void Release()
        {
            activating = false; //slow mo was activated, so is not activating anymore
            Time.timeScale = 1; //set timescale to default again

            ReleasedCharge?.Invoke();

            //If charge wasn't complete, cancel slowMo
            if (chargeTimer < 1)
            {
                chargeTimer = 0;
                currentTarget = null;
                TargetInScope?.Invoke(false);

                return;
            }

            //If charge was complete, slow obj
            chargeTimer = 0;
            ActivateTarget();

            currentTarget = null;
            TargetInScope?.Invoke(false);
        }
        void DEBUGDrawRays()
        {
            if (chargeTimer < 1) return; //Only slow if slow mo mode is active

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
            if (chargeTimer < 1) return; //Only slow if slow mo mode is active

            Ray ray = mainCam.ScreenPointToRay(centerOfScreen);

            //Get target (if not, exit)
            Transform hittedObj = null;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray.origin, ray.direction, slowdownRange, slowableLayers);

            ITimed objectToSlow = null;

            //Search for target even between old targets
            foreach (var hit in hits)
            {
                hittedObj = hit.transform;

                if (hittedObj == null) continue;

                //Check if target is valid (if not, skip)
                objectToSlow = hittedObj.GetComponent<ITimed>();
                if (objectToSlow == null) continue;

                //Check if target is already slowed, if it is, skip
                ObjectTimeController objTime;
                objTime = hittedObj.GetComponent<ObjectTimeController>();
                if (objTime != null)
                    if (objTime.slowMoLeft > 0)
                    {
                        //objectToSlow = null;
                        continue;
                    }

                break;
            }

            if (objectToSlow == null)
            {
                currentTarget = null;
                TargetInScope?.Invoke(false);
                return;
            }

            Debug.Log("Valid Target");

            //If already targetting a valid object, exit
            if (currentTarget != null) return;

            currentTarget = objectToSlow;

            Debug.Log("Target Found!");
            TargetInScope?.Invoke(true);
        }
        void ActivateTarget()
        {
            //If there's no target, exit
            if (currentTarget == null)
            {
                //Restart Charge
                chargeTimer = 0;
                return;
            }

            StartCoroutine(SlowRoutine(currentTarget));
        }
        void UpdateTimers()
        {
            //Run Cooldown Timer
            if (activating && chargeTimer < 1)
            {
                chargeTimer += Time.deltaTime / chargeLength;
                
                if(chargeTimer >= 1)
                {
                    chargeTimer = 1;
                    activating = false;
                }

                Time.timeScale = Mathf.Lerp(1, chargeSlowdown, chargeTimer);
            }
        }

        //Routines
        IEnumerator SlowRoutine(ITimed target)
        {
            yield return new WaitForSeconds(slowdownDelay);
            
            //Slow target
            target.ChangeTime(slowdownFactor);

            //Restart Charge
            chargeTimer = 0;

            //ObjectSlowed.Invoke(target.transform, target.timer);
        }
    }
}