using System.Linq;
using UnityEngine;
using Universal.Singletons;

namespace TimeDistortion.Gameplay.Physic
{
    public class PhysicsManager : MonoBehaviourSingletonInScene<PhysicsManager>
    {
        [SerializeField] float gravityValue;
        Transform root;

        public float publicGravity { get { return gravityValue; } }

        //Unity Events
        private void Start()
        {
            root = transform.root.parent;
        }

        //Methods
        public Vector3 GetObjectGravityPull(float mass)
        {
            return gravityValue * mass * Vector3.down;
        }

        #region Time THingys banana
        [SerializeField] float currentTime = 1;
        [SerializeField] float targetTime = 1;
        [SerializeField] float timeCountdown = 1;
        [SerializeField] float timeFadeSpeed = 10;
        [SerializeField] float slowdownFactor;
        [SerializeField] float slowdownLength;
        void Update()
        {
            if (timeCountdown < 1)
            {
                timeCountdown += Time.deltaTime / slowdownLength;
                currentTime = targetTime + (1 - targetTime) * Mathf.Pow(timeCountdown, timeFadeSpeed);
                UpdateTime();
            }
            //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
            //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            if (Input.GetKeyDown(KeyCode.F))
            {
                SlowMotionTime();
            }
        }
        void UpdateTime()
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
                    timedObject.TimeChanged(currentTime);
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
        #endregion
    }
}