using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PlatformManager : MonoBehaviour, ITimed
    {
        [System.Serializable]
        class Platform
        {
            public PlatformController controller;
            public int step;

            public Platform(Transform _controller, int _step = 1)
            {
                controller = _controller.GetComponent<PlatformController>();
                step = _step;
            }
        }

        [Header("Set Values")]
        [SerializeField] List<Vector3> steps;
        [SerializeField] GameObject platformPrefab;
        [SerializeField] Transform platformsEmpty;
        [SerializeField] float platformSpeed;
        [SerializeField] float timeBetweenPlatforms;
        [SerializeField] [Tooltip("Max Platforms in screen")] int numberOfPlatforms;
        [SerializeField] bool affectedByTime = true;
        [Header("Runtime Values")]
        [SerializeField] List<Platform> platforms;
        [SerializeField] float timer;
        [SerializeField] float localTime = 1;

        //Unity Events
        private void Start()
        {
            //Initialize List and Create First Platform
            platforms = new List<Platform>();
            //CreatePlatform();
            if (platformsEmpty == null)
            {
                platformsEmpty = transform;
            }
        }
        private void Update()
        {
            foreach (var platform in platforms)
            {
                MovePlatform(platform);
            }

            //Instantiate one platform after X seconds had passed
            if (platforms.Count >= numberOfPlatforms) return;
            timer += localTime * Time.deltaTime;
            if (timer > timeBetweenPlatforms)
            {
                CreatePlatform();
                timer = 0;
            }
        }
#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void OnDrawGizmos()
        {
            Color newColor;
            for (int i = 0; i < steps.Count; i++)
            {
                //Get step color
                newColor = Color.white;
                newColor.r *= (i + 1) / (float)steps.Count;
                newColor.g *= (i + 1) / (float)steps.Count;
                newColor.b *= (i + 1) / (float)steps.Count;

                //Set step color
                Gizmos.color = newColor;

                //Draw step cube
                Gizmos.DrawCube(transform.position + steps[i], Vector3.one * 0.25f);
            }
        }
#endif

        //Methods
        void CreatePlatform()
        {
            platforms.Add(new Platform(Instantiate(platformPrefab, platformsEmpty).transform));
            platforms[platforms.Count - 1].controller.transform.localPosition = steps[0];
        }
        void MovePlatform(Platform platform)
        {
            //Get Platform Transform
            Transform platTransform = platform.controller.transform;

            //Get Time
            float timeValue = localTime * Time.deltaTime;

            //Calculate Distances
            Vector3 platNextStepDistance = steps[platform.step] - platTransform.localPosition;
            Vector3 platPrevStepDistance = steps[platform.step - 1] - platTransform.localPosition;
            Vector3 stepStepDistance = steps[platform.step] - steps[platform.step -1];
            Vector3 platNextStepDir = platNextStepDistance.normalized;

            //Check if platform already reached next step
            if ((platPrevStepDistance).magnitude >= (stepStepDistance).magnitude)
            {
                platform.step++;
                if (platform.step >= steps.Count)
                {
                    platform.step = 1;
                    platTransform.localPosition = steps[0];
                }
            }

            //Move Platform
            platform.controller.Move(platNextStepDir * platformSpeed * timeValue);
        }

        //Interface Implementations
        public void TimeChanged(float newTime)
        {
            if (!affectedByTime) return;
            localTime = newTime;
        }
    }
}