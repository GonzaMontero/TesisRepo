using System.Collections.Generic;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class TimeManager : MonoBehaviourSingletonInScene<TimeManager>
    {
        [Header("Set Values")]
        [SerializeField] Transform player;
        [SerializeField] float timeFadeSpeed = 10;
        [SerializeField] float slowdownRange;
        [SerializeField] float slowdownFactor;
        [SerializeField] float slowdownLength;
        [Header("Runtime Values")]
        [SerializeField] List<ITimed> targets;
        [SerializeField] float currentTime = 1;
        [SerializeField] float targetTime = 1;
        [SerializeField] float timeCountdown = 1;


        //Unity Events
        void Update()
        {
            if (timeCountdown < 1)
            {
                timeCountdown += Time.unscaledDeltaTime / slowdownLength;
                currentTime = targetTime + (1 - targetTime) * Mathf.Pow(timeCountdown, timeFadeSpeed);
                Time.timeScale = currentTime;
                UpdateTime(true);
            }
            else if (timeCountdown != 2)
            {
                timeCountdown = 2;
                UpdateTime(false);
            }

            //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

            if (Input.GetKeyDown(KeyCode.F))
            {
                SlowMotionTime();
                UpdateTime(true);
            }
        }

        //Methods
        void SlowTarget()
        {
            //Get target
            RaycastHit hit;
            Physics.Raycast(player.position, player.forward, out hit, slowdownRange);
            if (hit.transform == null) return;
            
            //Check if target is valid
            ITimed objectToSlow = hit.transform.GetComponent<ITimed>();
            if (objectToSlow == null) return;

            //Add target to list
            targets.Add(objectToSlow);
            SlowMotionTime();

            //Start Length
            timeCountdown = 1;
        }
        void UpdateTime(bool applyModifiedTime)
        {
            GameObject[] rootGOs;
            ITimed[] timedObjects;

            rootGOs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var rootGO in rootGOs)
            {
                timedObjects = rootGO.GetComponentsInChildren<ITimed>();
                if (timedObjects == null)
                {
                    Debug.Log("No ITimed in " + rootGO);
                    continue;
                }

                foreach (ITimed timedObject in timedObjects)
                {
                    Debug.Log(timedObject);
                    timedObject.TimeChanged(applyModifiedTime);
                }

                timedObjects = null;
            }
        }
        void SlowMotionTime()
        {
            targetTime = slowdownFactor;
            timeCountdown = 0;

            //Time.timeScale = slowdownFactor;
            //Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }
}