using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class TimeManager : MonoBehaviourSingletonInScene<TimeManager>
    {
        [Serializable]
        class SlowMoTarget
        {
            public ITimed time;
            public float timer;

            public SlowMoTarget(ITimed newTarget, float newTimer)
            {
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
        [SerializeField] float cooldownTimer;

        //Unity Events
        private void Start()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }

            Time.timeScale = slowdownFactor;
        }
        void Update()
        {
#if UNITY_EDITOR
            Debug.DrawRay(player.position, player.forward * slowdownRange, Color.blue);
#endif

            UpdateTimers();

            if (cooldownTimer < 1) return; //Only slow if cooldown over
            if (Input.GetKeyDown(KeyCode.LeftControl) && PlayerIsOnFloor())
            {
                SlowTarget();

                if (targets.Count >= maxTargets)
                {
                    DeSlowTarget(targets[0]);
                }
            }
        }

        //Methods
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
            targets.Add(new SlowMoTarget(objectToSlow, 0));
            objectToSlow.TimeChanged(true);

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
            bool playerOnFloor = Physics.Raycast(player.position, -player.up, 2);

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