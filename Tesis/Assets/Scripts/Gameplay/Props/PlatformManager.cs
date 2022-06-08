using System.Collections.Generic;
using UnityEngine;

namespace TimeDistortion.Gameplay.Props
{
    public class PlatformManager : MonoBehaviour, ITimed
    {
        [System.Serializable]
        class Platform
        {
            public Transform transform;
            public int step;

            public Platform(Transform _transform, int _step = 1)
            {
                transform = _transform;
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

        //Methods
        void CreatePlatform()
        {
            platforms.Add(new Platform(Instantiate(platformPrefab, platformsEmpty).transform));
            platforms[platforms.Count - 1].transform.localPosition = steps[0];
        }
        void MovePlatform(Platform platform)
        {
            //Get Platform Transform
            Transform platTransform = platform.transform;

            //Get Time
            float timeValue = localTime * Time.deltaTime;

            //Calculate Distances
            Vector3 platNextStepDistance = steps[platform.step] - platTransform.localPosition;
            Vector3 platPrevStepDistance = steps[platform.step - 1] - platTransform.localPosition;
            Vector3 stepStepDistance = steps[platform.step] - steps[platform.step -1];

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
            platTransform.Translate(platNextStepDistance.normalized * platformSpeed * timeValue);
        }

        //Interface Implementations
        public void TimeChanged(float newTime)
        {
            if (!affectedByTime) return;
            localTime = newTime;
        }
    }
}