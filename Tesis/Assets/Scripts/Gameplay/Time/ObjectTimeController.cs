using System.Collections;
using UnityEngine;

namespace TimeDistortion.Gameplay.TimePhys
{
    public class ObjectTimeController : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] float localTimeMod = 1;
        [SerializeField] float slowMoDuration = 1;
        [SerializeField] float slowMoDelay;
        [SerializeField] bool slowable = true;
        [Header("Runtime Values")]
        [SerializeField] float localTime = 1;
        [SerializeField] float timer;

        public System.Action TimeChanged;

        public float publicTime { get { return localTime; } }
        public float delta { get { return Time.deltaTime * localTime; } }
        public float slowMoLeft { get { return timer / slowMoDuration; } }

        //Unity Events
        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (!(timer > 0))
                    ChangeTime(1);
            }
        }

        //Methods
        public void ChangeTime(float newTime)
        {
            if (!slowable) return;

            if (newTime == 1)
            {
                StartCoroutine(ReleaseTimeRoutine());
                return;
            }

            if (newTime * localTimeMod == localTime) return;

            localTime = newTime * localTimeMod;
            timer = slowMoDuration;

            TimeChanged?.Invoke();
        }
        IEnumerator ReleaseTimeRoutine()
        {
            float exitTimer = slowMoDelay;
            float modifiedTime = localTime;

            while (exitTimer > 0)
            {
                exitTimer -= Time.deltaTime;

                localTime = Mathf.Lerp(1, modifiedTime, exitTimer / slowMoDelay);

                yield return null;
            }

            localTime = 1;
            timer = 0;

            TimeChanged?.Invoke();
        }
    }
}